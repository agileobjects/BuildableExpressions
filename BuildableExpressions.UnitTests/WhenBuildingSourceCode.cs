namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions;
    using Common;
    using NetStandardPolyfills;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCode : TestClassBase
    {
        [Fact]
        public void ShouldBuildAMethodFromAParameterlessLambdaAction()
        {
            var doNothing = Lambda<Action>(Default(typeof(void)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod(doNothing)))
                .ToCSharpString();

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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod(doNothing)))
                .ToCSharpString();

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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("Get1000", returnOneThousand)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int Get1000()
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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetLong", returnGivenLong)))
                .ToCSharpString();

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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("Subtract", subtractShortFromInt)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int Subtract
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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("AddInts", addIntsLambda)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int AddInts
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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass("Messager", cls =>
                    {
                        cls.SetImplements<IMessager>();
                        cls.AddMethod(nameof(IMessager.GetMessage), sayHello);
                    }))
                .ToCSharpString();

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

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls =>
                    {
                        cls.SetImplements(typeof(IMessager), typeof(INumberSource));
                        cls.AddMethod(nameof(IMessager.GetMessage), sayHello);
                        cls.AddMethod(nameof(INumberSource.GetNumber), return123);
                    }))
                .ToCSharpString();

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
        public void ShouldBuildAClassWithABaseType()
        {
            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass("DerivedType", cls =>
                    {
                        cls.SetBaseType<BaseType>();
                        cls.AddMethod("SayHello", Constant("Hello!"));
                    }))
                .ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class DerivedType : WhenBuildingSourceCode.BaseType
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnEmptyClass()
        {
            BuildableExpression
                .SourceCode(sc => sc
                    .AddClass("EmptyClass", cls => { }))
                .ToCSharpString();
        }

        [Fact]
        public void ShouldBuildAValueType()
        {
            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddStruct("MyStruct", cls => cls
                        .AddMethod(Default(typeof(void)))))
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
        public void ShouldBuildAGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var param = BuildableExpression.GenericParameter("T");
                    var paramType = BuildableExpression.TypeOf(param);

                    cls.AddMethod("GetType", paramType, m => m
                        .AddGenericParameter(param));
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public Type GetType<T>()
        {
            return typeof(T);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMultipleGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var param1 = BuildableExpression.GenericParameter("T1");
                    var param2 = BuildableExpression.GenericParameter("TParam2");
                    var param3 = BuildableExpression.GenericParameter("T3");
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

                    cls.AddMethod("GetNames", nameConcatCall, m => m
                        .AddGenericParameters(param1, param2, param3));
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetNames<T1, TParam2, T3>()
        {
            return nameof(T1) + nameof(TParam2) + nameof(T3);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructConstrainedGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var param = BuildableExpression.GenericParameter("TStruct", gp => gp
                        .WithStructConstraint());

                    cls.AddMethod("GetObject", Default(typeof(object)), m => m
                        .AddGenericParameter(param));
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<TStruct>()
            where TStruct : struct
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
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var param = BuildableExpression.GenericParameter("TNewable", gp => gp
                        .WithClassConstraint()
                        .WithNewableConstraint());

                    cls.AddMethod("GetObject", Default(typeof(object)), m => m
                        .AddGenericParameter(param));
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<TNewable>()
            where TNewable : class, new()
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
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var param = BuildableExpression.GenericParameter("TMarker", gp => gp
                        .WithStructConstraint()
                        .WithTypeConstraint<IMarker1>());

                    cls.AddMethod("GetMarker", Default(typeof(object)), m => m
                        .AddGenericParameter(param));
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetMarker<TMarker>()
            where TMarker : struct, WhenBuildingSourceCode.IMarker1
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
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var param = BuildableExpression.GenericParameter("TDerived", gp => gp
                        .WithTypeConstraints(typeof(BaseType), typeof(IMarker1), typeof(IMarker2)));

                    cls.AddMethod("GetDerived", Default(typeof(object)), m => m
                        .AddGenericParameter(param));
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetDerived<TDerived>()
            where TDerived : WhenBuildingSourceCode.BaseType, WhenBuildingSourceCode.IMarker1, WhenBuildingSourceCode.IMarker2
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
            var param = BuildableExpression.GenericParameter("T");
            var paramDefault = Default(param.Type);

            var translated = BuildableExpression.SourceCode(sc => sc
                .AddClass(cls => cls
                    .AddMethod("GetT", paramDefault, m => m
                        .AddGenericParameter(param))))
                .ToCSharpString();

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

        public class BaseType { }

        public interface IMarker1 { }

        public interface IMarker2 { }

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
