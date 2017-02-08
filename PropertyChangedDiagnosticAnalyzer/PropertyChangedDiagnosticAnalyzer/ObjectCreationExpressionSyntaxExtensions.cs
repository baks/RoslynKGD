using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PropertyChangedDiagnosticAnalyzer
{
    public static class ObjectCreationExpressionSyntaxExtensions
    {
        public static bool IsPartOfStaticMemberDeclaration(this ObjectCreationExpressionSyntax objectCreationExprSyntax)
        {
            var memberForObjectCreationExpr = objectCreationExprSyntax.FirstAncestorOrSelf<MemberDeclarationSyntax>();
            switch (memberForObjectCreationExpr.Kind())
            {
                case SyntaxKind.ConstructorDeclaration:
                    var constructorDeclaration = (ConstructorDeclarationSyntax)memberForObjectCreationExpr;
                    return constructorDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
                case SyntaxKind.FieldDeclaration:
                    var fieldDeclaration = (FieldDeclarationSyntax)memberForObjectCreationExpr;
                    return fieldDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
            }

            return false;
        }
    }
}
