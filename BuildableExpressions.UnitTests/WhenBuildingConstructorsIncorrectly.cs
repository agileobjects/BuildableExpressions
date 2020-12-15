namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using Xunit;

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
                            ctor.SetBody(Expression.Empty());
                        });
                    });

                    sc.AddClass("Class2", cls =>
                    {
                        cls.AddConstructor(ctor =>
                        {
                            ctor.SetConstructorCall(
                                class1.ConstructorExpressions.First(),
                                Expression.Constant("Nope"));
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
                            ctor.SetBody(Expression.Empty());
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
                                Expression.Constant("Nope"));
                        });
                    });
                });
            });

            ctorCallEx.Message.ShouldContain("Constructor DerivedClass(string)");
            ctorCallEx.Message.ShouldContain("cannot call");
            ctorCallEx.Message.ShouldContain("private constructor BaseClass(string)");
        }
    }
}