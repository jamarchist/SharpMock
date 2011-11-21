using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.PostCompiler.Construction.Assemblies;

namespace ConstructionTests
{
    [TestFixture]
    public class EnumConstructionTests : BaseConstructionTests
    {
        private void InAssembly(VoidAction<ITypeOptions> create)
        {
            AssemblyBuilder.CreateNewDll(with =>
            {
                with.Name(AssemblyName);
                create(with.Type);
            });
        }

        [Test]
        public void CanCreateEnum()
        {
              
        }
    }
}
