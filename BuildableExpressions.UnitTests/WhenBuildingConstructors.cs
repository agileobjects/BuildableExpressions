namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;
    using static BuildableExpressions.SourceCode.MemberVisibility;

    public class WhenBuildingConstructors : TestClassBase
    {
        [Fact]
        public void ShouldBuildAnEmptyPublicClassConstructor()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetBody(Empty());
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public GeneratedExpressionClass()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildASimpleInternalClassConstructor()
        {
            var writeLineLambda = CreateLambda(() => Console.WriteLine("Constructing!"));

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetVisibility(Internal);
                            ctor.SetBody(writeLineLambda);
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
        internal GeneratedExpressionClass()
        {
            Console.WriteLine(""Constructing!"");
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAPropertyAssignmentStructConstructor()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("CountPair", str =>
                    {
                        var count1Property = str.AddProperty<int>("Count1", p => p.SetGetter());
                        var count2Property = str.AddProperty<int>("Count2", p => p.SetGetter());

                        str.AddConstructor(ctor =>
                        {
                            var count1Param = Parameter(typeof(int), "one");
                            var count2Param = Parameter(typeof(int), "two");

                            var count1Assignment = Assign(
                                Property(
                                    str.ThisInstanceExpression,
                                    count1Property.PropertyInfo),
                                count1Param);

                            var count2Assignment = Assign(
                                Property(
                                    str.ThisInstanceExpression,
                                    count2Property.PropertyInfo),
                                count2Param);

                            ctor.AddParameters(count1Param, count2Param);

                            ctor.SetBody(Block(new Expression[]
                            {
                                count1Assignment,
                                count2Assignment
                            }));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct CountPair
    {
        public CountPair
        (
            int one,
            int two
        )
        {
            this.Count1 = one;
            this.Count2 = two;
        }

        public int Count1 { get; private set; }

        public int Count2 { get; private set; }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAbstractFieldAssignmentClassConstructor()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("PersonBase", cls =>
                    {
                        cls.SetAbstract();

                        var nameField = cls.AddField<string>("_name", f =>
                        {
                            f.SetVisibility(Private);
                        });

                        var nameFieldAccess =
                            Field(cls.ThisInstanceExpression, nameField.FieldInfo);

                        cls.AddProperty<string>("Name", p =>
                        {
                            p.SetGetter(g => g.SetBody(nameFieldAccess));
                        });

                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetVisibility(Protected);

                            var nameParam = Parameter(typeof(string), "name");
                            ctor.AddParameter(nameParam);

                            var nameAssignment = Assign(nameFieldAccess, nameParam);
                            ctor.SetBody(nameAssignment);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class PersonBase
    {
        private string _name;

        protected PersonBase
        (
            string name
        )
        {
            this._name = name;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}