namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using BuildableExpressions.SourceCode;
    using Common;
    using ReadableExpressions;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenCustomisingSourceCode
    {
        [Fact]
        public void ShouldUseACustomNamespace()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace("AgileObjects.GeneratedStuff")
                    .WithClass(cls => cls
                        .WithMethod(doNothing))).ToSourceCode();

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

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace(null)
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

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

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace(string.Empty)
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

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

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespaceOf<WhenBuildingSourceCode>()
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

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
        public void ShouldUseACustomClassName()
        {
            var doNothing = Default(typeof(void));

            var factory = new SourceCodeFactory(s => s
                .NameClassesUsing((sc, ctx) => $"My{ctx.Methods.First().Type.Name}Class_{ctx.Index}"));

            var translated = factory
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class MyVoidClass_0
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
        public void ShouldUseACustomMethodName()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var factory = new SourceCodeFactory(s => s
                .NameMethodsUsing((sc, cls, ctx) => $"Method_{cls.Index}_{ctx.Index}"));

            var translated = factory
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void Method_0_0()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseCustomClassAndMethodNames()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespaceOf(GetType())
                    .WithClass(cls => cls
                        .Named("MyClass")
                        .WithMethod(doNothing, m => m
                            .Named("MyMethod"))))
                .ToSourceCode();

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
        public void ShouldUseCustomClassAndMethodNameFactories()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var factory = new SourceCodeFactory(s => s
                .NameClassesUsing(ctx => "MySpecialClass")
                .NameMethodsUsing((clsExp, mCtx) =>
                    $"{clsExp.Name}{mCtx.ReturnTypeName}Method_{clsExp.Index + 1}_{mCtx.Index + 1}"));

            var translated = factory
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class MySpecialClass
    {
        public void MySpecialClassVoidMethod_1_1()
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

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithSummary(CLASS_SUMMARY)
                        .Named("MyClass")
                        .WithMethod(doNothing, m => m
                            .WithSummary(METHOD_SUMMARY)
                            .Named("MyMethod"))))
                .ToSourceCode();

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

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithSummary(classSummary)
                        .Named("MyClass")
                        .WithMethod(doNothing, m => m
                            .WithSummary(methodSummary)
                            .Named("MyMethod"))))
                .ToSourceCode();

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
            var getIntFromString = Lambda<Func<string, int>>(Constant(1), Parameter(typeof(string), "str"));
            var getIntFromLong = Lambda<Func<long, int>>(Constant(2), Parameter(typeof(long), "lng"));
            var getIntFromDate = Lambda<Func<DateTime, int>>(Constant(3), Parameter(typeof(DateTime), "date"));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithVisibility(ClassVisibility.Internal)
                        .WithMethod(getIntFromString, m => m
                            .WithVisibility(MemberVisibility.Internal))
                        .WithMethod(getIntFromLong, m => m
                            .WithVisibility(MemberVisibility.Protected))
                        .WithMethod(getIntFromDate, m => m
                            .WithVisibility(MemberVisibility.Private))))
                .ToSourceCode();

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
            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(Default(typeof(string)), m => m
                            .AsStatic())
                        .WithMethod(Default(typeof(int)))))
                .ToSourceCode();

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
            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .AsStatic()
                        .WithMethod(Default(typeof(string)))
                        .WithMethod(Default(typeof(int)), m => m
                            .AsStatic())))
                .ToSourceCode();

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
    }
}