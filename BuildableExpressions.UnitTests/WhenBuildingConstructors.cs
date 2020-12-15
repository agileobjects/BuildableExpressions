namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
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
                            f.SetReadonly();
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
        private readonly string _name;

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

        [Fact]
        public void ShouldBuildAChainedStructConstructorCall()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("LongWrapper", str =>
                    {
                        var valueProperty =
                            str.AddProperty<long>("Value", p => p.SetGetter());

                        var longParamCtor = str.AddConstructor(ctor =>
                        {
                            var longValuePropertyAccess = Property(
                                str.ThisInstanceExpression,
                                valueProperty.PropertyInfo);

                            var longParam = Parameter(typeof(long), "value");
                            ctor.AddParameter(longParam);

                            var longValueAssignment = Assign(longValuePropertyAccess, longParam);
                            ctor.SetBody(longValueAssignment);
                        });

                        str.AddConstructor(ctor =>
                        {
                            var intParam = Parameter(typeof(int), "intValue");
                            ctor.AddParameter(intParam);

                            ctor.SetConstructorCall(longParamCtor, Convert(intParam, typeof(long)));

                            ctor.SetBody(Empty());
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct LongWrapper
    {
        public LongWrapper
        (
            long value
        )
        {
            this.Value = value;
        }

        public LongWrapper
        (
            int intValue
        )
        : this((long)intValue)
        {
        }

        public long Value { get; private set; }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAChainedBaseClassConstructorCall()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var personClass = sc.AddClass("PersonBase", cls =>
                    {
                        cls.SetAbstract();

                        var nameProperty =
                            cls.AddProperty<string>("Name", p => p.SetGetter());

                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetVisibility(Protected);

                            var namePropertyAccess = Property(
                                cls.ThisInstanceExpression,
                                nameProperty.PropertyInfo);

                            var nameParam = ctor.AddParameter<string>("name");
                            var nameAssignment = Assign(namePropertyAccess, nameParam);
                            ctor.SetBody(nameAssignment);
                        });
                    });

                    sc.AddClass("Customer", cls =>
                    {
                        cls.SetBaseType(personClass);

                        var numberProperty =
                            cls.AddProperty<string>("Number", p => p.SetGetter());

                        cls.AddConstructor(ctor =>
                        {
                            var personCtor = personClass.ConstructorExpressions.First();
                            var nameParam = ctor.AddParameter<string>("customerName");
                            ctor.SetConstructorCall(personCtor, nameParam);

                            var numberPropertyAccess = Property(
                                cls.ThisInstanceExpression,
                                numberProperty.PropertyInfo);

                            var numberParam = ctor.AddParameter<string>("customerNumber");
                            var numberAssignment = Assign(numberPropertyAccess, numberParam);
                            ctor.SetBody(numberAssignment);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class PersonBase
    {
        protected PersonBase
        (
            string name
        )
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }

    public class Customer : PersonBase
    {
        public Customer
        (
            string customerName,
            string customerNumber
        )
        : base(customerName)
        {
            this.Number = customerNumber;
        }

        public string Number { get; private set; }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }
    }
}