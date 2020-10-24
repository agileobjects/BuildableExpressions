namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

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
                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("T");
            param.IsClosed.ShouldBeFalse();
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

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TNewable");
            param.IsClosed.ShouldBeFalse();
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

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TStruct");
            param.IsClosed.ShouldBeFalse();
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

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
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

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDisposable");
            param.IsClosed.ShouldBeFalse();
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

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
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
                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByName()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T1");
                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T2");
                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
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
                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddStructConstraint();
                        });

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByClassConstraint()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddClassConstraint();
                        });

                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByNewableConstraint()
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
                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddNewableConstraint();
                        });

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByTypeConstraintPresence()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint(typeof(Stream));
                        });

                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByTypeConstraintDifference()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint(typeof(Stream));
                        });

                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint(typeof(StringComparer));
                        });

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByInterfaceTypeConstraintPresence()
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
                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint<IDisposable>();
                        });

                        m.SetBody(Default(typeof(void)));
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByInterfaceTypeConstraintDifference()
        {
            var param1 = default(GenericParameterExpression);
            var param2 = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param1 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint<IDisposable>();
                        });

                        m.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraints(new List<Type>
                            {
                                typeof(IDisposable),
                                typeof(IMessager)
                            });
                        });

                        m.SetBody(Default(typeof(void)));
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

        public interface IMessager
        {
            void SendMessage(string message);
        }

        #endregion
    }
}
