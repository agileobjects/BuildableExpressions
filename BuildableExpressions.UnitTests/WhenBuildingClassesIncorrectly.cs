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
            var baseTypeEx = Should.Throw<ArgumentNullException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(default, _ => { });
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfGivenMultipleBaseTypes()
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
        public void ShouldErrorIfAttributeTypeGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(typeof(Attribute));
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("'Attribute' is not a valid base type");
            baseTypeEx.Message.ShouldContain("create an AttributeExpression instead");
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
        public void ShouldErrorIfMarkedStaticAndAbstract()
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
        public void ShouldErrorIfMarkedStaticAndSealed()
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
        public void ShouldErrorIfMarkedAbstractAndStatic()
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
        public void ShouldErrorIfMarkedAbstractAndSealed()
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
        public void ShouldErrorIfMarkedSealedAndStatic()
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
        public void ShouldErrorIfMarkedSealedAndAbstract()
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