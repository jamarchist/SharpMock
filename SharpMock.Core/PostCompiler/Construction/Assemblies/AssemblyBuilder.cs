using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Classes;
using SharpMock.Core.PostCompiler.Construction.Methods;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    public class AssemblyBuilder : IAssemblyBuilder, IAssemblyConstructionOptions
    {
        private readonly AssemblyConfiguration model = new AssemblyConfiguration();

        private void CreateNewDll(string assemblyName)
        {
            var host = new PeReader.DefaultHost();
            var core = host.LoadAssembly(host.CoreAssemblySymbolicIdentity);

            var assembly = new Assembly();
            assembly.Name = host.NameTable.GetNameFor(assemblyName);
            assembly.ModuleName = host.NameTable.GetNameFor(assemblyName + ".dll");
            assembly.Kind = ModuleKind.DynamicallyLinkedLibrary;
            assembly.PlatformType = host.PlatformType;
            assembly.TargetRuntimeVersion = core.TargetRuntimeVersion;
            assembly.AssemblyReferences.Add(core);

            foreach (var referencePath in model.ReferencePaths)
            {
                assembly.AssemblyReferences.Add(host.LoadUnitFrom(referencePath) as IAssembly);
            }

            var root = new RootUnitNamespace();
            root.Unit = assembly;
            assembly.UnitNamespaceRoot = root;

            var module = new NamespaceTypeDefinition();
            module.Name = host.NameTable.GetNameFor("<Module>");
            module.IsClass = true;
            module.InternFactory = host.InternFactory;
            module.ContainingUnitNamespace = root;

            assembly.AllTypes.Add(module);

            var rootTypeNamespace = new NestedUnitNamespace();
            rootTypeNamespace.Name = host.NameTable.GetNameFor(assembly.Name.Value);

            root.Members.Add(rootTypeNamespace);

            foreach (var classConfiguration in model.Classes)
            {
                var newClass = new NamespaceTypeDefinition();
                newClass.IsAbstract = classConfiguration.IsAbstract;
                newClass.IsClass = true;
                newClass.BaseClasses = new List<ITypeReference>{ host.PlatformType.SystemObject };
                newClass.IsPublic = true;
                
                if (classConfiguration.IsStatic)
                {
                    newClass.IsStatic = true;
                    newClass.IsAbstract = true;
                    newClass.IsSealed = true;
                }
                
                newClass.ContainingUnitNamespace = rootTypeNamespace;
                newClass.InternFactory = host.InternFactory;
                newClass.Name = host.NameTable.GetNameFor(classConfiguration.Name);
                newClass.Methods = new List<IMethodDefinition>(classConfiguration.Methods.Count);

                foreach (var methodConfiguration in classConfiguration.Methods)
                {
                    var newMethod = new MethodDefinition();
                    newMethod.Name = host.NameTable.GetNameFor(methodConfiguration.Name);
                    newMethod.IsStatic = methodConfiguration.IsStatic;
                    newMethod.ContainingTypeDefinition = newClass;
                    newMethod.IsCil = true;
                    newMethod.InternFactory = host.InternFactory;
                    newMethod.Visibility = TypeMemberVisibility.Public;
                    newMethod.Type = host.PlatformType.SystemVoid;

                    var newMethodParameters = new List<IParameterDefinition>();
                    foreach (var param in methodConfiguration.Parameters)
                    {
                        var newMethodParameter = new ParameterDefinition();
                        newMethodParameter.ContainingSignature = newMethod;
                        newMethodParameter.Index = (ushort)methodConfiguration.Parameters.IndexOf(param);
                        newMethodParameter.Name = host.NameTable.GetNameFor(param.Key);
                        newMethodParameter.Type = new UnitReflector(host).Get(param.Value);

                        newMethodParameters.Add(newMethodParameter);
                    }

                    newMethod.Parameters = newMethodParameters;

                    var methodBody = new SourceMethodBody(host, null);
                    methodBody.MethodDefinition = newMethod;
                    methodBody.LocalsAreZeroed = true;

                    var block = new BlockStatement();
                    var returnStatement = new ReturnStatement();

                    if (methodConfiguration.ReturnType != null)
                    {
                        newMethod.Type = new UnitReflector(host).Get(methodConfiguration.ReturnType);
                        returnStatement.Expression = new CompileTimeConstant();
                    }

                    if (methodConfiguration.MethodBody != null)
                    {
                        var codeBuilder = new CodeBuilder(host, newMethod.Parameters);
                        methodConfiguration.MethodBody(codeBuilder);

                        foreach (var statement in codeBuilder.Statements)
                        {
                            block.Statements.Add(statement);
                        }
                    }

                    // "Stack must be empty on return from a void method"
                    //returnStatement.Expression = new CompileTimeConstant();
                    //block.Statements.Add(returnStatement);

                    methodBody.Block = block;

                    newMethod.Body = methodBody;

                    newClass.Methods.Add(newMethod);
                }
                
                assembly.AllTypes.Add(newClass);
            }

            using (var dll = File.Create(assemblyName + ".dll"))
            {
                PeWriter.WritePeToStream(assembly, host, dll);
            }
        }

        public void CreateNewDll(VoidAction<IAssemblyConstructionOptions> with)
        {
            with(this);
            CreateNewDll(model.Name);
        }

        public void Name(string assemblyName)
        {
            model.Name = assemblyName;
        }

        public ITypeOptions Type
        {
            get
            {
                var classConfiguration = new ClassConfiguration();
                model.Classes.Add(classConfiguration);
                return new TypeOptions(classConfiguration);
            }
        }

        public IReferenceOptions ReferenceTo
        {
            get { return new ReferenceOptions(model); }
        }
    }
}