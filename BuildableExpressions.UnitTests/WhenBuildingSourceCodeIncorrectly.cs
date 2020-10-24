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
                BuildableExpression.SourceCode(sc =>
                {
                    sc.WithNamespaceOf<CommentExpression>();
                });
            });

            classEx.Message.ShouldContain("type must be specified");
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

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls => cls.AddMethod(doNothing));
                    sc.AddClass("MyClass", cls => { });
                });
            });

            configEx.Message.ShouldContain("Duplicate type name");
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
        public void ShouldErrorIfInterfaceMethodNotImplemented()
        {
            var interfaceEx = Should.Throw<InvalidOperationException>(() =>
            {
                var getString = Lambda<Func<string>>(Default(typeof(string)));

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetImplements(typeof(IMessager));
                        cls.AddMethod("GetMsg", getString);
                    });
                });
            });

            interfaceEx.Message.ShouldContain("'string WhenBuildingSourceCodeIncorrectly.IMessager.GetMessage()'");
            interfaceEx.Message.ShouldContain("has not been implemented");
        }

        [Fact]
        public void ShouldErrorIfAmbiguousInterfacesImplemented()
        {
            var interfaceEx = Should.Throw<AmbiguousMatchException>(() =>
            {
                var getString = Lambda<Func<string>>(Default(typeof(string)));

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass("Stringy", cls =>
                    {
                        cls.SetImplements(typeof(ICodeGenerator), typeof(IRandomStringFactory));
                        cls.AddMethod(nameof(ICodeGenerator.Generate), getString);
                    });
                });
            });

            interfaceEx.Message.ShouldContain("'string Stringy.Generate()'");
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
                    .AddClass(cls => cls
                        .AddMethod(" My_Method")));
            });

            methodNameEx.Message.ShouldContain("invalid method name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateMethodSignatures()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("MyMethod");
                        cls.AddMethod("MyMethod");
                    });
                });
            });

            configEx.Message.ShouldContain("duplicate method signature");
            configEx.Message.ShouldContain("void MyMethod()");
        }

        [Fact]
        public void ShouldErrorIfNullGenericParameterName()
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
        public void ShouldErrorIfBlankGenericParameterName()
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
        public void ShouldErrorIfWhitespaceGenericParameterName()
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
        public void ShouldErrorIfInvalidGenericParameterName()
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
        public void ShouldErrorIfDuplicateGenericParameterNames()
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
        public void ShouldErrorIfEmptyStructMethod()
        {
            var methodEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddStruct("EmptyMethodStruct", str =>
                    {
                        str.AddMethod("EmptyMethod", m => { });
                    });
                });
            });

            methodEx.Message.ShouldContain("void EmptyMethodStruct.EmptyMethod()");
            methodEx.Message.ShouldContain("no method body defined");
        }

        #region Helper Members

        public interface IMessager
        {
            string GetMessage();
        }

        public interface ICodeGenerator
        {
            string Generate();
        }

        public interface IRandomStringFactory
        {
            string Generate();
        }

        #endregion
    }
}
