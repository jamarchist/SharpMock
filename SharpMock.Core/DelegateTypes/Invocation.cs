﻿using System;
using System.Collections.Generic;

namespace SharpMock.Core.DelegateTypes
{
    public class Invocation : IInvocation
    {
        public Invocation()
        {
            Arguments = new List<object>();
        }

        public IList<object> Arguments { get; set; }
        public object Return { get; set; }
        public object Target { get; set; }
        public Delegate OriginalCall { get; set; }
    }
}
