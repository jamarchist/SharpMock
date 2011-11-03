using System.Collections.Generic;
using System.IO;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    public interface IClassAccessiblityOptions 
    {
        IClassModifierOptions Public { get; }
        IClassModifierOptions Private { get; }
        IClassModifierOptions Internal { get; }
    }

    public interface IClassModifierOptions
    {
        IClassBuilder Static { get; }
        IClassBuilder Abstract { get; }
        IClassBuilder Concrete { get; }
    }

    public interface IClassBuilder
    {
        IClassBuilder Named(string className);
        IClassBuilder With(VoidAction<IMethodAccessibilityOptions> method);
    }

    public interface IMethodAccessibilityOptions
    {
        IMethodModifierOptions Public { get; }
        IMethodModifierOptions Private { get; }
        IMethodModifierOptions Internal { get; }
        IMethodModifierOptions Protected { get; }
        IMethodModifierOptions ProtectedInternal { get; }
    }

    public interface IMethodModifierOptions
    {
        IMethodBuilder Static { get; }
        IMethodBuilder Virtual { get; }
        IMethodBuilder Sealed { get; }
        //IMethodBuilder Override { get; }
    }

    public interface IMethodBuilder
    {
        IMethodBuilder Named(string methodName);
        IMethodBuilder WithParameters();
        IMethodBuilder WithBody();
    }

    public interface ITypeOptions
    {
        IClassAccessiblityOptions Class { get; }
    }

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
                    newMethod.Parameters = new List<IParameterDefinition>();
                    newMethod.Visibility = TypeMemberVisibility.Public;
                    newMethod.Type = host.PlatformType.SystemVoid;

                    var methodBody = new SourceMethodBody(host, null);
                    methodBody.MethodDefinition = newMethod;
                    methodBody.LocalsAreZeroed = true;

                    var block = new BlockStatement();
                    var returnStatement = new ReturnStatement();
                    // "Stack must be empty on return from a void method"
                    //returnStatement.Expression = new CompileTimeConstant();
                    block.Statements.Add(returnStatement);

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

        private class AssemblyConfiguration
        {
            public AssemblyConfiguration()
            {
                Classes = new List<ClassConfiguration>();
            }

            public string Name { get; set; }
            public IList<ClassConfiguration> Classes { get; private set; }
        }

        private class ClassConfiguration
        {
            public ClassConfiguration()
            {
                Methods = new List<MethodConfiguration>();
            }

            public bool IsStatic { get; set; }
            public bool IsAbstract { get; set; }
            public string Modifier { get; set; }
            public string Name { get; set; }
            public IList<MethodConfiguration> Methods { get; private set; } 
        }

        private class TypeOptions : ITypeOptions
        {
            private readonly ClassConfiguration config;

            public TypeOptions(ClassConfiguration config)
            {
                this.config = config;
            }

            public IClassAccessiblityOptions Class
            {
                get { return new ClassAccessibilityOptions(config); }
            }
        }

        private class ClassModifierOptions : IClassModifierOptions
        {
            private readonly ClassConfiguration config;

            public ClassModifierOptions(ClassConfiguration config)
            {
                this.config = config;
            }

            public IClassBuilder Static
            {
                get
                {
                    config.IsStatic = true;
                    return Options();
                }
            }

            public IClassBuilder Abstract
            {
                get
                {
                    config.IsAbstract = true;
                    return Options();
                }
            }

            public IClassBuilder Concrete
            {
                get { return Options(); }
            }

            private IClassBuilder Options()
            {
                return new ClassBuilder(config);
            }
        }

        private class ClassAccessibilityOptions : IClassAccessiblityOptions
        {
            private readonly ClassConfiguration config;

            public ClassAccessibilityOptions(ClassConfiguration config)
            {
                this.config = config;
            }

            public IClassModifierOptions Public
            {
                get
                {
                    config.Modifier = "Public";
                    return Options();
                }
            }

            public IClassModifierOptions Private
            {
                get
                {
                    config.Modifier = "Private";
                    return Options();
                }
            }

            public IClassModifierOptions Internal
            {
                get
                {
                    config.Modifier = "Internal";
                    return Options();
                }
            }

            private IClassModifierOptions Options()
            {
                return new ClassModifierOptions(config);
            }
        }

        private class ClassBuilder : IClassBuilder
        {
            private readonly ClassConfiguration config;

            public ClassBuilder(ClassConfiguration config)
            {
                this.config = config;
            }

            public IClassBuilder Named(string className)
            {
                config.Name = className;
                return this;
            }

            public IClassBuilder With(VoidAction<IMethodAccessibilityOptions> method)
            {
                var newMethod = new MethodConfiguration();
                config.Methods.Add(newMethod);
                method(new MethodAcessibilityOptions(newMethod));
                return this;
            }
        }

        private class MethodConfiguration
        {
            public string Modifier { get; set; }
            public bool IsStatic { get; set; }
            public string Name { get; set; }
        }

        private class MethodAcessibilityOptions : IMethodAccessibilityOptions
        {
            private readonly MethodConfiguration config;

            public MethodAcessibilityOptions(MethodConfiguration config)
            {
                this.config = config;
            }

            public IMethodModifierOptions Public
            {
                get
                {
                    config.Modifier = "Public";
                    return new MethodModifierOptions(config);
                }
            }

            public IMethodModifierOptions Private
            {
                get { throw new System.NotImplementedException(); }
            }

            public IMethodModifierOptions Internal
            {
                get { throw new System.NotImplementedException(); }
            }

            public IMethodModifierOptions Protected
            {
                get { throw new System.NotImplementedException(); }
            }

            public IMethodModifierOptions ProtectedInternal
            {
                get { throw new System.NotImplementedException(); }
            }
        }

        private class MethodModifierOptions : IMethodModifierOptions
        {
            private readonly MethodConfiguration config;

            public MethodModifierOptions(MethodConfiguration config)
            {
                this.config = config;
            }

            public IMethodBuilder Static
            {
                get
                {
                    config.IsStatic = true;
                    return new MethodBuilder(config);
                }
            }

            public IMethodBuilder Virtual
            {
                get { throw new System.NotImplementedException(); }
            }

            public IMethodBuilder Sealed
            {
                get { throw new System.NotImplementedException(); }
            }
        }

        private class MethodBuilder : IMethodBuilder
        {
            private readonly MethodConfiguration config;

            public MethodBuilder(MethodConfiguration config)
            {
                this.config = config;
            }

            public IMethodBuilder Named(string methodName)
            {
                config.Name = methodName;
                return this;
            }

            public IMethodBuilder WithParameters()
            {
                throw new System.NotImplementedException();
            }

            public IMethodBuilder WithBody()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}