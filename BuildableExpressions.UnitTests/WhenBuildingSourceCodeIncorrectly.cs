﻿namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Common;
    using ReadableExpressions;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCodeIncorrectly
    {
        [Fact]
        public void ShouldErrorIfNoClasses()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
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
                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .Named(null)));
            });

            classNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfNullCustomClassName()
        {
            var classNameEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Default(typeof(void));

                var factory = new SourceCodeFactory(scf => scf
                    .NameClassesUsing((sc, ctx) => null));

                factory.CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing)))
                    .ToSourceCode();
            });

            classNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfBlankCustomClassName()
        {
            var classNameEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                var factory = new SourceCodeFactory(scf => scf
                    .NameClassesUsing((sc, ctx) => string.Empty));

                factory.CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing)))
                    .ToSourceCode();
            });

            classNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
             {
                 SourceCodeFactory.Default
                     .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .Named("   ")));
             });

            classNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .Named("X-Y-Z")));
            });

            classNameEx.Message.ShouldContain("invalid class name");
        }

        [Fact]
        public void ShouldErrorIfInvalidCustomClassName()
        {
            var classNameEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                var factory = new SourceCodeFactory(scf => scf
                    .NameClassesUsing((sc, ctx) => $"1_Class_{ctx.Index}"));

                factory.CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing)))
                    .ToSourceCode();
            });

            classNameEx.Message.ShouldContain("invalid class name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateClassNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .Named("MyClass")
                            .WithMethod(doNothing))
                        .WithClass(cls => cls
                            .Named("MyClass")
                            .WithMethod(doNothing)))
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
                var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
                var @class = sourceCode.AddClass();
                @class.BaseType = typeof(Stream);
                @class.BaseType = typeof(TestClassBase);
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

                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .Implementing(typeof(IMessager), typeof(IRandomStringFactory))
                            .WithMethod(getString)))
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
                var int1Variable = Parameter(typeof(int), "int1");
                var int2Variable = Variable(typeof(int), "int2");
                var addInts = Add(int1Variable, int2Variable);

                var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
                sourceCode.AddClass(cls => cls.WithMethod(addInts));
            });

            methodEx.Message.ShouldContain("undefined variable(s) 'int int1', 'int int2'");
        }

        [Fact]
        public void ShouldErrorIfNullMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing, m => m
                                .Named(null))));
            });

            methodNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfNullCustomMethodName()
        {
            var methodNameEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                var factory = new SourceCodeFactory(scf => scf
                    .NameMethodsUsing((sc, cls, ctx) => null));

                factory.CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing)))
                    .ToSourceCode();
            });

            methodNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfBlankCustomMethodName()
        {
            var methodNameEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                var factory = new SourceCodeFactory(scf => scf
                    .NameMethodsUsing((sc, cls, ctx) => string.Empty));

                factory.CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing)))
                    .ToSourceCode();
            });

            methodNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing, m => m
                                .Named("\t"))));
            });

            methodNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidMethodName()
        {
            var methodNameEx = Should.Throw<ArgumentException>(() =>
            {
                var doNothing = Lambda<Action>(Default(typeof(void)));

                SourceCodeFactory.Default
                    .CreateSourceCode(sc => sc
                        .WithClass(cls => cls
                            .WithMethod(doNothing, m => m
                                .Named(" My_Method"))));
            });

            methodNameEx.Message.ShouldContain("invalid method name");
        }

        [Fact]
        public void ShouldErrorIfInvalidCustomMethodName()
        {
            var methodNameEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Default(typeof(void));

                var factory = new SourceCodeFactory(scf => scf
                    .NameMethodsUsing((sc, cls, ctx) => $"Method {ctx.Index}"));

                factory.CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing)))
                    .ToSourceCode();
            });

            methodNameEx.Message.ShouldContain("invalid method name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateMethodNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Default(typeof(void));

                SourceCodeFactory.Default.CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .WithMethod(doNothing, m => m
                            .Named("MyMethod"))
                        .WithMethod(doNothing, m => m
                            .Named("MyMethod"))))
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

                var param1 = BuildableExpression.GenericParameter("T1");
                var param2 = BuildableExpression.GenericParameter("T1");

                SourceCodeFactory.Default.CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .Named("Class1")
                        .WithMethod(doNothing, m => m
                            .Named("Method1")
                            .WithGenericParameters(param1, param2))))
                    .ToSourceCode();
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

                var param = BuildableExpression.GenericParameter("T");

                SourceCodeFactory.Default.CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .Named("Class1")
                        .WithMethod(doNothing, m => m
                            .Named("Method1")
                            .WithGenericParameter(param))
                        .WithMethod(doNothing, m => m
                            .Named("Method2")
                            .WithGenericParameter(param))))
                    .ToSourceCode();
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
