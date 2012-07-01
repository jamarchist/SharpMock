using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Helpers;
using SharpMock.Core.PostCompiler.CciExtensions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core.CciExtensions;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler
{
    public class AddInterceptionTargetsToAssembly : IPostCompilerPipelineStep
    {
        private NestedUnitNamespace fake;
        private IDictionary<string, NestedUnitNamespace> namespaces;
        private IDictionary<string, NamespaceTypeDefinition> classes;
        private IMetadataHost host;
        private Module module;
        private ILogger log;

        public void Execute(PostCompilerContext context)
        {
            host = context.Host;
            module = context.AssemblyToAlter;
            log = context.Log;

            ConfigureLocals();
            AddInterceptionTargets();    
        }

        private void ConfigureLocals()
        {
            fake = module.UnitNamespaceRoot.AddNestedNamespace("Fake", host);
            namespaces = new Dictionary<string, NestedUnitNamespace>();
            classes = new Dictionary<string, NamespaceTypeDefinition>();
        }

        private void AddNamespaces(ReverseStringBuilder reversedNamespaces)
        {
            log.WriteTrace("Adding fake namespace: '{0}'.", reversedNamespaces);
            var allNamespaces = reversedNamespaces.ToStringArray();
            var stack = StackedNamespaces(allNamespaces);
            
            foreach (var ns in stack)
            {
                if (!namespaces.ContainsKey(ns.Key))
                {
                    NestedUnitNamespace root = null;
                    if (namespaces.ContainsKey(ns.Value.Root))
                        root = namespaces[ns.Value.Root];
                    else
                        root = fake.AddNestedNamespace(ns.Value.Root, host);

                    var newNamespace = root.AddNestedNamespace(ns.Value.LastElement, host);
                    namespaces.Add(ns.Key, newNamespace);
                }
            }
        }

        private void AddClass(string fullNamespace, string className)
        {
            var fullyQualifiedName = String.Format("{0}.{1}", fullNamespace, className);
            log.WriteTrace("Adding fake class: '{0}'.", fullyQualifiedName);
            if (!classes.ContainsKey(fullyQualifiedName))
            {
                var ns = namespaces[fullNamespace];
                var newClass = ns.AddStaticClass(module, className, host);

                classes.Add(fullyQualifiedName, newClass);
            }
        }

        private IDictionary<string, NamespaceInfo> StackedNamespaces(string[] namespaceElements)
        {
            var stack = new Dictionary<string, NamespaceInfo>();
            for (var elementIndex = 0; elementIndex < namespaceElements.Length; elementIndex++)
            {
                var stackElement = StackedNamespace(namespaceElements, elementIndex);

                var namespaceInfo = new NamespaceInfo();
                namespaceInfo.FullNamespace = stackElement;
                namespaceInfo.LastElement = namespaceElements[elementIndex];
                namespaceInfo.Root = StackedNamespace(namespaceElements, elementIndex - 1);

                stack.Add(namespaceInfo.FullNamespace, namespaceInfo);
            }

            return stack;
        }

        private string StackedNamespace(string[] elements, int lastElementIndex)
        {
            var stackElement = new StringBuilder();
            for (var subIndex = 0; subIndex <= lastElementIndex; subIndex++)
            {
                stackElement.Append(elements[subIndex]);
                stackElement.Append('.');
            }

            var stack = stackElement.ToString();

            if (stack.Length > 0)
            {
                return stack.Trim('.');
            }

            return stack;
        }

        private void AddInterceptionTargets()
        {
            //var createdPaths = new Dictionary<QualifiedMethodPath, NamespaceTypeDefinition>();
            var createdPaths = new Dictionary<string, NamespaceTypeDefinition>();

            host.LoadUnit(host.CoreAssemblySymbolicIdentity);

            //var fakeNamespace = mutableAssembly.UnitNamespaceRoot.AddNestedNamespace("Fake", host);

            foreach (var method in MethodReferenceReplacementRegistry.GetMethodsToIntercept())
            {
                //var qualifiedMethodPath = new QualifiedMethodPath();

                var nsType = method.ContainingType.GetNamespaceType();
                var fullNs = nsType.NamespaceBuilder();
                var fullNsWithType = String.Format("{0}.{1}", fullNs, nsType.Name.Value);

                log.WriteTrace("Adding interception target for '{0}'.", fullNsWithType);

                AddNamespaces(fullNs);
                AddClass(fullNs.ToString(), nsType.Name.Value);

                //AggregateDeclarationSpaces(qualifiedMethodPath, method, false);

                //NamespaceTypeDefinition methodClass;
                ////if (!createdPaths.ContainsKey(qualifiedMethodPath))
                //if (!createdPaths.ContainsKey(fullNsWithType))
                //{
                //    IUnitNamespace lastNs = fakeNamespace;
                //    foreach (var element in qualifiedMethodPath)
                //    {
                //        lastNs = lastNs.AddNestedNamespace(element, host);
                //    }

                //    methodClass = lastNs.AddStaticClass(mutableAssembly, qualifiedMethodPath.GetClassName(), host);

                //    //createdPaths.Add(qualifiedMethodPath, methodClass);
                //    createdPaths.Add(fullNsWithType, methodClass);
                //}
                //else
                //{
                //    methodClass = createdPaths[fullNsWithType];
                //    //methodClass = createdPaths[qualifiedMethodPath];
                //}

                var methodClass = classes[fullNsWithType];
                var fakeMethod = methodClass.AddPublicStaticMethod(method.Name.Value, method.Type, host);

                // if it's an instance method, we add a parameter at the end for the target
                ushort extraParameters = 0;
                if (!method.ResolvedMethod.IsStatic)
                {
                    fakeMethod.AddParameter(0, "target", method.ContainingType, host);
                    extraParameters = 1;
                }

                foreach (var parameter in method.Parameters)
                {
                    fakeMethod.AddParameter((ushort)(parameter.Index + extraParameters), "p" + parameter.Index, parameter.Type, host);
                }

                var customAttribute = new CustomAttribute();
                customAttribute.Constructor = new UnitReflector(host).From<SharpMockGeneratedAttribute>().GetConstructor();
                fakeMethod.Attributes = new List<ICustomAttribute>();
                fakeMethod.Attributes.Add(customAttribute);
                fakeMethod.Body = GetBody(host, fakeMethod, method);

                var parameterTypes = new List<ITypeDefinition>();
                foreach (var param in fakeMethod.Parameters)
                {
                    parameterTypes.Add(param.Type.ResolvedType);
                }

                var fakeCallReference = new Microsoft.Cci.MethodReference(host, fakeMethod.ContainingTypeDefinition,
                                                                          fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name,
                                                                          0, parameterTypes.ToArray());

                MethodReferenceReplacementRegistry.ReplaceWith(method, fakeCallReference);
            }
        }

        private static void AggregateDeclarationSpaces(QualifiedMethodPath path, INamedEntity namedEntity, bool isClass)
        {
            if (namedEntity as IMethodReference == null && !isClass)
            {
                path.AddPathElement(namedEntity.Name.Value);
            }

            var assembly = namedEntity as AssemblyReference;
            if (assembly != null)
            {
                return;
                // exit
            }

            var method = namedEntity as IMethodReference;
            if (method != null)
            {
                path.AddClassName((method.ContainingType as INamedEntity).Name.Value);

                var nestedTypeParentClass = method.ContainingType as NestedTypeReference;
                if (nestedTypeParentClass != null)
                {
                    AggregateDeclarationSpaces(path, nestedTypeParentClass, true);
                }

                var nestedTypeParentNamespace = method.ContainingType as NamespaceTypeReference;
                if (nestedTypeParentNamespace != null)
                {
                    AggregateDeclarationSpaces(path, nestedTypeParentNamespace, true);
                }
            }

            var typeDeclaration = namedEntity as NamespaceTypeReference;
            if (typeDeclaration != null)
            {
                var typeParent = typeDeclaration.ContainingUnitNamespace as NestedUnitNamespaceReference;
                if (typeParent != null)
                {
                    AggregateDeclarationSpaces(path, typeParent, false);
                }

                var typeParent2 = typeDeclaration.ContainingUnitNamespace as RootUnitNamespaceReference;
                if (typeParent2 != null)
                {
                    AggregateDeclarationSpaces(path, typeParent2.Unit, false);
                }
                //AggregateDeclarationSpaces(path, typeDeclaration.ContainingUnitNamespace.Unit);
            }

            var nestedNamespace = namedEntity as NestedUnitNamespaceReference;
            if (nestedNamespace != null)
            {
                var container = nestedNamespace.ContainingUnitNamespace as NestedUnitNamespaceReference;
                if (container != null)
                {
                    AggregateDeclarationSpaces(path, container, false);
                }

                var containerNamespace = nestedNamespace.ContainingUnitNamespace as RootUnitNamespaceReference;
                if (containerNamespace != null)
                {
                    AggregateDeclarationSpaces(path, containerNamespace.Unit, false);
                }
            }

            var nestedTypeDefinition = namedEntity as NestedTypeReference;
            if (nestedTypeDefinition != null)
            {
                var nestedTypeParentClass = nestedTypeDefinition.ContainingType as NestedTypeReference;
                if (nestedTypeParentClass != null)
                {
                    AggregateDeclarationSpaces(path, nestedTypeParentClass, false);
                }

                var nestedTypeParentNamespace = nestedTypeDefinition.ContainingType as NamespaceTypeReference;
                if (nestedTypeParentNamespace != null)
                {
                    AggregateDeclarationSpaces(path, nestedTypeParentNamespace, false);
                }
            }
        }

        private static void AddAlternativeInvocation(BlockStatement block,
            IMethodDefinition fakeMethod, IMethodReference originalCall, IMetadataHost host)
        {
            var context = new ReplacementMethodConstructionContext(host, originalCall, fakeMethod, block);
            var methodBuilder = context.GetMethodBuilder();

            methodBuilder.BuildMethod();
        }

        private static SourceMethodBody GetBody(IMetadataHost host, IMethodDefinition method, IMethodReference originalCall)
        {
            var coreAssembly = host.LoadUnit(host.CoreAssemblySymbolicIdentity);
            var nameTable = host.NameTable;

            var body = new SourceMethodBody(host, null, null);
            body.MethodDefinition = method;
            body.LocalsAreZeroed = true;

            var block = new BlockStatement();
            body.Block = block;

            AddAlternativeInvocation(block, method, originalCall, host);

            return body;
        }

        private class NamespaceInfo
        {
            public string FullNamespace { get; set; }
            public string LastElement { get; set; }
            public string Root { get; set; }
        }
    }
}