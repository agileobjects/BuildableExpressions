namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using BuildableExpressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenExtractingBlockMethods
    {
        [Fact]
        public void ShouldExtractAMultilineIfTestBlockToAPrivateMethod()
        {
            var intVariable = Parameter(typeof(int), "input");

            var yepOrNopeBlock = Block(
                IfThen(
                    Block(
                        new[] { intVariable },
                        Assign(
                            intVariable,
                            Call(typeof(Console), "Read", Type.EmptyTypes)),
                        Condition(
                            GreaterThan(intVariable, Constant(100)),
                            Constant(false),
                            Constant(true))),
                    Constant("Yep")),
                Constant("Nope"));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetYepOrNope", yepOrNopeBlock)))
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetYepOrNope()
        {
            if (this.GetBool())
            {
                return ""Yep"";
            }

            return ""Nope"";
        }

        private bool GetBool()
        {
            var input = Console.Read();

            return (input > 100) ? false : true;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldExtractMultilineConditionalBranchesToPrivateMethods()
        {
            var intParameter1 = Parameter(typeof(int), "i");
            var intParameter2 = Parameter(typeof(int), "j");

            var conditional = Condition(
                GreaterThan(intParameter1, Constant(3)),
                Block(
                    Assign(intParameter2, Constant(2)),
                    Multiply(intParameter1, intParameter2)),
                Block(
                    Assign(intParameter2, Constant(3)),
                    Multiply(intParameter1, intParameter2)));

            var conditionalLambda = Lambda<Func<int, int, int>>(
                conditional,
                intParameter1,
                intParameter2);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetInts", conditionalLambda)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetInts
        (
            int i,
            int j
        )
        {
            return (i > 3) ? this.GetInt1(j, i) : this.GetInt2(j, i);
        }

        private int GetInt1
        (
            int j,
            int i
        )
        {
            j = 2;

            return i * j;
        }

        private int GetInt2
        (
            int j,
            int i
        )
        {
            j = 3;

            return i * j;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldExtractMultilineConditionTestOperandBlocksToPrivateMethods()
        {
            var intParameter1 = Parameter(typeof(int), "i");
            var intParameter2 = Parameter(typeof(int), "j");
            var intVariable1 = Variable(typeof(int), "k");
            var intVariable2 = Variable(typeof(int), "l");

            var yepOrNope = Condition(
                GreaterThan(
                    Block(
                        new[] { intVariable1 },
                        Assign(intVariable1, Constant(2)),
                        Multiply(intParameter1, intVariable1)),
                    Block(
                        new[] { intVariable2 },
                        Assign(intVariable2, Constant(3)),
                        Multiply(intParameter2, intVariable2))
                ),
                Constant("Yep"),
                Constant("Nope"));

            var yepOrNopeLambda = Lambda<Func<int, int, string>>(
                yepOrNope,
                intParameter1,
                intParameter2);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetYepOrNope", yepOrNopeLambda)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetYepOrNope
        (
            int i,
            int j
        )
        {
            return (this.GetInt1(i) > this.GetInt2(j)) ? ""Yep"" : ""Nope"";
        }

        private int GetInt1
        (
            int i
        )
        {
            var k = 2;

            return i * k;
        }

        private int GetInt2
        (
            int j
        )
        {
            var l = 3;

            return j * l;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldExtractNestedMultilineBlocksToPrivateMethods()
        {
            var parameterI = Parameter(typeof(int), "i");
            var variableJ = Variable(typeof(int), "j");
            var variableK = Variable(typeof(int), "k");
            var variableL = Variable(typeof(int), "l");

            var assignNestedBlockResult = Block(
                new[] { variableJ },
                Assign(
                    variableJ,
                    Block(
                        new[] { variableK },
                        Assign(variableK, Constant(2)),
                        Multiply(variableK, Block(
                            new[] { variableL },
                            Assign(variableL, Constant(3)),
                            Multiply(parameterI, variableL))))
                    ),
                variableJ);

            var assignmentLambda = Lambda<Func<int, int>>(assignNestedBlockResult, parameterI);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetInt", assignmentLambda)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int GetInt
        (
            int i
        )
        {
            var j = this.GetInt2(i);

            return j;
        }

        private int GetInt2
        (
            int i
        )
        {
            var k = 2;

            return k * this.GetInt3(i);
        }

        private int GetInt3
        (
            int i
        )
        {
            var l = 3;

            return i * l;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldExtractSingleLineVariableAssignmentValueBlockToPrivateMethod()
        {
            var objectParam = Parameter(typeof(PublicProperty<int>), "obj");
            var stringParam = Parameter(typeof(string), "str");
            var intOutputVariable = Variable(typeof(int), "i");

            var tryParseResultAssignment = Assign(
                Property(objectParam, "Value"),
                Block(
                    new[] { intOutputVariable },
                    Condition(
                        Call(typeof(int).GetPublicStaticMethod(
                            "TryParse",
                            typeof(string),
                            intOutputVariable.Type.MakeByRefType()),
                            stringParam,
                            intOutputVariable),
                        intOutputVariable,
                        Constant(0))));

            var tryParseResultLambda = Lambda<Action<PublicProperty<int>, string>>(
                tryParseResultAssignment,
                objectParam,
                stringParam);

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("SetIntValue", m =>
                        {
                            m.SetDefinition(tryParseResultLambda);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void SetIntValue
        (
            WhenExtractingBlockMethods.PublicProperty<int> obj,
            string str
        )
        {
            obj.Value = this.GetInt(str);
        }

        private int GetInt
        (
            string str
        )
        {
            int i;
            return int.TryParse(str, out i) ? i : 0;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldExtractAMultilineIfTestBlockToAStaticPrivateMethod()
        {
            var intVariable = Parameter(typeof(int), "input");

            var yepOrNopeBlock = Block(
                IfThen(
                    Block(
                        new[] { intVariable },
                        Assign(
                            intVariable,
                            Call(typeof(Console), "Read", Type.EmptyTypes)),
                        Condition(
                            GreaterThan(intVariable, Constant(100)),
                            Constant(false),
                            Constant(true))),
                    Constant("Yep")),
                Constant("Nope"));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("GetYepOrNope", m =>
                        {
                            m.SetStatic();
                            m.SetBody(yepOrNopeBlock);
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
        public static string GetYepOrNope()
        {
            if (GeneratedExpressionClass.GetBool())
            {
                return ""Yep"";
            }

            return ""Nope"";
        }

        private static bool GetBool()
        {
            var input = Console.Read();

            return (input > 100) ? false : true;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldNotDuplicateMethodExtractsWhenTransientClassCreated()
        {
            var intVariable = Parameter(typeof(int), "input");

            var yepOrNopeBlock = Block(
                IfThen(
                    Block(
                        new[] { intVariable },
                        Assign(
                            intVariable,
                            Call(typeof(Console), "Read", Type.EmptyTypes)),
                        Condition(
                            GreaterThan(intVariable, Constant(100)),
                            Constant(false),
                            Constant(true))),
                    Constant("Yep")),
                Constant("Nope"));

            var transientClassType = default(Type);

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var testClass = sc.AddClass("TestClass", cls =>
                    {
                        cls.AddMethod("GetYepOrNope", yepOrNopeBlock);
                    });

                    transientClassType = testClass.Type;
                })
                .ToCSharpString();

            transientClassType.ShouldNotBeNull();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class TestClass
    {
        public string GetYepOrNope()
        {
            if (this.GetBool())
            {
                return ""Yep"";
            }

            return ""Nope"";
        }

        private bool GetBool()
        {
            var input = Console.Read();

            return (input > 100) ? false : true;
        }
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldNotDuplicateMethodExtractsWhenTransientClassFieldCreated()
        {
            var intVariable = Parameter(typeof(int), "input");

            var yepOrNopeBlock = Block(
                IfThen(
                    Block(
                        new[] { intVariable },
                        Assign(
                            intVariable,
                            Call(typeof(Console), "Read", Type.EmptyTypes)),
                        Condition(
                            GreaterThan(intVariable, Constant(100)),
                            Constant(false),
                            Constant(true))),
                    Constant("Yep")),
                Constant("Nope"));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var testClass = sc.AddClass("TestClass", cls =>
                    {
                        cls.AddMethod("GetYepOrNope", yepOrNopeBlock);
                    });

                    sc.AddClass("WrapperClass", cls =>
                    {
                        cls.AddField(
                            "_" + testClass.GetVariableNameInCamelCase(),
                            testClass,
                            field =>
                            {
                                field.SetVisibility(MemberVisibility.Private);
                                field.SetStatic();
                                field.SetReadonly();
                                field.SetInitialValue(New(testClass.Type));
                            });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class TestClass
    {
        public string GetYepOrNope()
        {
            if (this.GetBool())
            {
                return ""Yep"";
            }

            return ""Nope"";
        }

        private bool GetBool()
        {
            var input = Console.Read();

            return (input > 100) ? false : true;
        }
    }

    public class WrapperClass
    {
        private static readonly TestClass _testClass = new TestClass();
    }
}";
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        public class PublicProperty<T>
        {
            public T Value { get; set; }
        }

        #endregion
    }
}
