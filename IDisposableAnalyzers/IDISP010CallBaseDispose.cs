namespace IDisposableAnalyzers
{
    using Microsoft.CodeAnalysis;

    internal static class IDISP010CallBaseDispose
    {
        public const string DiagnosticId = "IDISP010";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Call base.Dispose(disposing)",
            messageFormat: "Call base.Dispose({0})",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Call base.Dispose(disposing)",
            helpLinkUri: HelpLink.ForId(DiagnosticId));
    }
}
