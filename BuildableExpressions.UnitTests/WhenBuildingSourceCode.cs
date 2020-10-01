namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions.SourceCode;
    using Common;
    using ReadableExpressions;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCode
    {
        [Fact]
        public void ShouldBuildFromAnEmptyParameterlessLambdaAction()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespace("GeneratedStuffs.Yo")
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedStuffs.Yo
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
        public void ShouldBuildANamedClassAndMethod()
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
        public void ShouldBuildANamedInternalClass()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithNamespaceOf(GetType())
                    .WithClass(cls => cls
                        .WithSummary(ReadableExpression.Comment("Class summary!"))
                        .WithVisibility(ClassVisibility.Internal)
                        .Named("MyClass")
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace AgileObjects.BuildableExpressions.UnitTests
{
    /// <summary>
    /// Class summary!
    /// </summary>
    internal class MyClass
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
        public void ShouldBuildNamedClassesAndMethodsWithVisibilities()
        {
            var getIntFromString = Lambda<Func<string, int>>(Constant(1), Parameter(typeof(string), "str"));
            var getIntFromLong = Lambda<Func<long, int>>(Constant(2), Parameter(typeof(long), "lng"));
            var getIntFromDate = Lambda<Func<DateTime, int>>(Constant(3), Parameter(typeof(DateTime), "date"));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
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
    public class GeneratedExpressionClass
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
        public void ShouldBuildNamedClassesAndMethodsWithSummaries()
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
        public void ShouldBuildNamedClassesAndMethodsWithCommentSummaries()
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
        public void ShouldBuildMultipleClassesAndMethods()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));
            var getBlah = Lambda<Func<string>>(Constant("Blah"));
            var getOne = Lambda<Func<int>>(Constant(1));
            var getTen = Lambda<Func<int>>(Constant(10));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing, m => m.Named("DoNothing"))
                        .WithMethod(getBlah))
                    .WithClass(cls => cls
                        .WithMethod(getOne)
                        .WithMethod(getTen)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass1
    {
        public void DoNothing()
        {
        }

        public string GetString()
        {
            return ""Blah"";
        }
    }

    public class GeneratedExpressionClass2
    {
        public int GetInt1()
        {
            return 1;
        }

        public int GetInt2()
        {
            return 10;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnImplementationClassAndMethod()
        {
            var sayHello = Lambda<Func<string>>(Constant("Hello!"));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .Implementing<IMessager>()
                        .WithMethod(sayHello)))
                .ToSourceCode();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class Messager : WhenBuildingSourceCode.IMessager
    {
        public string GetMessage()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassWithMultipleImplementationMethods()
        {
            var sayHello = Lambda<Func<string>>(Constant("Hello!"));
            var return123 = Lambda<Func<int>>(Constant(123));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .Implementing(typeof(IMessager), typeof(INumberSource))
                        .WithMethod(sayHello)
                        .WithMethod(return123)))
                .ToSourceCode();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass : WhenBuildingSourceCode.IMessager, WhenBuildingSourceCode.INumberSource
    {
        public string GetMessage()
        {
            return ""Hello!"";
        }

        public int GetNumber()
        {
            return 123;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        public interface IMessager
        {
            string GetMessage();
        }

        public interface INumberSource
        {
            int GetNumber();
        }

        #endregion
    }
}
