using System;
using System.Threading;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
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
