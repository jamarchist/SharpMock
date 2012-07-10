using System;
using System.Collections.Generic;
using System.Reflection;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.MatchingStrategies;

namespace SharpMock.Core.Interception.Interceptors
{
    public class Expectations
    {
        private readonly MemberInfo method;
        private readonly IList<object> arguments;

        public Expectations(MemberInfo method, IList<object> arguments)
        {
            this.method = method;
            this.arguments = arguments;
            this.Assertions = new List<Delegate>();
        }

        public MemberInfo Method
        {
            get { return method; }
        }

        public IList<object> Arguments
        {
            get { return arguments; }
        }

        public IList<Delegate> Assertions { get; private set; }
        public Delegate Replacement { get; set; }
        public object[] OutAndRefParameters { get; set; }
        private IMatchingStrategy matchingStrategy;
        public IMatchingStrategy MatchingStrategy
        {
            get
            {
                if (matchingStrategy == null)
                {
                    matchingStrategy = new EquivalentCallsMatch(Method);
                }

                return matchingStrategy;
            }

            set { matchingStrategy = value; }
        }

        private IInterceptionStrategy invoker;
        public IInterceptionStrategy Invoker
        {
            get
            {
                if (invoker == null)
                {
                    invoker = new InsteadOfCall(() => Replacement);
                }

                return invoker;
            }

            set { invoker = value; }
        }
    }
}