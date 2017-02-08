using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Microsoft.CodeAnalysis.Editing;

namespace CodeRefactoringExcercise
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(CodeRefactoringExcerciseCodeRefactoringProvider)), Shared]
    internal class CodeRefactoringExcerciseCodeRefactoringProvider : CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            var node = syntaxRoot.FindNode(context.Span);
            var catchClauseSyntax = node.FirstAncestorOrSelf<CatchClauseSyntax>();
            if(catchClauseSyntax == null)
            {
                return;
            }

            var classDeclaration = node.FirstAncestorOrSelf<ClassDeclarationSyntax>();
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

            var inheritsFromBaseViewModel = classSymbol.InheritsFrom("BaseViewModel");

            if(inheritsFromBaseViewModel)
            {
                var action = CodeAction.Create("Show reportable error to user", ct => AddMethodCall(context.Document, syntaxRoot, catchClauseSyntax, ct));

                context.RegisterRefactoring(action);
            }
        }

        private Task<Document> AddMethodCall(Document document, SyntaxNode root, CatchClauseSyntax catchClause, CancellationToken ct)
        {
            var variableName = catchClause.Declaration.Identifier.Text;
            var rewriter = new CatchClauseRewriter(variableName);
            var newCatchClause = catchClause.Accept(rewriter);
            var newRoot = root.ReplaceNode(catchClause, newCatchClause);

            var newDocument = document.WithSyntaxRoot(newRoot);

            return Microsoft.CodeAnalysis.Formatting.Formatter.FormatAsync(newDocument, cancellationToken: ct);
        }
    }
}