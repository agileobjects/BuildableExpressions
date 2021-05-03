namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using BuildableExpressions.SourceCode;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingConstructorsIncorrectly : TestClassBase
    {
        [Fact]
        public void ShouldErrorIfConstructorHasNullParameter()
        {
            var ctorParamEx = Should.Throw<ArgumentNullException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter(null);
                        });
                    });
                });
            });

            ctorParamEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfConstructorHasNullParameterName()
        {
            var ctorParamEx = Should.Throw<ArgumentNullException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>(null);
                        });
                    });
                });
            });

            ctorParamEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfConstructorHasBlankParameterName()
        {
            var ctorParamEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>(string.Empty);
                        });
                    });
                });
            });

            ctorParamEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfConstructorHasWhitespaceParameterName()
        {
            var ctorParamEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("\t");
                        });
                    });
                });
            });

            ctorParamEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfConstructorChainedToUnrelatedConstructor()
        {
            var ctorCallEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var class1 = sc.AddClass("Class1", cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("name");
                            ctor.SetBody(Empty());
                        });
                    });

                    sc.AddClass("Class2", cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetConstructorCall(
                                class1.ConstructorExpressions.First(),
                                Constant("Nope"));
                        });
                    });
                });
            });

            ctorCallEx.Message.ShouldContain("Constructor Class2()");
            ctorCallEx.Message.ShouldContain("cannot call");
            ctorCallEx.Message.ShouldContain("constructor Class1(string)");
        }

        [Fact]
        public void ShouldErrorIfConstructorChainedToPrivateBaseClassConstructor()
        {
            var ctorCallEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var baseClass = sc.AddClass("BaseClass", cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetVisibility(MemberVisibility.Private);
                            ctor.AddParameter<string>("name");
                            ctor.SetBody(Empty());
                        });
                    });

                    sc.AddClass("DerivedClass", cls =>
                    {
                        cls.SetBaseType(baseClass);

                        cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("name");

                            ctor.SetConstructorCall(
                                baseClass.ConstructorExpressions.First(),
                                Constant("Nope"));
                        });
                    });
                });
            });

            ctorCallEx.Message.ShouldContain("Constructor DerivedClass(string)");
            ctorCallEx.Message.ShouldContain("cannot call");
            ctorCallEx.Message.ShouldContain("private constructor BaseClass(string)");
        }

        [Fact]
        public void ShouldErrorIfIncorrectChainedConstructorArgumentCount()
        {
            var ctorArgsEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("Class1", cls =>
                    {
                        var nameCtor = cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("name");
                            ctor.SetBody(Empty());
                        });

                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetConstructorCall(nameCtor);
                        });
                    });
                });
            });

            ctorArgsEx.Message.ShouldContain("Constructor Class1(string)");
            ctorArgsEx.Message.ShouldContain("1 parameter(s)");
            ctorArgsEx.Message.ShouldContain("0 were supplied");
        }

        [Fact]
        public void ShouldErrorIfIncorrectChainedConstructorArgumentType()
        {
            var ctorArgsEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("Entity", cls =>
                    {
                        var nameCtor = cls.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<int>("id");
                            ctor.AddParameter<string>("name");
                            ctor.SetBody(Empty());
                        });

                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetConstructorCall(
                                nameCtor,
                                Constant(123),
                                Constant(DateTime.Now));
                        });
                    });
                });
            });

            ctorArgsEx.Message.ShouldContain("Constructor Entity(int, string)");
            ctorArgsEx.Message.ShouldContain("cannot be called");
            ctorArgsEx.Message.ShouldContain("argument(s) of Type int, DateTime");
        }
    }
}