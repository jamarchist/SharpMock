using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace ConstructionTests
{
    [TestFixture]
    public class MethodConstructionTests : BaseConstructionTests
    {
        private void InStaticClass(VoidAction<IMethodAccessibilityOptions> createMethod)
        {
            AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                with.ReferenceTo.Assembly("SharpMock.Core.dll");
                with.Type.Class.Public.Static
                    .Named("TestClass")
                    .With(method => createMethod(method));
            });
        }

        private MethodInfo GetMethodFromTestClass(string methodName)
        {
            return GetMethodFromClass("TestClass", methodName);
        }

        [Test]
        public void CanCreateStaticMethod()
        {
            InStaticClass(createMethod => createMethod.Public.Static.Named("TestMethod"));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.IsTrue(testMethod.IsStatic);
        }

        [Test]
        public void CanCreateMethodThatReturnsVoid()
        {
            InStaticClass(createMethod => createMethod.Public.Static.Named("TestMethod"));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.AreEqual(typeof(void), testMethod.ReturnType);
        }

        [Test]
        public void CanCreateEmptyStaticMethodReturningString()
        {
            InStaticClass(createMethod => createMethod.Public.Static
                .Named("TestMethod")
                .Returning<string>()
                .WithBody(code =>
                              {
                                  code.AddLine( x => x.Declare.Variable<string>("returnValue").As(x.Constant.Of("something")) );
                                  code.AddLine( x => x.Return.Variable(x.Locals["returnValue"]) );
                              }));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
            Assert.AreEqual(typeof(string), testMethod.ReturnType);
        }

        [Test]
        public void CanCreateMethodThatCallsDefaultValue()
        {
            InStaticClass(createMethod => createMethod.Public.Static
                .Named("TestMethod")
                .Returning<DateTime>()
                .WithBody(code =>
                              {
                                  code.AddLine( x => x.Declare.Variable<DateTime>("defaultDateTime").As(x.Create.Default<DateTime>()) );
                                  code.AddLine( x => x.Return.Variable( x.Locals["defaultDateTime"]) );
                              }));

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
        }

        [Test]
        public void CanCreateMethodSimilarToReplacementMethod()
        {
            InStaticClass(createMethod => createMethod.Public.Static
                .Named("IsNullOrEmpty")
                .WithParameters(new Dictionary<string, Type>{{ "arg", typeof(string) }})
                .Returning<bool>()
                .WithBody(code =>
                {
                    code.AddLine( x => x.Declare.Variable<RegistryInterceptor>("interceptor").As(x.Create.New<RegistryInterceptor>()) );
                    code.AddLine( x => x.Declare.Variable<Invocation>("invocation").As(x.Create.New<Invocation>()) );
                    code.AddLine( x => x.Declare.Variable<Type>("interceptedType").As(x.Operators.TypeOf(x.Reflector.Get<String>())) );
                    code.AddLine( x => x.Declare.Variable<Type[]>("parameterTypes").As(x.Create.NewArray<Type>(1)) );
                    code.AddLine( x => x.Locals.Array<Type>("parameterTypes")[0].Assign("interceptedType") );
                    code.AddLine( x => x.Declare.Variable<MethodInfo>("interceptedMethod").As(
                        x.Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                            .ThatReturns<MethodInfo>()
                            .WithArguments(x.Constant.Of<string>("IsNullOrEmpty"), x.Locals["parameterTypes"])
                            .On("interceptedType")));
                    code.AddLine( x => 
                        x.Declare.Variable<Function<string, bool>>("anonymous").As(
                        x.Anon.Of<Function<string, bool>>()
                            .WithBody(anonCode =>
                            {
                                anonCode.AddLine(z => z.Declare.Variable<bool>("constantReturn").As(z.Constant.Of(false)));
                                anonCode.AddLine(z => z.Return.Variable(z.Locals["constantReturn"]));
                            })));
                    code.AddLine( x => x.Declare.Variable<List<object>>("arguments").As(x.Create.New<List<object>>()) );
                    code.AddLine( x => x.Do(x.Call.VirtualMethod("Add", typeof(object))
                                                    .ThatReturnsVoid()
                                                    .WithArguments(x.ChangeType.Box(x.Params["arg"]))
                                                    .On("arguments")));
                    code.AddLine( x => x.Do(x.Call.PropertySetter<Delegate>("OriginalCall").WithArguments("anonymous").On("invocation")) );
                    code.AddLine( x => x.Do(x.Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation")) );
                    code.AddLine( x => x.Do(x.Call.PropertySetter<object>("Target").WithArguments(x.Constant.Of<object>(null)).On("invocation")) );
                    code.AddLine( x => x.Declare.Variable<bool>("shouldIntercept").As(x.Call.VirtualMethod("ShouldIntercept", typeof(MethodInfo), typeof(IList<object>)).ThatReturns<bool>().WithArguments("interceptedMethod", "arguments").On("interceptor")) );
                    code.AddLine( x => x.Do(x.Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor")) );
                    code.AddLine( x => x.Declare.Variable<bool>("interceptionResult").As(
                        x.ChangeType.Convert(x.Call.PropertyGetter<object>("Return").On("invocation")).To<bool>()) );
                    code.AddLine( x => x.Return.Variable(x.Locals["interceptionResult"]));
                }));

            var testMethod = GetMethodFromTestClass("IsNullOrEmpty");
            Assert.IsNotNull(testMethod);
        }
    }
}
