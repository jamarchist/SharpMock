using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace DecompilerHelper
{
    public class CodePrinter : BaseCodeTraverser
    {
        private readonly StringBuilder output = new StringBuilder();
        private int indentation = 0;

        public override void Visit(IAssembly assembly)
        {
            AppendElementType(assembly);
            output.Append(assembly.Name.Value);

            Visit(assembly.GetAllTypes());
            System.Console.WriteLine(output);
        }

        public override void Visit(IEnumerable<INamedTypeDefinition> types)
        {
            foreach (var type in types)
            {
                NewLine();
                Indent(1);

                AppendElementType(type);
                output.Append(type.Name.Value);
                AppendSpace();

                Visit(type);
            }
        }

        public override void Visit(IMethodDefinition method)
        {
            NewLine();
            Indent(2);

            AppendElementType(method);
            output.Append(method.Name.Value);
            AppendSpace();

            base.Visit(method);
        }

        public override void Visit(IFieldDefinition fieldDefinition)
        {
            NewLine();
            Indent(2);

            AppendElementType(fieldDefinition);
            base.Visit(fieldDefinition);

            output.Append(fieldDefinition.Name.Value);
            AppendSpace();
        }

        public override void Visit(ILocalDeclarationStatement localDeclarationStatement)
        {
            NewLineAddIndent();
            AppendElementType(localDeclarationStatement);
            Visit(localDeclarationStatement.LocalVariable);
            //output.Append("=");
            //AppendSpace();
            Visit(localDeclarationStatement.InitialValue);
            base.Visit(localDeclarationStatement);
        }

        public override void Visit(ILocalDefinition localDefinition)
        {
            NewLineAddIndent();
            AppendElementType(localDefinition);
            //output.Append(localDefinition.Name.Value);
            //AppendSpace();
            base.Visit(localDefinition);
        }

        public override void Visit(IStatement statement)
        {
            NewLine();
            Indent(3);
            AppendElementType(statement);
            base.Visit(statement);
        }

        public override void Visit(IExpression expression)
        {
            if (expression == null)
            {
                output.Append("null");
            }
            else
            {
                AppendElementType(expression);
                base.Visit(expression);                
            }

        }

        public override void Visit(ICreateArray createArray)
        {
            NewLineAddIndent();
            AppendElementType(createArray);
            //Visit(createArray.ElementType);   
            base.Visit(createArray);
        }

        public override void Visit(IBoundExpression boundExpression)
        {
            NewLineAddIndent();
            AppendElementType(boundExpression);
            //if (boundExpression.Definition != null && boundExpression.Definition is ILocalDefinition)
            //{
            //    Visit(boundExpression.Definition as ILocalDefinition);
            //}
            base.Visit(boundExpression);
        }

        public override void Visit(ITypeReference typeReference)
        {
            if (typeReference is INamedTypeReference)
            {
                output.Append((typeReference as INamedTypeReference).Name.Value);
            }
            else if (typeReference is VectorTypeReference)
            {
                output.Append((((typeReference as VectorTypeReference).ElementType) as INamedTypeReference).Name.Value);
                output.Append("[]");
            }
            else
            {
                output.AppendFormat("?{0}?", typeReference.GetType().Name);
            }
            AppendSpace();
            base.Visit(typeReference);
        }

        public override void Visit(ICompileTimeConstant constant)
        {
            NewLineAddIndent();
            AppendElementType(constant);
            //output.Append(constant.Value);
            //AppendSpace();
            base.Visit(constant);
        }

        public override void Visit(IAssignment assignment)
        {
            NewLineAddIndent();
            AppendElementType(assignment);
            //Visit(assignment.Target);
            //output.Append("= ");
            //Visit(assignment.Source);
            base.Visit(assignment);
        }

        public override void Visit(ITargetExpression target)
        {
            NewLineAddIndent();
            AppendElementType(target);
            //Visit(target.Type);
            base.Visit(target);
        }

        public override void Visit(IAddressOf addressOf)
        {
            NewLineAddIndent();
            AppendElementType(addressOf);
            base.Visit(addressOf);
        }

        public override void Visit(IAddressableExpression addressableExpression)
        {
            NewLineAddIndent();
            AppendElementType(addressableExpression);
            //output.Append(addressableExpression.Definition);
            base.Visit(addressableExpression);
        }

        private void AppendElementType(object element)
        {
            output.AppendFormat("[{0}] ", element.GetType().Name);
        }

        private void Indent()
        {
            for (int i = 1; i <= indentation; i++)
            {
                AppendSpace();
            }
        }

        private void Indent(int numberOfSpaces)
        {
            for (int i = 1; i <= numberOfSpaces; i++)
            {
                indentation++;
                AppendSpace();
            }            
        }

        private void NewLine()
        {
            NewLine(false);
        }

        private void NewLine(bool preserveIndentation)
        {
            output.AppendLine();
            if (!preserveIndentation) indentation = 0;
        }

        private void NewLineIndented()
        {
            NewLine(true);
            Indent();
        }

        private void NewLineAddIndent()
        {
            var indent = indentation + 1;
            NewLine(false);
            Indent(indent);
        }

        private void AppendSpace()
        {
            output.Append(" ");
        }
    }
}