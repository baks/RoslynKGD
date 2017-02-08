using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeRefactoringExcercise
{
    public class CatchClauseRewriter : CSharpSyntaxRewriter
    {
        private readonly string name;
        public CatchClauseRewriter(string name)
        {
            this.name = name;
        }

        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            var newStatement = 
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName("ShowReportableError"))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(IdentifierName(name))))))
                .NormalizeWhitespace();

            return node.AddStatements(newStatement);
        }
    }
}
