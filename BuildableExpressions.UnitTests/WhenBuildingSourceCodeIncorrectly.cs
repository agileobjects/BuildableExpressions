namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Common;
    using ReadableExpressions;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCodeIncorrectly
    {
        [Fact]
        public void ShouldErrorIfNoClasses()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .WithNamespaceOf<CommentExpression>())
                    .ToSourceCode();
            });

            classEx.Message.ShouldContain("class must be specified");
        }

        [Fact]
        public void ShouldErrorIfNullClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(null, cls => { }));
            });

            classNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfBlankClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(string.Empty, cls => { }));
            });

            classNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
             {
                 BuildableExpression
                     .SourceCode(sc => sc
                        .AddClass("   ", cls => { }));
             });

            classNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass("X-Y-Z", cls => { }));
            });

            classNameEx.Message.ShouldContain("invalid type name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateClassNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                BuildableExpression
                    .SourceCode(sc =>
                    {
                        sc.AddClass("MyClass", cls => cls.AddMethod(doNothing));
                        sc.AddClass("MyClass", cls => { });
                    })
                    .ToSourceCode();
            });

            configEx.Message.ShouldContain("Duplicate class name");
            configEx.Message.ShouldContain("MyClass");
        }

        [Fact]
        public void ShouldErrorIfClassGivenMultipleBaseTypes()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(typeof(Stream));
                        cls.SetBaseType(typeof(TestClassBase));
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("Unable to set class base type");
            baseTypeEx.Message.ShouldContain("already been set to Stream");
        }

        [Fact]
        public void ShouldErrorIfAmbiguousInterfacesImplemented()
        {
            var interfaceEx = Should.Throw<AmbiguousMatchException>(() =>
            {
                var getString = Lambda<Func<string>>(Default(typeof(string)));

                BuildableExpression
                    .SourceCode(sc =>
                    {
                        sc.AddClass(cls =>
                        {
                            cls.SetImplements(typeof(IMessager), typeof(IRandomStringFactory));
                            cls.AddMethod("GetString", getString);
                        });
                    })
                    .ToSourceCode();
            });

            interfaceEx.Message.ShouldContain("'(): string'");
            interfaceEx.Message.ShouldContain("matches multiple interface methods");
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
        public void ShouldErrorIfNullMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(cls => cls
                            .AddMethod(null, doNothing, m => { })));
            });

            methodNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(cls => cls
                            .AddMethod("\t", doNothing, m => { })));
            });

            methodNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(cls => cls
                            .AddMethod(" My_Method", doNothing, m => { })));
            });

            methodNameEx.Message.ShouldContain("invalid method name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateMethodNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        var doNothing = Default(typeof(void));

                        cls.AddMethod("MyMethod", doNothing, m => { });
                        cls.AddMethod("MyMethod", doNothing, m => { });
                    });
                })
                .ToSourceCode();
            });

            configEx.Message.ShouldContain("duplicate method name");
            configEx.Message.ShouldContain("MyMethod");
        }

        [Fact]
        public void ShouldErrorIfNullGenericParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.GenericParameter(null);
            });

            paramNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfBlankGenericParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.GenericParameter(string.Empty);
            });

            paramNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceGenericParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.GenericParameter("   ");
            });

            paramNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidGenericParameterName()
        {
            var paramNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.GenericParameter("Param1!");
            });

            paramNameEx.Message.ShouldContain("invalid generic parameter name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateGenericParameterNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Default(typeof(void));

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("Class1", cls =>
                    {
                        cls.AddMethod("Method1", doNothing, m =>
                        {
                            var param1 = BuildableExpression.GenericParameter("T1");
                            var param2 = BuildableExpression.GenericParameter("T1");

                            m.AddGenericParameters(param1, param2);
                        });
                    });
                })/*
                .ToSourceCode()*/;
            });

            configEx.Message.ShouldContain("Class1.Method1");
            configEx.Message.ShouldContain("duplicate generic parameter name");
            configEx.Message.ShouldContain("T1");
        }

        [Fact]
        public void ShouldErrorIfGenericParameterReused()
        {
            var paramEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Default(typeof(void));

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("Class1", cls =>
                    {
                        var param = BuildableExpression.GenericParameter("T");

                        cls.AddMethod("Method1", doNothing, m =>
                        {
                            m.AddGenericParameter(param);
                        });

                        cls.AddMethod("Method2", doNothing, m =>
                        {
                            m.AddGenericParameter(param);
                        });
                    });
                })/*
                .ToSourceCode()*/;
            });

            paramEx.Message.ShouldContain("Unable to add generic parameter");
            paramEx.Message.ShouldContain("Class1.Method1");
            paramEx.Message.ShouldContain("already been added");
            paramEx.Message.ShouldContain("Class1.Method2");
        }

        #region Helper Members

        public interface IMessager
        {
            string GetMessage();
        }

        public interface IRandomStringFactory
        {
            string Generate();
        }

        #endregion
    }
}
