using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallRegistrar : BaseCodeTraverser
    {
        private readonly IUnitReflector reflector;

        public StaticMethodCallRegistrar(IMetadataHost host)
        {
            reflector = new UnitReflector(host);
        }

        public override void Visit(IMethodCall methodCall)
        {
            var matchers = new CompositeReplacementMatcher(
                //new RegisteredMethodMatcher()
                new StaticMethodMatcher(),
                new MethodInSealedClassMatcher()
                //new ConstructorMatcher(),
                //new DelegateConstructorMatcher()
            );

            //new RegisteredMethodMatcher().ShouldReplace(methodCall);

            //var methodAttributes = new List<ICustomAttribute>(methodCall.MethodToCall.Attributes) ?? new List<ICustomAttribute>();
            if (matchers.ShouldReplace(methodCall))
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(methodCall.MethodToCall);
            }

            base.Visit(methodCall);
        }

        private interface IReplacementMatcher
        {
            bool ShouldReplace(IMethodCall methodCall);
        }

        private class CompositeReplacementMatcher : IReplacementMatcher
        {
            private readonly IReplacementMatcher[] matchers;

            public CompositeReplacementMatcher(params IReplacementMatcher[] matchers)
            {
                this.matchers = matchers;
            }

            public bool ShouldReplace(IMethodCall methodCall)
            {
                foreach (var matcher in matchers)
                {
                    if (matcher.ShouldReplace(methodCall))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private class StaticMethodMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return methodCall.IsStaticCall;
            }
        }

        private class MethodInSealedClassMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return methodCall.ThisArgument.Type.ResolvedType.IsSealed;
            }
        }

        private class ConstructorMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return !methodCall.MethodToCall.ResolvedMethod.IsConstructor;
            }
        }

        private class DelegateConstructorMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return methodCall.MethodToCall.ResolvedMethod.ContainingTypeDefinition.IsDelegate &&
                       new ConstructorMatcher().ShouldReplace(methodCall);
            }
        }

        private class RegisteredMethodMatcher : IReplacementMatcher
        {
            public bool ShouldReplace(IMethodCall methodCall)
            {
                return MethodReferenceReplacementRegistry.HasReplacementFor(methodCall.MethodToCall);
            }
        }
    }
}