namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using ReadableExpressions;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCode : TestClassBase
    {
        [Fact]
        public void ShouldUseACustomFileHeaderString()
        {
            const string fileHeader = @"
This is some auto-generated source code
It was BuildableExpressions wot dun it!";

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetHeader(fileHeader.TrimStart());
                    sc.AddClass(cls => cls.AddMethod(Lambda<Action>(Empty())));
                })
                .ToCSharpString();

            const string expected = @"
// This is some auto-generated source code
// It was BuildableExpressions wot dun it!

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void DoAction()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseACustomFileHeaderComment()
        {
            var fileHeader = ReadableExpression.Comment(@"
This is some auto-generated source code.
It was BuildableExpressions wot dun it");

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetHeader(fileHeader);
                    sc.AddClass(cls => cls.AddMethod(Lambda<Action>(Empty())));
                })
                .ToCSharpString();

            const string expected = @"
// This is some auto-generated source code.
// It was BuildableExpressions wot dun it

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void DoAction()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseACustomNamespace()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace("AgileObjects.GeneratedStuff");

                    var doNothing = Lambda<Action>(Empty());
                    sc.AddClass(cls => cls.AddMethod("DoAction", doNothing));
                })
                .ToCSharpString();

            const string expected = @"
namespace AgileObjects.GeneratedStuff
{
    public class GeneratedExpressionClass
    {
        public void DoAction()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldAllowANullNamespace()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace(null);

                    var doNothing = Lambda<Action>(Empty());
                    sc.AddClass(cls => cls.AddMethod("DoAction", doNothing));
                })
                .ToCSharpString();

            const string expected = @"
public class GeneratedExpressionClass
{
    public void DoAction()
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldAllowABlankNamespace()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace(string.Empty);

                    var doNothing = Lambda<Action>(Empty());
                    sc.AddClass(cls => cls.AddMethod("DoAction", doNothing));
                })
                .ToCSharpString();

            const string expected = @"
public class GeneratedExpressionClass
{
    public void DoAction()
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseACustomTypeNamespace()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespaceToThatOf<WhenBuildingSourceCode>();

                    sc.AddClass(cls =>
                    {
                        var doNothing = Lambda<Action>(Empty());
                        cls.AddMethod("DoAction", doNothing);
                    });
                })
                .ToCSharpString();

            var expected = @$"
namespace {typeof(WhenBuildingSourceCode).Namespace}
{{
    public class GeneratedExpressionClass
    {{
        public void DoAction()
        {{
        }}
    }}
}}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodNamesAndStringSummaries()
        {
            const string classSummary = @"
This is my class!
Isn't it great?";

            const string methodSummary = @"
This is my method!
It's even better.";

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetSummary(classSummary);

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.SetSummary(methodSummary);

                            var doNothing = Lambda<Action>(Empty());
                            m.SetDefinition(doNothing);
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    /// <summary>
    /// This is my class!
    /// Isn't it great?
    /// </summary>
    public class MyClass
    {
        /// <summary>
        /// This is my method!
        /// It's even better.
        /// </summary>
        public void MyMethod()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodNamesAndCommentSummaries()
        {
            var classSummary = ReadableExpression.Comment(@"
This is my class!
Isn't it great?");

            var methodSummary = ReadableExpression.Comment(@"
This is my method!
It's even better.".TrimStart());

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetSummary(classSummary);

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.SetSummary(methodSummary);

                            var doNothing = Lambda<Action>(Empty());
                            m.SetDefinition(doNothing);
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    /// <summary>
    /// This is my class!
    /// Isn't it great?
    /// </summary>
    public class MyClass
    {
        /// <summary>
        /// This is my method!
        /// It's even better.
        /// </summary>
        public void MyMethod()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodVisibilities()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetVisibility(TypeVisibility.Internal);

                        cls.AddMethod("GetInt", m =>
                        {
                            m.SetVisibility(MemberVisibility.Internal);
                            m.AddParameter(Parameter(typeof(string), "str"));
                            m.SetBody(Constant(1));
                        });

                        cls.AddMethod("GetInt", m =>
                        {
                            m.SetVisibility(MemberVisibility.Protected);

                            m.SetDefinition(Lambda<Func<long, int>>(
                                Constant(2),
                                Parameter(typeof(long), "lng")));
                        });

                        cls.AddMethod("GetInt", m =>
                        {
                            m.SetVisibility(MemberVisibility.Private);

                            m.SetDefinition(Lambda<Func<DateTime, int>>(
                                Constant(3),
                                Parameter(typeof(DateTime), "date")));
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    internal class GeneratedExpressionClass
    {
        internal int GetInt
        (
            string str
        )
        {
            return 1;
        }

        protected int GetInt
        (
            long lng
        )
        {
            return 2;
        }

        private int GetInt
        (
            DateTime date
        )
        {
            return 3;
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldPlaceTypesFirstInConstraintsList()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetDerived", m =>
                    {
                        m.AddGenericParameter("TDerived", gp =>
                        {
                            gp.AddTypeConstraints(typeof(IMarker1), typeof(BaseType));
                        });

                        m.SetBody(Default(typeof(object)));
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string expected = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetDerived<TDerived>()
            where TDerived : WhenBuildingSourceCode.BaseType, WhenBuildingSourceCode.IMarker1
        {
            return null;
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        #region Helper Members

        public class BaseType { }

        public interface IMarker1 { }

        #endregion
    }
}
