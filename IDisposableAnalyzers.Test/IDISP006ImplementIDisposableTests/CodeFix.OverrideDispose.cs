namespace IDisposableAnalyzers.Test.IDISP006ImplementIDisposableTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public partial class CodeFix
    {
        public class OverrideDispose
        {
            private static readonly DiagnosticAnalyzer Analyzer = new FieldAndPropertyDeclarationAnalyzer();
            private static readonly CodeFixProvider Fix = new ImplementIDisposableFix();
            private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("IDISP006");

            [Test]
            public void SubclassStreamReader()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : StreamReader
    {
        ↓private readonly Stream stream = File.OpenRead(string.Empty);

        public Foo(string path)
            : base(path)
        {
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : StreamReader
    {
        private readonly Stream stream = File.OpenRead(string.Empty);
        private bool disposed;

        public Foo(string path)
            : base(path)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        protected virtual void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new System.ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
                AnalyzerAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, testCode, fixedCode);
            }

            [Test]
            public void StyleCopCallingBaseThrowIfDisposed()
            {
                var baseCode = @"
namespace RoslynSandbox
{
    using System;

    public class BaseClass : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }
        }

        protected virtual void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}";
                var testCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : BaseClass
    {
        ↓private readonly Stream stream = File.OpenRead(string.Empty);
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : BaseClass
    {
        private readonly Stream stream = File.OpenRead(string.Empty);
        private bool disposed;

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        protected override void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new System.ObjectDisposedException(this.GetType().FullName);
            }

            base.ThrowIfDisposed();
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, testCode }, fixedCode);
            }

            [Test]
            public void UnderscoreWhenThrowIsNotVirtual()
            {
                var baseCode = @"
namespace RoslynSandbox
{
    using System;

    public class BaseClass : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (disposing)
            {
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}";
                var testCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : BaseClass
    {
        ↓private readonly Stream _stream = File.OpenRead(string.Empty);
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : BaseClass
    {
        private readonly Stream _stream = File.OpenRead(string.Empty);
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, testCode }, fixedCode);
            }

            [Test]
            public void UnderscoreWhenThrowIsVirtual()
            {
                var baseCode = @"
namespace RoslynSandbox
{
    using System;

    public class BaseClass : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (disposing)
            {
            }
        }

        protected virtual void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}";
                var testCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : BaseClass
    {
        ↓private readonly Stream _stream = File.OpenRead(string.Empty);
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System.IO;

    public class Foo : BaseClass
    {
        private readonly Stream _stream = File.OpenRead(string.Empty);
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        protected override void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new System.ObjectDisposedException(GetType().FullName);
            }

            base.ThrowIfDisposed();
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { baseCode, testCode }, fixedCode);
            }

            [Test]
            public void SubclassingNinjectModule()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Ninject.Modules;

    internal class Foo : NinjectModule
    {
        ↓private readonly IDisposable disposable = new Disposable();

        public override void Load()
        {
            throw new NotImplementedException();
        }
    }
}";

                var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using Ninject.Modules;

    internal class Foo : NinjectModule
    {
        private readonly IDisposable disposable = new Disposable();
        private bool disposed;

        public override void Load()
        {
            throw new NotImplementedException();
        }

        public override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        protected virtual void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
                AnalyzerAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { DisposableCode, testCode }, fixedCode);
                AnalyzerAssert.FixAll(Analyzer, Fix, ExpectedDiagnostic, new[] { DisposableCode, testCode }, fixedCode);
            }
        }
    }
}
