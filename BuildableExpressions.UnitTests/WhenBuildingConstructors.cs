namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using Xunit;
    using static BuildableExpressions.SourceCode.MemberVisibility;

    public class WhenBuildingConstructors : TestClassBase
    {
        [Fact]
        public void ShouldBuildAnEmptyPublicConstructor()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetBody(Expression.Empty());
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public GeneratedExpressionClass()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildASimpleInternalConstructor()
        {
            var writeLineLambda = CreateLambda(() => Console.WriteLine("Constructing!"));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetVisibility(Internal);
                            ctor.SetBody(writeLineLambda);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        internal GeneratedExpressionClass()
        {
            Console.WriteLine(""Constructing!"");
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}