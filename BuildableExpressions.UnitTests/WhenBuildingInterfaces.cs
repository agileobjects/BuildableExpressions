namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingInterfaces
    {
        [Fact]
        public void ShouldBuildAnEmptyInterface()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IMarker", itf => { });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public interface IMarker
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceWithAMethod()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("ILogger", itf =>
                    {
                        itf.AddMethod("Log", typeof(void), m =>
                        {
                            m.AddParameter(Parameter(typeof(Exception), "ex"));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public interface ILogger
    {
        void Log
        (
            Exception ex
        );
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceImplementingAnInterfaceType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IDisposableMarker", itf =>
                    {
                        itf.SetImplements<IDisposable>();
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public interface IDisposableMarker : IDisposable
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceImplementingAPartClosedGenericInterfaceType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IStringDictionary", itf =>
                    {
                        itf.AddGenericParameter("TValue");

                        itf.SetImplements(typeof(IDictionary<,>), impl =>
                        {
                            impl.SetGenericArgument<string>("TKey");
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System.Collections.Generic;

namespace GeneratedExpressionCode
{
    public interface IStringDictionary<TValue> : IDictionary<string, TValue>
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}
