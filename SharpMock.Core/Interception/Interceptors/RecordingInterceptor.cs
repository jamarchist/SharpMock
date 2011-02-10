﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RecordingInterceptor : IInterceptor
    {
        private Expectations expectiations;

        public bool ShouldIntercept(MethodInfo method, IList<object> arguments)
        {
            expectiations = new Expectations(method, arguments);
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            // Do nothing
        }

        public Expectations GetExpectations()
        {
            return expectiations;
        }
    }

    public class Expectations
    {
        private readonly MethodInfo method;
        private readonly IList<object> arguments;

        public Expectations(MethodInfo method, IList<object> arguments)
        {
            this.method = method;
            this.arguments = arguments;
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public IList<object> Arguments
        {
            get { return arguments; }
        }
    }
}
