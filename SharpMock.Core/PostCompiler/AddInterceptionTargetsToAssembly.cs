using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.CciExtensions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core.CciExtensions;

namespace SharpMock.Core.PostCompiler
{
    public class AddInterceptionTargetsToAssembly : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            AddInterceptionTargets(context.AssemblyToAlter, context.Host);
        }


        private static void AddInterceptionTargets(Module mutableAssembly, IMetadataHost host)
        {
            //var newAssemblyPath = mutableAssembly.Location;
            //var referencePaths = new List<string>();
            //foreach (var assemblyReference in mutableAssembly.AssemblyReferences)
            //{
            //    referencePaths.Add(assemblyReference.ResolvedAssembly.Location);
            //}

            //var newAssembly = new AssemblyBuilder().CreateNewDll(with =>
            //                                                         {
            //                                                             with.Name(mutableAssembly.Name.Value + ".Fakes");
            //                                                             foreach (var rp in referencePaths)
            //                                                             {
            //                                                                 with.ReferenceTo.Assembly(rp);
            //                                                             }
            //                                                             with.Type.Class.Public.Static.
            //                                                         })

            var createdPaths = new Dictionary<QualifiedMethodPath, NamespaceTypeDefinition>();

            host.LoadUnit(host.CoreAssemblySymbolicIdentity);

            var fakeNamespace = mutableAssembly.UnitNamespaceRoot.AddNestedNamespace("Fake", host);

            foreach (var method in MethodReferenceReplacementRegistry.GetMethodsToIntercept())
            {
                var qualifiedMethodPath = new QualifiedMethodPath();
                AggregateDeclarationSpaces(qualifiedMethodPath, method, false);

                NamespaceTypeDefinition methodClass;
                if (!createdPaths.ContainsKey(qualifiedMethodPath))
                {
                    IUnitNamespace lastNs = fakeNamespace;
                    foreach (var element in qualifiedMethodPath)
                    {
                        lastNs = lastNs.AddNestedNamespace(element, host);
                    }

                    methodClass = lastNs.AddStaticClass(mutableAssembly, qualifiedMethodPath.GetClassName(), host);

                    createdPaths.Add(qualifiedMethodPath, methodClass);
                }
                else
                {
                    methodClass = createdPaths[qualifiedMethodPath];
                }

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

    }
}