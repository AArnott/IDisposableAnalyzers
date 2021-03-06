namespace IDisposableAnalyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class AsyncAwait
    {
        internal static bool TryGetAwaitedInvocation(AwaitExpressionSyntax awaitExpression, SemanticModel semanticModel, CancellationToken cancellationToken, out InvocationExpressionSyntax result)
        {
            result = null;
            if (awaitExpression?.Expression == null)
            {
                return false;
            }

            if (TryPeelConfigureAwait(awaitExpression.Expression as InvocationExpressionSyntax, semanticModel, cancellationToken, out result))
            {
                return result != null;
            }

            result = awaitExpression.Expression as InvocationExpressionSyntax;
            return result != null;
        }

        internal static bool TryAwaitTaskFromResult(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax result)
        {
            switch (expression)
            {
                case InvocationExpressionSyntax invocation:
                    return TryAwaitTaskFromResult(invocation, semanticModel, cancellationToken, out result);
                case AwaitExpressionSyntax awaitExpression:
                    return TryAwaitTaskFromResult(awaitExpression.Expression, semanticModel, cancellationToken, out result);
            }

            result = null;
            return false;
        }

        internal static bool TryAwaitTaskFromResult(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax result)
        {
            result = null;
            if (TryPeelConfigureAwait(invocation, semanticModel, cancellationToken, out InvocationExpressionSyntax inner))
            {
                invocation = inner;
            }

            if (invocation?.ArgumentList == null ||
                invocation.ArgumentList.Arguments.Count == 0)
            {
                return false;
            }

            var symbol = semanticModel.GetSymbolSafe(invocation, cancellationToken);
            if (symbol == KnownSymbol.Task.FromResult)
            {
                result = invocation.ArgumentList.Arguments[0].Expression;
            }

            return result != null;
        }

        internal static bool TryAwaitTaskRun(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax result)
        {
            switch (expression)
            {
                case InvocationExpressionSyntax invocation:
                    return TryAwaitTaskRun(invocation, semanticModel, cancellationToken, out result);
                case AwaitExpressionSyntax awaitExpression:
                    return TryAwaitTaskRun(awaitExpression.Expression, semanticModel, cancellationToken, out result);
            }

            result = null;
            return false;
        }

        internal static bool TryAwaitTaskRun(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out ExpressionSyntax result)
        {
            result = null;
            if (TryPeelConfigureAwait(invocation, semanticModel, cancellationToken, out InvocationExpressionSyntax inner))
            {
                invocation = inner;
            }

            if (invocation?.ArgumentList == null ||
                invocation.ArgumentList.Arguments.Count == 0)
            {
                return false;
            }

            var symbol = semanticModel.GetSymbolSafe(invocation, cancellationToken);
            if (symbol == KnownSymbol.Task.Run)
            {
                result = invocation.ArgumentList.Arguments[0].Expression;
            }

            return result != null;
        }

        internal static bool TryPeelConfigureAwait(InvocationExpressionSyntax invocation, SemanticModel semanticModel, CancellationToken cancellationToken, out InvocationExpressionSyntax result)
        {
            result = null;
            var method = semanticModel.GetSymbolSafe(invocation, cancellationToken);
            if (method?.Name == "ConfigureAwait")
            {
                result = invocation?.Expression as InvocationExpressionSyntax;
                if (result != null)
                {
                    return true;
                }

                var memberAccess = invocation?.Expression as MemberAccessExpressionSyntax;
                while (memberAccess != null)
                {
                    result = memberAccess.Expression as InvocationExpressionSyntax;
                    if (result != null)
                    {
                        return true;
                    }

                    memberAccess = memberAccess.Expression as MemberAccessExpressionSyntax;
                }
            }

            return false;
        }
    }
}
