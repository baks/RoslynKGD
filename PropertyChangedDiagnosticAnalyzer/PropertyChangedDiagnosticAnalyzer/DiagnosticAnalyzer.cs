using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Semantics;

namespace PropertyChangedDiagnosticAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PropertyChangedDiagnosticAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "PropertyChangedDiagnosticAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Refactoring";
        private const string ProperyChangedEventArgsName = "PropertyChangedEventArgs";
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterOperationAction(AnalyzeObjectCreationOperation, OperationKind.ObjectCreationExpression);
            //context.RegisterSyntaxNodeAction(AnalyzeObjectCreationExpression, SyntaxKind.ObjectCreationExpression);
        }

        private void AnalyzeObjectCreationOperation(OperationAnalysisContext context)
        {
            var objectCreationOperation = (IObjectCreationExpression)context.Operation;
            var isPropertyChangedEventArgs = string.Equals(ProperyChangedEventArgsName, objectCreationOperation.Type.Name, StringComparison.Ordinal);

            if (isPropertyChangedEventArgs && !((ObjectCreationExpressionSyntax)objectCreationOperation.Syntax).IsPartOfStaticMemberDeclaration())
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreationOperation.Syntax.GetLocation()));
            }
        }

        private void AnalyzeObjectCreationExpression(SyntaxNodeAnalysisContext context)
        {
            var objectCreationExpressionNode = (ObjectCreationExpressionSyntax)context.Node;
            var typeName = objectCreationExpressionNode.Type.Accept(new TypeNameExtractor());

            var isPropertyChangedEventArgs = string.Equals(ProperyChangedEventArgsName, typeName.ValueText, StringComparison.Ordinal);

            if(isPropertyChangedEventArgs && !objectCreationExpressionNode.IsPartOfStaticMemberDeclaration())
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, objectCreationExpressionNode.GetLocation()));
            }
        }
    }
}
