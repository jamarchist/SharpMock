using System;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableAssemblyInfo
    {
        public string AssemblyPath { get; set; }
        public string Name { get; set; }
    }
}