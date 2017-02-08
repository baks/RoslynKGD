using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PropertyChangedDiagnosticAnalyzer
{
    public class TypeNameExtractor : CSharpSyntaxVisitor<SyntaxToken>
    {
        public override SyntaxToken VisitIdentifierName(IdentifierNameSyntax node)
        {
            return node.Identifier;
        }

        public override SyntaxToken VisitQualifiedName(QualifiedNameSyntax node)
        {
            return VisitIdentifierName((IdentifierNameSyntax)node.Right);
        }
    }
}
