using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class StaticMethodCallAnalyzer : BaseCodeTraverser
    {
        public override void Visit(IAnonymousDelegate anonymousDelegate)
        {
            Print(anonymousDelegate);
            base.Visit(anonymousDelegate);
        }

        public override void Visit(IAssembly assembly)
        {
            Print(assembly);
            base.Visit(assembly);
        }

        public override void Visit(IBlockExpression blockExpression)
        {
            Print(blockExpression);
            base.Visit(blockExpression);
        }

        public override void Visit(IBlockStatement block)
        {
            Print(block);
            base.Visit(block);
        }

        public override void Visit(IConditionalStatement conditionalStatement)
        {
            Print(conditionalStatement);
            base.Visit(conditionalStatement);
        }

        public override void Visit(IBoundExpression boundExpression)
        {
            Print(boundExpression);
            base.Visit(boundExpression);
        }

        public override void Visit(IConditional conditional)
        {
            Print(conditional);
            base.Visit(conditional);
        }

        public override void Visit(IAssignment assignment)
        {
            Print(assignment);
            base.Visit(assignment);
        }

        public override void Visit(ICompileTimeConstant constant)
        {
            Print(constant);
            base.Visit(constant);
        }

        public override void Visit(IConversion conversion)
        {
            Print(conversion);
            base.Visit(conversion);
        }

        public override void Visit(ICreateDelegateInstance createDelegateInstance)
        {
            Print(createDelegateInstance);
            base.Visit(createDelegateInstance);
        }

        public override void Visit(ICreateObjectInstance createObjectInstance)
        {
            Print(createObjectInstance);
            base.Visit(createObjectInstance);
        }

        public override void Visit(IEmptyStatement emptyStatement)
        {
            Print(emptyStatement);
            base.Visit(emptyStatement);
        }

        public override void Visit(IEnumerable<IExpression> expressions)
        {
            PrintEnumerable(expressions);
            base.Visit(expressions);
        }

        public override void Visit(IEnumerable<IAssignment> assignments)
        {
            PrintEnumerable(assignments);
            base.Visit(assignments);
        }

        public override void Visit(IEnumerable<IGenericMethodParameter> genericParameters)
        {
            PrintEnumerable(genericParameters);
            base.Visit(genericParameters);
        }

        public override void Visit(IEnumerable<IGenericTypeParameter> genericParameters)
        {
            PrintEnumerable(genericParameters);
            base.Visit(genericParameters);
        }

        public override void Visit(IEnumerable<ILocalDefinition> localDefinitions)
        {
            PrintEnumerable(localDefinitions);
            base.Visit(localDefinitions);
        }

        private static void PrintEnumerable<T>(IEnumerable<T> enumerable)
        {
            Print(enumerable);
            foreach (var element in enumerable)
            {
                Print("  ", element);
            }
        }
 
        private static void Print(object codeElement)
        {
            Console.WriteLine(codeElement.GetType().Name);
        }

        private static void Print(string indentation, object codeElement)
        {
            Console.WriteLine("{0}{1}", indentation, codeElement.GetType().Name);
        }
    }
}
