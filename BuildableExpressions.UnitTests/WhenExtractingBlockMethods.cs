namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        public void ShouldExtractAMultilineIfTestBlock()
        {
            var intVariable = Variable(typeof(int), "input");

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
                        cls.AddMethod("GetYepOrNope", yepOrNopeBlock);
                    });
                })
                .ToCSharpString();

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractAMultilineMethodArgumentBlock()
        {
            var boolListParam = Parameter(typeof(List<bool>), "bools");
            var intVariable = Variable(typeof(int), "input");

            var addBoolCall = Call(
                boolListParam,
                boolListParam.Type.GetPublicInstanceMethod("Add", typeof(bool)),
                Block(
                    new[] { intVariable },
                    Assign(
                        intVariable,
                        Call(typeof(Console), "Read", Type.EmptyTypes)),
                    Condition(
                        GreaterThan(intVariable, Constant(100)),
                        Constant(false),
                        Constant(true))));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("AddBool", Lambda<Action<List<bool>>>(addBoolCall, boolListParam));
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;
using System.Collections.Generic;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void AddBool
        (
            List<bool> bools
        )
        {
            bools.Add(this.GetBool());
        }

        private bool GetBool()
        {
            var input = Console.Read();

            return (input > 100) ? false : true;
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractMultilineConditionalBranchBlocks()
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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractMultilineConditionTestOperandBlocks()
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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractAMethodArgumentLambdaBodyBlock()
        {
            var stringListParam = Parameter(typeof(IEnumerable<string>), "strings");
            var stringParam = Parameter(typeof(string), "str");
            var intVariable = Variable(typeof(int), "intValue");

            var linqSelectMethod = typeof(Enumerable)
                .GetPublicStaticMethods("Select")
                .First(m => m.GetParameters()[1]
                    .ParameterType
                    .GetGenericTypeArguments().Length == 2)
                .MakeGenericMethod(typeof(string), typeof(int));

            var parseStringsLambda =
                Lambda<Func<IEnumerable<string>, IEnumerable<int>>>(
                    Call(
                        linqSelectMethod,
                        stringListParam,
                        Lambda<Func<string, int>>(
                            Block(
                                new[] { intVariable },
                                Condition(
                                    Call(
                                        typeof(int).GetPublicStaticMethod(
                                            "TryParse",
                                            typeof(string),
                                            typeof(int).MakeByRefType()),
                                        stringParam,
                                        intVariable),
                                    intVariable,
                                    Default(typeof(int)))),
                            stringParam)),
                    stringListParam);

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("ParseStrings", parseStringsLambda);
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System.Collections.Generic;
using System.Linq;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public IEnumerable<int> ParseStrings
        (
            IEnumerable<string> strings
        )
        {
            return strings.Select(str => this.GetInt(str));
        }

        private int GetInt
        (
            string str
        )
        {
            int intValue;
            return int.TryParse(str, out intValue) ? intValue : default(int);
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractNestedMultilineBlocks()
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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractSingleLineVariableAssignmentValueBlock()
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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractSingleLineVariableAssignmentValueTryCatch()
        {
            var inputParam = Parameter(typeof(string), "str");
            var resultVariable = Variable(typeof(int), "value");

            var tryParseResultAssignment = Block(
                new[] { resultVariable },
                Assign(
                    resultVariable,
                    TryCatch(
                        Call(
                            typeof(int).GetPublicStaticMethod("Parse", typeof(string)),
                            inputParam),
                        Catch(typeof(FormatException), Default(typeof(int))))),
                resultVariable);

            var tryParseResultLambda = Lambda<Func<string, int>>(
                tryParseResultAssignment,
                inputParam);

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("ParseInt", m =>
                        {
                            m.SetStatic();
                            m.SetDefinition(tryParseResultLambda);
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public static int ParseInt
        (
            string str
        )
        {
            var value = GeneratedExpressionClass.GetInt(str);

            return value;
        }

        private static int GetInt
        (
            string str
        )
        {
            try
            {
                return int.Parse(str);
            }
            catch (FormatException)
            {
                return default(int);
            }
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldExtractAMultilineStaticMethodIfTestBlock()
        {
            var intVariable = Variable(typeof(int), "input");

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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldNotDuplicateBlockExtractsWhenTransientClassCreated()
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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldNotDuplicateBlockExtractsWhenTransientClassFieldCreated()
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

            const string expected = @"
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
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldNotExtractAnIfStatementBodyBlock()
        {
            var intParam = Parameter(typeof(int), "intValue");
            var stringVariable1 = Variable(typeof(string), "str1");
            var stringVariable2 = Variable(typeof(string), "str2");

            var intParamToString = Call(
                intParam,
                intParam.Type.GetPublicInstanceMethod("ToString", Type.EmptyTypes));

            var ifStatementLambda = Lambda<Func<int, int>>(
                Block(
                    IfThen(
                        GreaterThan(intParam, Constant(100, typeof(int))),
                        Block(
                            new[] { stringVariable1, stringVariable2 },
                            Assign(stringVariable1, intParamToString),
                            Assign(stringVariable2, intParamToString))),
                    intParam),
                intParam);

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("TestClass", cls =>
                    {
                        cls.AddMethod("GetIntValue", ifStatementLambda);
                    });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public class TestClass
    {
        public int GetIntValue
        (
            int intValue
        )
        {
            if (intValue > 100)
            {
                var str1 = intValue.ToString();
                var str2 = intValue.ToString();
            }

            return intValue;
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        #region Helper Members

        public class PublicProperty<T>
        {
            public T Value { get; set; }
        }

        #endregion
    }
}
