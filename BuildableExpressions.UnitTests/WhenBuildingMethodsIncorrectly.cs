namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethodsIncorrectly
    {
        [Fact]
        public void ShouldErrorIfNullMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod(default(string))));
            });

            methodNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("\t")));
            });

            methodNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc => sc
                    .AddStruct(cls => cls
                        .AddMethod(" My_Method")));
            });

            methodNameEx.Message.ShouldContain("invalid method name");
        }

        [Fact]
        public void ShouldErrorIfNullGenericMethodParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("NullParam", m =>
                        {
                            m.AddGenericParameter(null);
                        });
                    });
                });
            });

            paramNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfBlankGenericMethodParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("BlankParam", m =>
                        {
                            m.AddGenericParameter(string.Empty);
                        });
                    });
                });
            });

            paramNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceGenericMethodParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("WhitespaceParam", m =>
                        {
                            m.AddGenericParameter("    ");
                        });
                    });
                });
            });

            paramNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidMethodGenericParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("InvalidParam", m =>
                        {
                            m.AddGenericParameter("Param1!");
                        });
                    });
                });
            });

            paramNameEx.Message.ShouldContain("invalid generic parameter name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateMethodGenericParameterNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("Class1", cls =>
                    {
                        cls.AddMethod("Method1", m =>
                        {
                            m.AddGenericParameter("T1");
                            m.AddGenericParameter("T1");
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("Class1.Method1");
            configEx.Message.ShouldContain("duplicate generic parameter name");
            configEx.Message.ShouldContain("T1");
        }

        [Fact]
        public void ShouldErrorIfAbstractMethodAddedToNonAbstractClass()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("ConcreteClass", cls =>
                    {
                        cls.AddMethod("AbstractMethod", m =>
                        {
                            m.SetAbstract(typeof(void));
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("Unable to add abstract method");
            methodEx.Message.ShouldContain("'void AbstractMethod()'");
            methodEx.Message.ShouldContain("non-abstract declaring type 'ConcreteClass'");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothAbstractAndStatic()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(string), "str"));
                            m.SetAbstract(typeof(void));
                            m.SetStatic();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(string str)'");
            methodEx.Message.ShouldContain("both abstract and static");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothAbstractAndVirtual()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.SetAbstract(typeof(void));
                            m.SetVirtual();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod()'");
            methodEx.Message.ShouldContain("both abstract and virtual");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothStaticAndAbstract()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(DateTime), "date"));
                            m.AddParameter(Parameter(typeof(long), "number"));
                            m.SetStatic();
                            m.SetAbstract<string>();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(DateTime date, long number)'");
            methodEx.Message.ShouldContain("both static and abstract");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothStaticAndVirtual()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(DateTime), "date"));
                            m.SetStatic();
                            m.SetVirtual();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(DateTime date)'");
            methodEx.Message.ShouldContain("both static and virtual");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothVirtualAndAbstract()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(DateTime), "date"));
                            m.AddParameter(Parameter(typeof(long), "number"));
                            m.SetVirtual();
                            m.SetAbstract<DateTime>();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(DateTime date, long number)'");
            methodEx.Message.ShouldContain("both virtual and abstract");
        }

        [Fact]
        public void ShouldErrorIfClassMethodMarkedBothVirtualAndStatic()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddMethod("MyMethod", m =>
                        {
                            m.AddParameter(Parameter(typeof(long), "number"));
                            m.SetVirtual();
                            m.SetStatic();
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("'void MyMethod(long number)'");
            methodEx.Message.ShouldContain("both virtual and static");
        }

        [Fact]
        public void ShouldErrorIfDuplicateStructMethodSignature()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddStruct("MyStruct", str =>
                    {
                        str.AddMethod("DoNothing", m =>
                        {
                            m.AddParameter(Parameter(typeof(long), "number"));
                            m.SetBody(Empty());
                        });

                        str.AddMethod("DoNothing", m =>
                        {
                            m.AddParameter(Parameter(typeof(long), "number"));
                            m.SetBody(Empty());
                        });
                    });
                });
            });

            methodEx.Message.ShouldContain("MyStruct");
            methodEx.Message.ShouldContain("duplicate method signature");
            methodEx.Message.ShouldContain("'void DoNothing(long number)'");
        }

        [Fact]
        public void ShouldErrorIfStructMethodBodyNotSet()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddStruct("EmptyMethodStruct", str =>
                    {
                        str.AddMethod("EmptyMethod", _ => { });
                    });
                });
            });

            methodEx.Message.ShouldContain("void EmptyMethodStruct.EmptyMethod()");
            methodEx.Message.ShouldContain("no method body defined");
        }

        [Fact]
        public void ShouldErrorIfMethodAccessesUnscopedVariable()
        {
            var methodEx = Should.Throw<NotSupportedException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        var int1Variable = Parameter(typeof(int), "int1");
                        var int2Variable = Variable(typeof(int), "int2");
                        var addInts = Add(int1Variable, int2Variable);

                        cls.AddMethod(addInts);
                    });
                });
            });

            methodEx.Message.ShouldContain("undefined variable(s) 'int int1', 'int int2'");
        }

        [Fact]
        public void ShouldErrorIfOutParameterSetToRef()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(Parameter(typeof(string)), p =>
                            {
                                p.SetOut();
                                p.SetOut();
                                p.SetRef();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be both out and ref");
        }

        [Fact]
        public void ShouldErrorIfOutParameterSetToParamsArray()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(typeof(string[]), "words", p =>
                            {
                                p.SetOut();
                                p.SetParamsArray();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be both out and params");
        }

        [Fact]
        public void ShouldErrorIfRefParameterSetToOut()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(Parameter(typeof(string)), p =>
                            {
                                p.SetRef();
                                p.SetOut();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be both ref and out");
        }

        [Fact]
        public void ShouldErrorIfRefParameterSetToParamsArray()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(typeof(string[]), "words", p =>
                            {
                                p.SetRef();
                                p.SetRef();
                                p.SetParamsArray();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be both ref and params");
        }

        [Fact]
        public void ShouldErrorIfParamsArraySetToOutParameter()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(typeof(string[]), "words", p =>
                            {
                                p.SetParamsArray();
                                p.SetParamsArray();
                                p.SetOut();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be both params and out");
        }

        [Fact]
        public void ShouldErrorIfParamsArraySetToRefParameter()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(typeof(string[]), "words", p =>
                            {
                                p.SetParamsArray();
                                p.SetRef();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be both params and ref");
        }

        [Fact]
        public void ShouldErrorIfParamsArrayIsNotArrayType()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(typeof(string), "words", p =>
                            {
                                p.SetParamsArray();
                            });
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain("cannot be a params array");
            paramEx.Message.ShouldContain("has type 'string'.");
        }

        [Fact]
        public void ShouldErrorIfParameterAddedAfterParamsArray()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Nope", m =>
                        {
                            m.AddParameter(typeof(string[]), "words", p =>
                            {
                                p.SetParamsArray();
                            });

                            m.AddParameter<int>("count");
                        });
                    });
                });
            });

            paramEx.Message.ShouldContain(
                "parameters cannot be added after params array 'words'");
        }
    }
}