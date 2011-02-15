using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
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
            return Decompiler.GetCodeModelFromMetadataModel(host, testAssembly, null);    
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

			        //var ns = mutableAssembly.UnitNamespaceRoot.AddNestedNamespace(method.Name.Value + "NS", host);
			        methodClass = lastNs.AddStaticClass(mutableAssembly, qualifiedMethodPath.GetClassName(), host);

                    createdPaths.Add(qualifiedMethodPath, methodClass);
                }
                else
                {
                    methodClass = createdPaths[qualifiedMethodPath];
                }

			    var fakeMethod = methodClass.AddPublicStaticMethod(method.Name.Value, method.Type, host);

				foreach (var parameter in method.Parameters)
				{
				    fakeMethod.AddParameter(parameter.Index, "p" + parameter.Index, parameter.Type, host);
				}

				fakeMethod.Body = GetBody(host, fakeMethod, method);

                var fakeCallReference = new MethodReference(host, fakeMethod.ContainingTypeDefinition,
                    fakeMethod.CallingConvention, fakeMethod.Type, fakeMethod.Name,
                    0, method.Parameters);

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

            #region Delegate Invocation Code

            //var delegateCallDeclaration = new LocalDeclarationStatement();
            //var delegateReturnVariableDefinition = new LocalDefinition();
            //delegateReturnVariableDefinition.Name = host.NameTable.GetNameFor("delval");
            //delegateReturnVariableDefinition.Type = toCall.Type;

            //var invoke = new SpecializedMethodReference();
            //invoke.Name = host.NameTable.GetNameFor("Invoke");
            //invoke.CallingConvention = CallingConvention.HasThis;
            //invoke.Type = toCall.Type;
            //invoke.ContainingType = closedGenericFunction;
            //invoke.InternFactory = host.InternFactory;

            //var genericParameter = new Microsoft.Cci.MutableCodeModel.GenericTypeParameterReference();
            //genericParameter.DefiningType = openGenericFunctionWithNoArgs;
            //genericParameter.InternFactory = host.InternFactory;
            //genericParameter.TypeCode = PrimitiveTypeCode.NotPrimitive;
            //genericParameter.Name = host.NameTable.GetNameFor("!0");

            //var unspecializedInvoke = new Microsoft.Cci.MutableCodeModel.MethodReference();
            //unspecializedInvoke.ContainingType = openGenericFunctionWithNoArgs;
            //unspecializedInvoke.Type = genericParameter;
            //unspecializedInvoke.Name = host.NameTable.GetNameFor("Invoke");
            //unspecializedInvoke.CallingConvention = CallingConvention.HasThis;
            //unspecializedInvoke.InternFactory = host.InternFactory;

            //invoke.UnspecializedVersion = unspecializedInvoke;

            //var delval = new BoundExpression();
            //delval.Definition = delegateDefinition;
            //delval.Type = closedGenericFunction;

            //var callInvoke = new MethodCall();
            //callInvoke.MethodToCall = invoke;
            //callInvoke.Type = toCall.Type;
            //callInvoke.ThisArgument = delval;
            //callInvoke.IsVirtualCall = true;

            //delegateCallDeclaration.InitialValue = callInvoke;
            //delegateCallDeclaration.LocalVariable = delegateReturnVariableDefinition;

            #endregion

            #region Old Code

            // preserve this code

            ////////var codeBuilder = new BlockBuilder(host);

            ////////var @delegate = new AnonymousDelegate();
            ////////foreach (var parameter in fakeMethod.Parameters)
            ////////{
            ////////    var delegateParameter = new ParameterDefinition();
            ////////    delegateParameter.Name = parameter.Name;
            ////////    delegateParameter.Type = parameter.Type;

            ////////    @delegate.Parameters.Add(delegateParameter);
            ////////}
            ////////@delegate.ReturnType = fakeMethod.Type;

            ////////var ass =
            ////////    host.LoadUnitFrom(
            ////////        @"C:\Documents and Settings\Administrator\My Documents\My Dropbox\Source\GoogleCode\SharpMock\SharpMock\SharpMock.Core.DelegateTypes\bin\Debug\SharpMock.Core.DelegateTypes.dll");
            ////////var sharpMockDelegateTypes = ass as IAssembly;

            ////////INamedTypeReference functionWithNoArgs = null;
            ////////INamedTypeReference functionWithOneArg = null;

            ////////foreach (var type in sharpMockDelegateTypes.GetAllTypes())
            ////////{
            ////////    if (type.GenericParameterCount == 1)
            ////////    {
            ////////        functionWithNoArgs = type;
            ////////    }

            ////////    if (type.GenericParameterCount == 2)
            ////////    {
            ////////        functionWithOneArg = type;
            ////////    }
            ////////}

            ////////var delegateBody = new BlockStatement();
            ////////@delegate.Body = delegateBody;

            ////////var callOriginal = new MethodCall();
            ////////var toCall = TypeHelper.GetMethod(originalCall.ResolvedMethod.ContainingTypeDefinition, originalCall);

            ////////callOriginal.MethodToCall = toCall;
            ////////callOriginal.Type = toCall.Type;

            ////////var genericFunction = new GenericTypeInstanceReference();
            ////////genericFunction.GenericType = functionWithNoArgs;
            ////////genericFunction.GenericArguments.Add(toCall.Type);


            ////////foreach (var parameter in @delegate.Parameters)
            ////////{
            ////////    var parameterValue = new BoundExpression();
            ////////    parameterValue.Definition = parameter;
            ////////    parameterValue.Type = parameter.Type;

            ////////    callOriginal.Arguments.Add(parameterValue);

            ////////    genericFunction.GenericType = functionWithOneArg;
            ////////    genericFunction.GenericArguments.Add(parameter.Type);
            ////////}

            ////////@delegate.Type = genericFunction;

            ////////var callAnonymous = new MethodCall();
            //////////callAnonymous.MethodToCall = genericFunction;
            ////////callAnonymous.Type = toCall.Type;

            ////////var mr = new SpecializedMethodReference();
            ////////mr.ContainingType = callAnonymous.Type;
            ////////mr.Name = host.NameTable.GetNameFor("Invoke");
            ////////callAnonymous.MethodToCall = mr;
            ////////mr.CallingConvention = CallingConvention.HasThis;
            ////////foreach (var parameter in fakeMethod.Parameters)
            ////////{
            ////////    mr.Parameters.Add(parameter);

            ////////    var passedParameter = new BoundExpression();
            ////////    passedParameter.Definition = parameter;
            ////////    passedParameter.Type = parameter.Type;

            ////////    callAnonymous.Arguments.Add(passedParameter);
            ////////}

            ////////var tempReturn = codeBuilder.Declare.Variable("tempReturn").OfType(callOriginal.Type).InitializeWith(callOriginal).In(delegateBody);
            ////////codeBuilder.Return.Variable(tempReturn).OfType(callOriginal.Type).In(delegateBody);

            ////////var local = codeBuilder.Declare.Variable("originalCall").OfType(genericFunction).InitializeWith(
            ////////    callAnonymous).In(block);
            ////////var result =
            ////////    codeBuilder.Declare.Variable("resultOfAnon").OfType(local.Type).InitializeWith(@delegate).In(block);

            ////////var localDef = new BoundExpression();
            ////////localDef.Definition = local;
            ////////localDef.Type = local.Type;

            ////////callAnonymous.ThisArgument = localDef;

            ////////codeBuilder.Return.NullOrVoid().In(block);

            #endregion

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
