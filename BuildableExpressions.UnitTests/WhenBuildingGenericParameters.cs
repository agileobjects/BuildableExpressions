namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using SourceCode;
    using Xunit;

    public class WhenBuildingGenericParameters
    {
        [Fact]
        public void ShouldBuildAnUnconstrainedParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("T");
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("T");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldNotBeNull();
            param.Method.Name.ShouldBe("DoStuff");
            param.Method.IsGeneric.ShouldBeTrue();
            param.Method.GenericArguments.ShouldHaveSingleItem().ShouldBe(param);
            param.Type.ShouldNotBeNull().Name.ShouldBe("T");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
        }

        [Fact]
        public void ShouldBuildANewableClassParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("TNewable", gp =>
                        {
                            gp.AddClassConstraint();
                            gp.AddNewableConstraint();
                        });
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TNewable");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldNotBeNull().Name.ShouldBe("DoStuff");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TNewable");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            Activator.CreateInstance(param.Type).ShouldNotBeNull().GetType().ShouldBe(param.Type);
        }

        [Fact]
        public void ShouldBuildAStructParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("TStruct", gp =>
                        {
                            gp.AddStructConstraint();
                        });
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TStruct");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldNotBeNull().Name.ShouldBe("DoStuff");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TStruct");
            param.Type.IsClass().ShouldBeFalse();
            param.Type.IsValueType().ShouldBeTrue();
        }

        [Fact]
        public void ShouldBuildATypeConstrainedParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("TDerived", gp =>
                        {
                            gp.AddTypeConstraint(typeof(BaseType));
                        });
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldNotBeNull().Name.ShouldBe("DoStuff");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(BaseType));
        }

        [Fact]
        public void ShouldBuildAnInterfaceConstrainedParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("TDisposable", gp =>
                        {
                            gp.AddTypeConstraint(typeof(IDisposable));
                        });
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDisposable");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldNotBeNull().Name.ShouldBe("DoStuff");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDisposable");
            param.Type.IsAbstract().ShouldBeFalse();
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(object));
            param.Type.GetAllInterfaces().ShouldHaveSingleItem().ShouldBe(typeof(IDisposable));
        }

        [Fact]
        public void ShouldBuildAnAbstractTypeConstrainedParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("TDerived", gp =>
                        {
                            gp.AddTypeConstraint(typeof(AbstractBaseType));
                        });
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldNotBeNull().Name.ShouldBe("DoStuff");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsAbstract().ShouldBeTrue();
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(AbstractBaseType));
        }

        [Fact]
        public void ShouldReuseBuiltParameterTypes()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T");
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByStructConstraint()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T");
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddStructConstraint();
                        });
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        #region Helper Members

        public class BaseType
        {
        }

        public abstract class AbstractBaseType
        {
            public void DoNothing()
            {
            }

            protected abstract void ImplementThis(string value1, int value2);
        }

        #endregion
    }
}
