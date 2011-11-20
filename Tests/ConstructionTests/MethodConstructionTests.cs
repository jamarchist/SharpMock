using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using NUnit.Framework;
using ScenarioDependencies;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace ConstructionTests
{
    [TestFixture]
    public class MethodConstructionTests : BaseConstructionTests
    {
        private IModule Assembly { get; set; }

        private void InStaticClass(VoidAction<IMethodAccessibilityOptions> createMethod)
        {
            Assembly = AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                with.ReferenceTo.Assembly("SharpMock.Core.dll");
                with.ReferenceTo.Assembly("ScenarioDependencies.dll");
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
        public void CanCreateMethodThatDeclaresVoidAction()
        {
            InStaticClass(createMethod => createMethod.Public.Static
                .Named("TestMethod")
                .WithBody(code =>
                {
                    code.AddLine(x => x.Declare.Variable<VoidAction>("local_0").As(x.Anon.Of<VoidAction>().WithBody(
                        anon =>
                            {
                                anon.AddLine(z => z.Do(z.Call.StaticMethod("VoidReturnNoParameters").ThatReturnsVoid().WithNoArguments().On(typeof (StaticClass))));
                                anon.AddLine(z => z.Return.Void());
                            })));
                    code.AddLine(x => x.Return.Void());
                })
            );

            var testMethod = GetMethodFromTestClass("TestMethod");
            Assert.IsNotNull(testMethod);
        }

        [Test]
        public void CanCreateActionSimilarToReplacementAction()
        {
            InStaticClass(createMethod => createMethod.Public.Static
                .Named("VoidReturnNoParameters")
                .WithBody(code =>
                {
                    code.AddLine(x => x.Declare.Variable<RegistryInterceptor>("interceptor").As(x.Create.New<RegistryInterceptor>()));
                    code.AddLine(x => x.Declare.Variable<Invocation>("invocation").As(x.Create.New<Invocation>()));
                    code.AddLine(x => x.Declare.Variable<Type>("interceptedType").As(x.Operators.TypeOf(x.Reflector.Get(typeof(StaticClass)))));
                    code.AddLine(x => x.Declare.Variable<Type[]>("parameterTypes").As(x.Create.NewArray<Type>(0)));
                    code.AddLine(x => x.Locals.Array<Type>("parameterTypes")[0].Assign("interceptedType"));
                    code.AddLine(x => x.Declare.Variable<MethodInfo>("interceptedMethod").As(
                        x.Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                            .ThatReturns<MethodInfo>()
                            .WithArguments(x.Constant.Of<string>("VoidReturnNoParameters"), x.Locals["parameterTypes"])
                            .On("interceptedType")));
                    code.AddLine(x =>
                        x.Declare.Variable<VoidAction>("anonymous").As(
                        x.Anon.Of<VoidAction>()
                            .WithBody(anonCode =>
                            {
                                anonCode.AddLine(z => z.Do(
                                    z.Call.StaticMethod("VoidReturnNoParameters").ThatReturnsVoid().WithNoArguments().On(typeof(StaticClass))));
                                anonCode.AddLine(z => z.Return.Void());
                            })));
                    code.AddLine(x => x.Declare.Variable<List<object>>("arguments").As(x.Create.New<List<object>>()));
                    code.AddLine(x => x.Do(x.Call.PropertySetter<Delegate>("OriginalCall").WithArguments("anonymous").On("invocation")));
                    code.AddLine(x => x.Do(x.Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation")));
                    code.AddLine(x => x.Do(x.Call.PropertySetter<object>("Target").WithArguments(x.Constant.Of<object>(null)).On("invocation")));
                    code.AddLine(x => x.Declare.Variable<bool>("shouldIntercept").As(x.Call.VirtualMethod("ShouldIntercept", typeof(MethodInfo), typeof(IList<object>)).ThatReturns<bool>().WithArguments("interceptedMethod", "arguments").On("interceptor")));
                    code.AddLine(x => x.Do(x.Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor")));
                    code.AddLine(x => x.Return.Void());
                }));

            var testMethod = GetMethodFromTestClass("VoidReturnNoParameters");
            Assert.IsNotNull(testMethod);
        }

        [Test]
        public void CanCreateFunctionSimilarToReplacementFunction()
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
                        x.Declare.Variable<SharpMock.Core.Function<string, bool>>("anonymous").As(
                        x.Anon.Of<SharpMock.Core.Function<string, bool>>()
                            .WithBody(anonCode =>
                            {
                                anonCode.AddLine(z => z.Declare.Variable<bool>("originalReturn").As(
                                    z.Call.StaticMethod("IsNullOrEmpty", typeof(string)).ThatReturns<bool>().WithArguments(z.Params["alteredarg"]).On<String>()));
                                anonCode.AddLine(z => z.Return.Variable(z.Locals["originalReturn"]));
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
