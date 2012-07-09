using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class NewObjStatementVisitor : CodeTraverser
    {
        private readonly IStatement parent;
        private readonly ILogger log;

        public NewObjStatementVisitor(IStatement parent, ILogger log)
        {
            this.parent = parent;
            this.log = log;
        }

        public override void TraverseChildren(ICreateObjectInstance createObjectInstance)
        {
            var replaceableConstructor = createObjectInstance.MethodToCall.AsReplaceable();
            log.WriteTrace("Visiting constructor of '{0}'.", replaceableConstructor.DeclaringType.Name);

            if (MethodReferenceReplacementRegistry.HasReplacementFor(replaceableConstructor))
            {
                var replacementCall =
                    MethodReferenceReplacementRegistry.GetReplacementFor(createObjectInstance.MethodToCall);

                var newCall = new MethodCall();
                newCall.Type = createObjectInstance.Type;
                newCall.Arguments = new List<IExpression>(createObjectInstance.Arguments);
                newCall.Locations = new List<ILocation>(createObjectInstance.Locations);
                newCall.MethodToCall = replacementCall;
                newCall.IsStaticCall = true;

                if (parent is LocalDeclarationStatement)
                {
                    var declaration = parent as LocalDeclarationStatement;
                    declaration.InitialValue = newCall;
                }
            }

            //base.Visit(createObjectInstance);
        }
    }
}