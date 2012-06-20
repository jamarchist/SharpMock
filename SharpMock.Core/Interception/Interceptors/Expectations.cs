using System;
using System.Collections.Generic;
using System.Reflection;
using SharpMock.Core.Interception.MatchingStrategies;

namespace SharpMock.Core.Interception.Interceptors
{
    public class Expectations
    {
        private readonly MethodInfo method;
        private readonly IList<object> arguments;

        public Expectations(MethodInfo method, IList<object> arguments)
        {
            this.method = method;
            this.arguments = arguments;
            this.Assertions = new List<Delegate>();
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public IList<object> Arguments
        {
            get { return arguments; }
        }

        public IList<Delegate> Assertions { get; private set; }
        public Delegate Replacement { get; set; }
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
    }
}