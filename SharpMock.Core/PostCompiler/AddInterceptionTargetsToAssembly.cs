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
            fake = module.UnitNamespaceRoot.AddNestedNamespace("<Fake>", host);
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
            host.LoadUnit(host.CoreAssemblySymbolicIdentity);

            foreach (var method in MethodReferenceReplacementRegistry.GetMethodsToIntercept())
            {
                var nsType = method.ContainingType.GetNamespaceType();
                var fullNs = nsType.NamespaceBuilder();
                var fullNsWithType = String.Format("{0}.{1}", fullNs, nsType.Name.Value);

                log.WriteTrace("Adding interception target for '{0}'.", fullNsWithType);

                AddNamespaces(fullNs);
                AddClass(fullNs.ToString(), nsType.Name.Value);

                var methodClass = classes[fullNsWithType];
                var methodName = method.Name.Value == ".ctor" ? "<constructor>" : method.Name.Value;
                var fakeMethod = methodClass.AddPublicStaticMethod(methodName, method.Type, host);

                // if it's an instance method, we add a parameter at the end for the target
                ushort extraParameters = 0;
                if (!method.ResolvedMethod.IsStatic && !method.ResolvedMethod.IsConstructor)
                {
                    fakeMethod.AddParameter(0, "target", method.ContainingType, host);
                    extraParameters = 1;
                }

                foreach (var parameter in method.Parameters)
                {
                    fakeMethod.AddParameter((ushort)(parameter.Index + extraParameters), 
                        "p" + parameter.Index, parameter.Type, host);
                }

                if (method.ResolvedMethod.IsConstructor)
                {
                    fakeMethod.Type = method.ResolvedMethod.ContainingTypeDefinition;
                }

                var customAttribute = new CustomAttribute();
                customAttribute.Constructor = new UnitReflector(host)
                    .From<SharpMockGeneratedAttribute>().GetConstructor(Type.EmptyTypes);
                fakeMethod.Attributes = new List<ICustomAttribute>();
                fakeMethod.Attributes.Add(customAttribute);
                fakeMethod.Body = GetBody(host, fakeMethod, method);

                var parameterTypes = new List<ITypeDefinition>();
                foreach (var param in fakeMethod.Parameters)
                {
                    parameterTypes.Add(param.Type.ResolvedType);
                }

                var fakeCallReference = new Microsoft.Cci.MethodReference(host, fakeMethod.ContainingTypeDefinition,
                    fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name, 0, parameterTypes.ToArray());

                MethodReferenceReplacementRegistry.ReplaceWith(method, fakeCallReference);
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