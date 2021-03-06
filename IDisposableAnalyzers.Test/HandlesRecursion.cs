namespace IDisposableAnalyzers.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class HandlesRecursion
    {
        private static readonly IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers = typeof(AnalyzerCategory)
                                                                                 .Assembly
                                                                                 .GetTypes()
                                                                                 .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
                                                                                 .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                                                                                 .ToArray();

        [Test]
        public void NotEmpty()
        {
            CollectionAssert.IsNotEmpty(AllAnalyzers);
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public void ConstructorCallingSelf(DiagnosticAnalyzer analyzer)
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Constructors : IDisposable
    {
        private IDisposable disposable;

        public Constructors()
            : this()
        {
        }

        public Constructors(IDisposable disposable)
            : this(IDisposable disposable)
        {
            this.disposable = disposable;
        }

        public Constructors(int i, IDisposable disposable)
            : this(disposable, i)
        {
            this.disposable = disposable;
        }

        public Constructors(IDisposable disposable, int i)
            : this(i, disposable)
        {
            this.disposable = disposable;
        }

        public void Dispose()
        {
        }
    }
}";
            var solution = CodeFactory.CreateSolution(testCode, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);
            AnalyzerAssert.NoDiagnostics(Analyze.GetDiagnostics(analyzer, solution));
        }

        [TestCaseSource(nameof(AllAnalyzers))]
        public void ConstructorCycle(DiagnosticAnalyzer analyzer)
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Constructors : IDisposable
    {
        private IDisposable disposable;

        public Constructors(int i, IDisposable disposable)
            : this(disposable, i)
        {
            this.disposable = disposable;
        }

        public Constructors(IDisposable disposable, int i)
            : this(i, disposable)
        {
            this.disposable = disposable;
        }

        public void Dispose()
        {
        }
    }
}";
            var solution = CodeFactory.CreateSolution(testCode, CodeFactory.DefaultCompilationOptions(analyzer, AnalyzerAssert.SuppressedDiagnostics), AnalyzerAssert.MetadataReferences);
            AnalyzerAssert.NoDiagnostics(Analyze.GetDiagnostics(analyzer, solution));
        }
    }
}
