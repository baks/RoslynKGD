using Microsoft.CodeAnalysis;

namespace CodeRefactoringExcercise
{
    public static class INamedTypeSymbolExtensions
    {
        public static bool InheritsFrom(this INamedTypeSymbol symbol, string name)
        {
            var result = false;
            while (symbol != null)
            {
                if (string.Equals(symbol.Name, name, System.StringComparison.Ordinal))
                {
                    result = true;
                    break;
                }
                symbol = symbol.BaseType;
            }
            return result;
        }
    }
}
