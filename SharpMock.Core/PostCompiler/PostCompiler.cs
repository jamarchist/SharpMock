using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.CciExtensions;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core.CodeConstruction;
using AssemblyReference = Microsoft.Cci.MutableCodeModel.AssemblyReference;
using MethodReference = Microsoft.Cci.MethodReference;
using NamespaceTypeReference = Microsoft.Cci.MutableCodeModel.NamespaceTypeReference;
using NestedUnitNamespaceReference = Microsoft.Cci.MutableCodeModel.NestedUnitNamespaceReference;
using RootUnitNamespaceReference = Microsoft.Cci.MutableCodeModel.RootUnitNamespaceReference;
using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;
using SharpMock.PostCompiler.Core.CciExtensions;
using QualifiedMethodPath = SharpMock.Core.PostCompiler.Replacement.QualifiedMethodPath;

namespace SharpMock.PostCompiler.Core
{
	public class PostCompiler
	{
		private readonly PostCompilerArgs postCompilerArgs;

		public PostCompiler(PostCompilerArgs postCompilerArgs)
		{
			this.postCompilerArgs = postCompilerArgs;
		}

        public void InterceptSpecifications()
        {
            if (postCompilerArgs.AreValid())
            {
                var nameTable = new NameTable();
                var host = new PeReader.DefaultHost(nameTable);

                host.Errors += host_Errors;

                var mutableAssembly = GetMutableAssembly(postCompilerArgs.TestAssemblyPath, host);
                var sharpMockCore = host.LoadUnitFrom(
                        System.Reflection.Assembly.GetExecutingAssembly().Location
                    );
                var sharpMockDelegateTypes = sharpMockCore as IAssembly;

                mutableAssembly.AssemblyReferences.Add(sharpMockDelegateTypes);

                LoadReferencedAssemblies(mutableAssembly, host);

                ScanForInterceptionSpecifications(mutableAssembly, host);
                AddInterceptionTargets(mutableAssembly, host);
                var modifiedAssembly = ReplaceSpecifiedStaticMethodCalls(host, mutableAssembly);

                SaveAssembly(postCompilerArgs.TestAssemblyPath, modifiedAssembly, host);
            }            
        }

        public void InterceptAllStaticMethodCalls()
        {
            if (postCompilerArgs.AreValid())
            {
                var nameTable = new NameTable();
                var host = new PeReader.DefaultHost(nameTable);

                host.Errors += host_Errors;

                var mutableAssembly = GetMutableAssembly(postCompilerArgs.ReferencedAssemblyPath, host);
                var sharpMockCore = host.LoadUnitFrom(
                        System.Reflection.Assembly.GetExecutingAssembly().Location
                    );
                var sharpMockDelegateTypes = sharpMockCore as IAssembly;

                mutableAssembly.AssemblyReferences.Add(sharpMockDelegateTypes);

                LoadReferencedAssemblies(mutableAssembly, host);

                ScanForStaticMethodCalls(mutableAssembly, host);
                AddInterceptionTargets(mutableAssembly, host);
                var modifiedAssembly = ReplaceStaticMethodCalls(host, mutableAssembly);

                SaveAssembly(postCompilerArgs.ReferencedAssemblyPath, mutableAssembly, host);
            }
        }

        private static void host_Errors(object sender, Microsoft.Cci.ErrorEventArgs e)
        {
            Console.WriteLine("Error...");
        }

        private static Assembly GetMutableAssembly(string startingAssembly, IMetadataHost host)
        {
            var testAssembly = host.LoadUnitFrom(startingAssembly) as IAssembly;
            return Decompiler.GetCodeModelFromMetadataModel(host, testAssembly, null, DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators | DecompilerOptions.Loops);
        }

        private static void LoadReferencedAssemblies(IUnit assembly, IMetadataHost host)
        {
            foreach (var reference in assembly.UnitReferences)
            {
                if (reference != null)
                {
                    var unit = host.LoadUnit(reference.UnitIdentity);
                    LoadReferencedAssemblies(unit, host);   
                }
            }
        }

        private static IAssembly ScanForInterceptionSpecifications(IAssembly assembly, IMetadataHost host)
        {
            var registrar = new SpecifiedMethodCallRegistrar(host);
            registrar.Visit(assembly);
            return assembly;
        }

        private static IAssembly ScanForStaticMethodCalls(IAssembly assembly, IMetadataHost host)
        {
            var registrar = new StaticMethodCallRegistrar(host);
            registrar.Visit(assembly);
            return assembly;
        }

        private static IAssembly ReplaceSpecifiedStaticMethodCalls(IMetadataHost host, IAssembly mutableAssembly)
        {
            var methodCallReplacer = new SpecifiedMethodCallReplacer(host);
            methodCallReplacer.Visit(mutableAssembly);
            return mutableAssembly;
        }

        private static IAssembly ReplaceStaticMethodCalls(IMetadataHost host, IAssembly mutableAssembly)
        {
            var methodCallReplacer = new StaticMethodCallReplacer(host);
            methodCallReplacer.Visit(mutableAssembly);
            return mutableAssembly;
        }

        private static void SaveAssembly(string assemblyName, IModule mutableAssembly, IMetadataHost host)
        {
			var assemblyStream = File.OpenWrite(assemblyName);
			PeWriter.WritePeToStream(mutableAssembly, host, assemblyStream);       
            assemblyStream.Close();
        }

        private static IList<INamespaceTypeDefinition> nestedClasses = new List<INamespaceTypeDefinition>();

        private static void AddInterceptionTargets(Module mutableAssembly, IMetadataHost host)
		{
		    var createdPaths = new Dictionary<QualifiedMethodPath, NamespaceTypeDefinition>();

			var coreAssembly = host.LoadUnit(host.CoreAssemblySymbolicIdentity);

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

				fakeMethod.Body = GetBody(host, fakeMethod, method);

			    var parameterTypes = new List<ITypeDefinition>();
                foreach (var param in fakeMethod.Parameters)
                {
                    parameterTypes.Add(param.Type.ResolvedType);
                }

                var fakeCallReference = new MethodReference(host, fakeMethod.ContainingTypeDefinition,
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
