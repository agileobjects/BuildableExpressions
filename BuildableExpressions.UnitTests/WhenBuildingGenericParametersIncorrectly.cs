namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using Common;
    using SourceCode;
    using Xunit;

    public class WhenBuildingGenericParametersIncorrectly
    {
        [Fact]
        public void ShouldErrorIfConstrainedToStructAndClass()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddStructConstraint();
                                gp.AddClassConstraint();
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both struct and class constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToClassAndStruct()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddClassConstraint();
                                gp.AddStructConstraint();
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both class and struct constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToStructAndNewable()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddStructConstraint();
                                gp.AddNewableConstraint();
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both struct and new() constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToStructAndType()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddStructConstraint();
                                gp.AddTypeConstraint<Stream>();
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both struct and Stream constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToClassAndType()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddClassConstraint();
                                gp.AddTypeConstraint<StringComparer>();
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both class and StringComparer constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToMultipleTypesConcurrently()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddTypeConstraints(typeof(StringComparer), typeof(Stream));
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both StringComparer and Stream constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToMultipleTypesSequentially()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("DoStuff", m =>
                        {
                            m.AddGenericParameter("T", gp =>
                            {
                                gp.AddTypeConstraint<Stream>();
                                gp.AddTypeConstraint<StringComparer>();
                            });
                        });
                    });
                });
            });

            configEx.Message.ShouldContain("both Stream and StringComparer constraints");
        }
    }
}