namespace IDisposableAnalyzers.Test.IDISP001DisposeCreatedTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    // ReSharper disable once UnusedTypeParameter
    public partial class ValidCode<T>
    {
        [Test]
        public void IgnoresRecursiveCalculatedProperty()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public IDisposable RecursiveProperty => RecursiveProperty;

        public void Meh()
        {
            var item = RecursiveProperty;

            using(var meh = RecursiveProperty)
            {
            }

            using(RecursiveProperty)
            {
            }
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, testCode);
        }

        [Test]
        public void IgnoresRecursiveGetSetProperty()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public sealed class Foo
    {
        public Foo()
        {
            var temp1 = this.Bar;
            this.Bar = new Disposable();
            var temp2 = this.Bar;
        }

        public IDisposable Bar
        {
            get { return this.Bar; }
            set { this.Bar = value; }
        }

        public void Meh()
        {
            var temp3 = this.Bar;
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, DisposableCode, testCode);
        }

        [Test]
        public void MethodStatementBody()
        {
            var testCode = @"
    using System;

    public static class Foo
    {
        public static void Bar()
        {
            var disposable = Forever();
            Forever();
            using(var item = Forever())
            {
            }

            using(Forever())
            {
            }
        }

        private static IDisposable Forever()
        {
            return Forever();
        }
    }";
            AnalyzerAssert.Valid(Analyzer, testCode);
        }

        [Test]
        public void MethodExpressionBody()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public IDisposable Forever() => Forever();

        public void Meh()
        {
            var meh = Forever();
            Forever();

            using(var item = Forever())
            {
            }

            using(Forever())
            {
            }
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, testCode);
        }

        [Test]
        public void WithOptionalParameter()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Collections.Generic;

    public abstract class Foo
    {
        public Foo(IDisposable disposable)
        {
            var value = disposable;
            value = WithOptionalParameter(value);
        }

        private static IDisposable WithOptionalParameter(IDisposable value, IEnumerable<IDisposable> values = null)
        {
            if (values == null)
            {
                return WithOptionalParameter(value, new[] { value });
            }

            return value;
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, testCode);
        }
    }
}
