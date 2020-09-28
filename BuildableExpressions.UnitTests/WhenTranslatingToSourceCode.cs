namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenTranslatingToSourceCode : TestClassBase
    {
        [Fact]
        public void ShouldTranslateAnEmptyParameterlessLambdaActionToASourceCodeMethod()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
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
        public void ShouldTranslateADefaultVoidExpressionToASourceCodeMethod()
        {
            var doNothing = Default(typeof(void));

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
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
        public void ShouldTranslateAParameterlessLambdaFuncToASourceCodeMethod()
        {
            var returnOneThousand = CreateLambda(() => 1000);

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(returnOneThousand)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetInt()
        {
            return 1000;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldTranslateASingleParameterLambdaFuncToASourceCodeMethod()
        {
            var returnGivenLong = CreateLambda((long arg) => arg);

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(returnGivenLong)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public long GetLong
        (
            long arg
        )
        {
            return arg;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldTranslateATwoParameterLambdaActionToASourceCodeMethod()
        {
            var subtractShortFromInt = CreateLambda((int value1, short value2) => value1 - value2);

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(subtractShortFromInt)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetInt
        (
            int value1,
            short value2
        )
        {
            return value1 - ((int)value2);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldIncludeNonScopeVariablesAsMethodParameters()
        {
            var int1Variable = Parameter(typeof(int), "int1");
            var int2Variable = Variable(typeof(int), "int2");
            var addInts = Add(int1Variable, int2Variable);

            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            sourceCode.AddClass(cls => cls.WithMethod(addInts));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetInt
        (
            int int1,
            int int2
        )
        {
            return int1 + int2;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldRecogniseBlockVariablesAsInScope()
        {
            var intParameter = Parameter(typeof(int), "scopedInt");
            var intVariable = Variable(typeof(int), "blockScopedInt");
            var assignBlockInt = Assign(intVariable, Constant(1));
            var addInts = Add(intVariable, intParameter);
            var block = Block(new[] { intVariable }, assignBlockInt, addInts);
            var addIntsLambda = Lambda<Func<int, int>>(block, intParameter);

            var translated = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(addIntsLambda)))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetInt
        (
            int scopedInt
        )
        {
            var blockScopedInt = 1;

            return blockScopedInt + scopedInt;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

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
                    .WithNamespaceOf<WhenTranslatingToSourceCode>()
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                .ToSourceCode();

            var expected = @$"
namespace {typeof(WhenTranslatingToSourceCode).Namespace}
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
                .NameClassesUsing((sc, ctx) => $"My{ctx.TypeName}Class_{ctx.Index}"));

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
        public void ShouldUseACustomClassAndMethodName()
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
    }
}
