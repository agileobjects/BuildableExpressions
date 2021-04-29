namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using BuildableExpressions;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenGeneratingSourceCodeUsings : TestClassBase
    {
        [Fact]
        public void ShouldIncludeAUsingFromADefaultExpression()
        {
            var getDefaultDate = Lambda<Func<DateTime>>(Default(typeof(DateTime)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetDateTime", getDefaultDate)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public DateTime GetDateTime()
        {
            return default(DateTime);
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromATypeOfExpression()
        {
            var getDefaultDate = Lambda<Func<Type>>(Constant(typeof(Stream)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetStreamType", getDefaultDate)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;
using System.IO;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public Type GetStreamType()
        {
            return typeof(Stream);
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAnObjectNewing()
        {
            var createStringBuilder = Lambda<Func<object>>(New(typeof(StringBuilder)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("MakeStringBuilder", createStringBuilder)))
                .ToCSharpString();

            const string EXPECTED = @"
using System.Text;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object MakeStringBuilder()
        {
            return new StringBuilder();
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAnArrayNewing()
        {
            var createStreamArray = Lambda<Func<object>>(
                NewArrayBounds(typeof(Stream), Constant(5)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetStreamObject", createStreamArray)))
                .ToCSharpString();

            const string EXPECTED = @"
using System.IO;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetStreamObject()
        {
            return new Stream[5];
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAnEnumMember()
        {
            var getEnumIntValue = CreateLambda<object>(() => (OddNumber?)OddNumber.One);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetOddOne", getEnumIntValue)))
                .ToCSharpString();

            var expected = @$"
using {typeof(OddNumber).Namespace};

namespace GeneratedExpressionCode
{{
    public class GeneratedExpressionClass
    {{
        public object GetOddOne()
        {{
            return (OddNumber?)OddNumber.One;
        }}
    }}
}}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAGenericTypeArgument()
        {
            var comparerType = typeof(Comparer<StringBuilder>);
            var defaultComparer = Property(null, comparerType, "Default");
            var comparerNotNull = NotEqual(defaultComparer, Default(defaultComparer.Type));
            var comparerCheckLambda = Lambda<Func<bool>>(comparerNotNull);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetNotNull", comparerCheckLambda)))
                .ToCSharpString();

            const string EXPECTED = @"
using System.Collections.Generic;
using System.Text;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public bool GetNotNull()
        {
            return Comparer<StringBuilder>.Default != null;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAGenericMethodArgument()
        {
            var helperVariable = Variable(typeof(TestHelper), "helper");
            var newHelper = New(typeof(TestHelper));
            var populateHelper = Assign(helperVariable, newHelper);

            var method = typeof(TestHelper)
                .GetPublicInstanceMethod("GetTypeName")
                .MakeGenericMethod(typeof(Regex));

            var methodCall = Call(helperVariable, method);
            var lambdaBody = Block(new[] { helperVariable }, populateHelper, methodCall);
            var lambda = Lambda<Func<string>>(lambdaBody);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetTypeName", lambda)))
                .ToCSharpString();

            const string EXPECTED = @"
using System.Text.RegularExpressions;
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetTypeName()
        {
            var helper = new WhenGeneratingSourceCodeUsings.TestHelper();

            return helper.GetTypeName<Regex>();
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldNotIncludeAUsingFromAnImplicitGenericMethodArgument()
        {
            var helperVariable = Variable(typeof(TestHelper), "helper");
            var newHelper = New(typeof(TestHelper));
            var populateHelper = Assign(helperVariable, newHelper);

            var method = typeof(TestHelper)
                .GetPublicInstanceMethods("GetHashCode")
                .First(m => m.IsGenericMethod)
                .MakeGenericMethod(typeof(Regex));

            var methodCall = Call(helperVariable, method, Property(helperVariable, "Regex"));
            var lambdaBody = Block(new[] { helperVariable }, populateHelper, methodCall);
            var lambda = Lambda<Func<int>>(lambdaBody);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetHash", lambda)))
                .ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetHash()
        {
            var helper = new WhenGeneratingSourceCodeUsings.TestHelper();

            return helper.GetHashCode(helper.Regex);
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAStaticMemberAccess()
        {
            var dateTimeNow = Property(null, typeof(DateTime), nameof(DateTime.Now));
            var dateTimeTicks = Property(dateTimeNow, nameof(DateTime.Ticks));
            var getDefaultDate = Lambda<Func<long>>(dateTimeTicks);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetNowTicks", getDefaultDate)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public long GetNowTicks()
        {
            return DateTime.Now.Ticks;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeUsingsFromMethodArgumentTypes()
        {
            var stringBuilderMatchesRegex = CreateLambda(
                (Regex re, StringBuilder sb) => re.IsMatch(sb.ToString()));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("IsMatch", stringBuilderMatchesRegex)))
                .ToCSharpString();

            const string EXPECTED = @"
using System.Text;
using System.Text.RegularExpressions;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public bool IsMatch
        (
            Regex re,
            StringBuilder sb
        )
        {
            return re.IsMatch(sb.ToString());
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeUsingsFromMethodGenericArgumentTypes()
        {
            var joinListItems = CreateLambda(
                (Func<IList<string>> listFactory) => string.Join(", ", listFactory.Invoke().ToArray()));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("InvokeFactory", joinListItems)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string InvokeFactory
        (
            Func<IList<string>> listFactory
        )
        {
            return string.Join("", "", listFactory.Invoke().ToArray());
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromAnExtensionMethod()
        {
            var joinListItems = CreateLambda(
                (string[] strings) => strings.Select(int.Parse).ToList());

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetInts", joinListItems)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public List<int> GetInts
        (
            string[] strings
        )
        {
            return strings.Select(int.Parse).ToList();
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeAUsingFromACatchBlockExceptionVariable()
        {
            var tryCatch = TryCatch(
                Call(typeof(Console), "ReadLine", Type.EmptyTypes),
                Catch(Parameter(typeof(IOException), "ioEx"), Default(typeof(string))));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("ReadFromConsole", tryCatch)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;
using System.IO;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string ReadFromConsole()
        {
            try
            {
                return Console.ReadLine();
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldNotDuplicateUsings()
        {
            var stringBuilderContainsOther = CreateLambda(
                (StringBuilder sb1, StringBuilder sb2) => sb1.ToString().Contains(sb2.ToString()));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetContains", stringBuilderContainsOther)))
                .ToCSharpString();

            const string EXPECTED = @"
using System.Text;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public bool GetContains
        (
            StringBuilder sb1,
            StringBuilder sb2
        )
        {
            return sb1.ToString().Contains(sb2.ToString());
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        public class TestHelper
        {
            public Regex Regex => null;

            public string GetTypeName<T>() => typeof(T).Name;

            public int GetHashCode<T>(T obj) => obj.GetHashCode();
        }

        #endregion
    }
}
