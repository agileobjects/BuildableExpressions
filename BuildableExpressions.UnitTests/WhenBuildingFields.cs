namespace AgileObjects.BuildableExpressions.UnitTests
{
    using Common;
    using Xunit;
    using static BuildableExpressions.SourceCode.MemberVisibility;

    public class WhenBuildingFields
    {
        [Fact]
        public void ShouldBuildAPublicInstanceReadWriteClassField()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddField<string>("MyField");
                    });
                })
                .ToSourceCodeString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string MyField;
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAPrivateStaticReadWriteStructField()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct(str =>
                    {
                        str.AddField<int>("MyField", f =>
                        {
                            f.SetVisibility(Private);
                            f.SetStatic();
                        });
                    });
                })
                .ToSourceCodeString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public struct GeneratedExpressionStruct
    {
        private static int MyField;
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildProtectedInstanceReadonlyAndConstantClassFields()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddField<string>("_myReadonlyStringField", f =>
                        {
                            f.SetVisibility(Protected);
                            f.SetReadonly();
                            f.SetInitialValue("hello!");
                        });

                        cls.AddField<int>("_myConstantIntField", f =>
                        {
                            f.SetVisibility(Protected);
                            f.SetConstant();
                            f.SetInitialValue(123);
                        });
                    });
                })
                .ToSourceCodeString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        protected readonly string _myReadonlyStringField = ""hello!"";
        protected const int _myConstantIntField = 123;
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }
    }
}