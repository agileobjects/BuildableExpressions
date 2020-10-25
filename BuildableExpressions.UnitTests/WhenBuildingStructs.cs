namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Linq.Expressions;
    using Common;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingStructs
    {
        [Fact]
        public void ShouldBuildAStruct()
        {
            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddStruct("MyStruct", cls => cls
                        .AddMethod(Expression.Default(typeof(void)))))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct MyStruct
    {
        public void DoAction()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceAndImplementingStruct()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var @interface = sc.AddInterface("IDoesNothing", itf =>
                    {
                        itf.AddMethod("DoNothing", typeof(void));
                    });

                    sc.AddStruct("StructImpl", cls =>
                    {
                        cls.SetImplements(@interface);

                        cls.AddMethod("DoNothing", m =>
                        {
                            m.SetBody(Default(typeof(void)));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public interface IDoesNothing
    {
        void DoNothing();
    }

    public struct StructImpl : IDoesNothing
    {
        public void DoNothing()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnEmptyStruct()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("EmptyStruct", str => { });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct EmptyStruct { }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericParameterStruct()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddStruct("GenericStruct", str =>
                {
                    var param = str.AddGenericParameter("T");
                    var paramType = BuildableExpression.TypeOf(param);

                    str.AddMethod("GetParamType", m =>
                    {
                        m.SetBody(paramType);
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public struct GenericStruct<T>
    {
        public Type GetParamType()
        {
            return typeof(T);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}