namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions.SourceCode;
    using Common;
    using ReadableExpressions;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenCustomisingSourceCode
    {
        [Fact]
        public void ShouldUseACustomNamespace()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace("AgileObjects.GeneratedStuff");
                    sc.AddClass(cls => cls.AddMethod("DoAction", doNothing));
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace AgileObjects.GeneratedStuff
{
    public class GeneratedExpressionClass
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
        public void ShouldAllowANullNamespace()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace(null);
                    sc.AddClass(cls => cls.AddMethod("DoAction", doNothing));
                })
                .ToCSharpString();

            const string EXPECTED = @"
public class GeneratedExpressionClass
{
    public void DoAction()
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldAllowABlankNamespace()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace(string.Empty);
                    sc.AddClass(cls => cls.AddMethod("DoAction", doNothing));
                })
                .ToCSharpString();

            const string EXPECTED = @"
public class GeneratedExpressionClass
{
    public void DoAction()
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseACustomTypeNamespace()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .WithNamespaceOf<WhenBuildingSourceCode>()
                    .AddClass(cls => cls
                        .AddMethod("DoAction", doNothing)))
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
            expected.ShouldCompile();
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodNames()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.WithNamespaceOf(GetType());

                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddMethod("MyMethod", m =>
                        {
                            m.SetDefinition(Lambda<Action>(Default(typeof(void))));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace AgileObjects.BuildableExpressions.UnitTests
{
    public class MyClass
    {
        public void MyMethod()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodNamesAndStringSummaries()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            const string CLASS_SUMMARY = @"
This is my class!
Isn't it great?";

            const string METHOD_SUMMARY = @"
This is my method!
It's even better.";

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetSummary(CLASS_SUMMARY);

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.SetSummary(METHOD_SUMMARY);
                            m.SetDefinition(doNothing);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
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
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodNamesAndCommentSummaries()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

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
                            m.SetDefinition(doNothing);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
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
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodVisibilities()
        {
            var getIntFromLong = Lambda<Func<long, int>>(Constant(2), Parameter(typeof(long), "lng"));
            var getIntFromDate = Lambda<Func<DateTime, int>>(Constant(3), Parameter(typeof(DateTime), "date"));

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
                            m.SetDefinition(getIntFromLong);
                        });

                        cls.AddMethod("GetInt", m =>
                        {
                            m.SetVisibility(MemberVisibility.Private);
                            m.SetDefinition(getIntFromDate);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
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
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseStaticMethodScope()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("GetString", m =>
                        {
                            m.SetStatic();
                            m.SetBody(Default(typeof(string)));
                        });

                        cls.AddMethod("GetInt", m =>
                        {
                            m.SetBody(Default(typeof(int)));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public static string GetString()
        {
            return null;
        }

        public int GetInt()
        {
            return default(int);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseStaticClassAndMethodScopes()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetStatic();

                        cls.AddMethod("GetString", Default(typeof(string)));

                        cls.AddMethod("GetInt", Default(typeof(int)), m =>
                        {
                            m.SetStatic();
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public static class GeneratedExpressionClass
    {
        public static string GetString()
        {
            return null;
        }

        public static int GetInt()
        {
            return default(int);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAbstractClass()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetAbstract();
                        cls.AddMethod("GetString", Default(typeof(string)));
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class GeneratedExpressionClass
    {
        public string GetString()
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}