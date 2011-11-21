using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMock.Core.PostCompiler.Construction.Enums
{
    

    public interface IEnumAccessibilityOptions
    {
        object Public { get; }
        object Private { get; }
        object Internal { get; }
    }

    public interface IEnumTypeOptions
    {
        
    }

    public interface IEnumFlagsOptions
    {
        
    }
}
