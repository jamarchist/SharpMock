using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.CciExtensions;
using SharpMock.Core.PostCompiler.Construction.Assemblies;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.PostCompiler.Core;
using SharpMock.PostCompiler.Core.CciExtensions;
using Assembly = Microsoft.Cci.MutableCodeModel.Assembly;
using MethodReference = Microsoft.Cci.MethodReference;
using Module = Microsoft.Cci.MutableCodeModel.Module;
using QualifiedMethodPath = SharpMock.Core.PostCompiler.Replacement.QualifiedMethodPath;
using SourceMethodBody = Microsoft.Cci.MutableCodeModel.SourceMethodBody;

namespace SharpMock.Core.PostCompiler
{
	public class PostCompiler
	{
		private readonly PostCompilerArgs postCompilerArgs;
	    private readonly IMetadataHost host;
	    private readonly IAssembly sharpMockDelegateTypes;
	    private readonly IUnit sharpMockCore;

		public PostCompiler(PostCompilerArgs postCompilerArgs)
		{
            if (postCompilerArgs.AreValid())
            {
                var nameTable = new NameTable();
                host = new PeReader.DefaultHost(nameTable);

                sharpMockCore = host.LoadUnitFrom(
                            System.Reflection.Assembly.GetExecutingAssembly().Location
                        );
                sharpMockDelegateTypes = sharpMockCore as IAssembly;

                host.Errors += host_Errors;            
            }

			this.postCompilerArgs = postCompilerArgs;
        }
        
        public void InterceptSpecifications()
        {
            System.Diagnostics.Debugger.Launch();

            var mutableAssembly = GetMutableAssembly(postCompilerArgs.TestAssemblyPath, host);
            mutableAssembly.AssemblyReferences.Add(sharpMockDelegateTypes);

            LoadReferencedAssemblies(mutableAssembly, host);

            ScanForInterceptionSpecifications(mutableAssembly, host);
            AddInterceptionTargets(mutableAssembly, host);
            var modifiedAssembly = ReplaceSpecifiedStaticMethodCalls(host, mutableAssembly);

            SaveAssembly(postCompilerArgs.TestAssemblyPath, modifiedAssembly, host);

            var autoSpecs = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(postCompilerArgs.TestAssemblyPath), "AutoSpecs.xml");
            SerializeSpecifications(autoSpecs, MethodReferenceReplacementRegistry.GetReplaceables());
            SerializeExplicitSpecifications(postCompilerArgs.TestAssemblyPath);
        }

        private void SerializeExplicitSpecifications(string specAssembly)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(specAssembly);
            var specs = new List<Type>(assembly.GetTypes())
                .FindAll(t => typeof(IReplacementSpecification).IsAssignableFrom(t));

            var specifiedMethods = new List<ReplaceableMethodInfo>();

            foreach (var specType in specs)
            {
                var spec = Activator.CreateInstance(specType) as IReplacementSpecification;
                specifiedMethods.AddRange(spec.GetMethodsToReplace());
            }

            var specPath = Path.GetDirectoryName(specAssembly);
            var specAssemblyName = Path.GetFileNameWithoutExtension(specAssembly);
            var serializedSpecName = String.Format("{0}.SharpMock.SerializedSpecifications.xml", specAssemblyName);

            var fullSpecPath = Path.Combine(specPath, serializedSpecName);
            SerializeSpecifications(fullSpecPath, specifiedMethods);
        }

        public static void SerializeSpecifications(string filename, IList<ReplaceableMethodInfo> specs)
        {
            var specList = new List<ReplaceableMethodInfo>(specs);
            var serializer = new XmlSerializer(typeof(List<ReplaceableMethodInfo>));
            using (var binFile = File.Create(filename))
            {
                serializer.Serialize(binFile, specList);
                binFile.Close();
            }            
        }

        public void InterceptAllStaticMethodCalls()
        {
            var mutableAssembly = GetMutableAssembly(postCompilerArgs.ReferencedAssemblyPath, host);       
            mutableAssembly.AssemblyReferences.Add(sharpMockDelegateTypes);

            LoadReferencedAssemblies(mutableAssembly, host);

            ScanForStaticMethodCalls(mutableAssembly, host);
            AddInterceptionTargets(mutableAssembly, host);

            var modifiedAssembly = ReplaceStaticMethodCalls(host, mutableAssembly);
            SaveAssembly(postCompilerArgs.ReferencedAssemblyPath, modifiedAssembly, host);
        }

        private static void host_Errors(object sender, Microsoft.Cci.ErrorEventArgs e)
        {
            foreach (var error in e.Errors)
            {
                Console.WriteLine(error.Message);
            }
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

            var replaceables = MethodReferenceReplacementRegistry.GetReplaceables();


            return assembly;
        }

        private static IAssembly ScanForStaticMethodCalls(IAssembly assembly, IMetadataHost host)
        {
            var registrar = new StaticMethodCallRegistrar(host, assembly.Location);
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
            assemblyStream.Dispose();
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
