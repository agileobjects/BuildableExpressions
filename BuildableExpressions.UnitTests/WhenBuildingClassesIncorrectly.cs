namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using Common;
    using Xunit;

    public class WhenBuildingClassesIncorrectly
    {
        [Fact]
        public void ShouldErrorIfNullTypeGivenAsBaseType()
        {
            var interfaceEx = Should.Throw<ArgumentNullException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(str =>
                    {
                        str.SetBaseType(default, bt => { });
                    });
                });
            });

            interfaceEx.Message.ShouldContain("cannot be null");
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
            baseTypeEx.Message.ShouldContain("already been set to 'Stream'");
        }

        [Fact]
        public void ShouldErrorIfInterfaceTypeGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(typeof(IDisposable));
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("'IDisposable' is not a valid base type");
        }

        [Fact]
        public void ShouldErrorIfSealedClassExpressionGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var sealedClass = sc.AddClass("MySealedClass", cls =>
                    {
                        cls.SetSealed();
                    });

                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(sealedClass);
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("'MySealedClass' is not a valid base type");
        }

        [Fact]
        public void ShouldErrorIfSealedClassTypeGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType<MySealedClass>();
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("MySealedClass' is not a valid base type");
        }

        [Fact]
        public void ShouldErrorIfStructTypeGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(typeof(DateTime));
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("'DateTime' is not a valid base type");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedStaticAndAbstract()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetStatic();
                        cls.SetAbstract();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both static and abstract");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedStaticAndSealed()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetStatic();
                        cls.SetSealed();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both static and sealed");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedAbstractAndStatic()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetAbstract();
                        cls.SetStatic();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both abstract and static");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedAbstractAndSealed()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetAbstract();
                        cls.SetSealed();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both abstract and sealed");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedSealedAndStatic()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetSealed();
                        cls.SetStatic();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both sealed and static");
        }

        [Fact]
        public void ShouldErrorIfClassMarkedSealedAndAbstract()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetSealed();
                        cls.SetAbstract();
                    });
                });
            });

            classEx.Message.ShouldContain("cannot be both sealed and abstract");
        }

        #region Helper Members

        public sealed class MySealedClass { }

        #endregion
    }
}