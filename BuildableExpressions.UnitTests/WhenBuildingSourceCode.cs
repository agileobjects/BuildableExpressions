namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using BuildableExpressions;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCode : TestClassBase
    {
        [Fact]
        public void ShouldBuildAMethodFromAParameterlessLambdaAction()
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
        public void ShouldBuildAMethodFromADefaultVoidExpression()
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
        public void ShouldBuildAMethodFromAParameterlessLambdaFunc()
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
        public void ShouldBuildAMethodFromSingleParameterLambdaFunc()
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
        public void ShouldBuildAMethodFromATwoParameterLambdaAction()
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

        [Fact]
        public void ShouldBuildAnEmptyClass()
        {
            SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls.Named("EmptyClass")))
                .ToSourceCode();
        }

        [Fact]
        public void ShouldBuildAGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();
            var param = BuildableExpression.GenericParameter();
            var paramName = BuildableExpression.NameOf(param);

            @class.AddMethod(paramName, m => m
                .WithGenericParameter(param));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetString<T>()
        {
            return nameof(T);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMultipleGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();
            var param1 = BuildableExpression.GenericParameter();
            var param2 = BuildableExpression.GenericParameter(gp => gp.Named("TParam2"));
            var param3 = BuildableExpression.GenericParameter();
            var param1Name = BuildableExpression.NameOf(param1);
            var param2Name = BuildableExpression.NameOf(param2);
            var param3Name = BuildableExpression.NameOf(param3);

            var concatMethod = typeof(string).GetPublicStaticMethod(
                nameof(string.Concat),
                typeof(string),
                typeof(string),
                typeof(string));

            var nameConcatCall = Call(
                concatMethod,
                param1Name,
                param2Name,
                param3Name);

            @class.AddMethod(nameConcatCall, m => m
                .WithGenericParameters(param1, param2, param3));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetString<T1, TParam2, T3>()
        {
            return nameof(T1) + nameof(TParam2) + nameof(T3);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildANamedGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();
            var param = BuildableExpression.GenericParameter(gp => gp.Named("TParam"));
            var paramType = BuildableExpression.TypeOf(param);

            @class.AddMethod(paramType, m => m
                .WithGenericParameter(param));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public Type GetType<TParam>()
        {
            return typeof(TParam);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructConstrainedGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();

            var param = BuildableExpression.GenericParameter(gp => gp
                .WithStructConstraint());

            @class.AddMethod(Default(typeof(object)), m => m
                .WithGenericParameter(param));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<T>()
            where T : struct
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildANewableClassConstrainedGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();

            var param = BuildableExpression.GenericParameter(gp => gp
                .WithClassConstraint()
                .WithNewableConstraint());

            @class.AddMethod(Default(typeof(object)), m => m
                .WithGenericParameter(param));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<T>()
            where T : class, new()
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructInterfaceConstrainedGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();

            var param = BuildableExpression.GenericParameter(gp => gp
                .WithStructConstraint()
                .WithTypeConstraint<IDisposable>());

            @class.AddMethod(Default(typeof(object)), m => m
                .WithGenericParameter(param));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<T>()
            where T : struct, IDisposable
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassAndInterfaceConstrainedGenericParameterMethod()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();

            var param = BuildableExpression.GenericParameter(gp => gp
                .WithTypeConstraints(typeof(Stream), typeof(INumberSource), typeof(IMessager)));

            @class.AddMethod(Default(typeof(object)), m => m
                .WithGenericParameter(param));

            var translated = sourceCode.ToSourceCode();

            const string EXPECTED = @"
using System.IO;
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<T>()
            where T : Stream, WhenBuildingSourceCode.INumberSource, WhenBuildingSourceCode.IMessager
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildADefaultGenericParameterValueMethod()
        {
            var param = BuildableExpression.GenericParameter();
            var paramDefault = BuildableExpression.Default(param);

            var translated = SourceCodeFactory.Default.CreateSourceCode(sc => sc
                .WithClass(cls => cls
                    .WithMethod(paramDefault, m => m
                        .WithGenericParameter(param))))
                .ToSourceCode();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public T GetT<T>()
        {
            return default(T);
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
