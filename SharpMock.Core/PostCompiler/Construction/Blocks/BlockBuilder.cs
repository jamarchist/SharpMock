using System;
using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Declarations;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Blocks
{
    public class BlockBuilder : IBlockBuilder
    {
        private readonly IMetadataHost host;

        public BlockBuilder(IMetadataHost host)
        {
            this.host = host;
        }

        public IReturnStatementBuilder Return
        {
            get
            {
                return new ReturnStatementBuilder();
            }
        }

        public IDeclarationBuilder Declare
        {
            get
            {
                throw new NotImplementedException();
                //return new DeclarationBuilder(host);
            }
        }
    }
}
