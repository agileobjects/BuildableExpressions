namespace AgileObjects.BuildableExpressions.UnitTests.SourceCode.Generics
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BuildableExpressions.SourceCode.Generics;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingGenericParameters
    {
        [Fact]
        public void ShouldBuildAnUnconstrainedMethodParameter()
        {
            var param = default(GenericParameterExpression);

            BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("DoStuff", m =>
                    {
                        param = m.AddGenericParameter("T");
                        m.SetBody(Empty());
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("T");
            param.Type.ShouldNotBeNull().Name.ShouldBe("T");
            param.Type.IsClass().ShouldBeFalse();
            param.Type.IsValueType().ShouldBeTrue();
        }

        [Fact]
        public void ShouldCreateAnUnconstrainedParameterExpression()
        {
            var typeParameter = new TypedGenericParameterExpression(
                typeof(List<>)
                    .GetGenericTypeArguments()
                    .First());

            typeParameter.ShouldNotBeNull();
            typeParameter.HasConstraints.ShouldBeFalse();
            typeParameter.HasClassConstraint.ShouldBeFalse();
            typeParameter.HasNewableConstraint.ShouldBeFalse();
            typeParameter.HasStructConstraint.ShouldBeFalse();
            typeParameter.TypeConstraints.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldBuildANewableClassMethodParameter()
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

                        m.SetBody(Empty());
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TNewable");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TNewable");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            Activator.CreateInstance(param.Type).ShouldNotBeNull().GetType().ShouldBe(param.Type);
        }

        [Fact]
        public void ShouldCreateANewableClassParameterExpression()
        {
            var typeParameter = new TypedGenericParameterExpression(
                typeof(NewableClassParameter<>)
                    .GetGenericTypeArguments()
                    .First());

            typeParameter.ShouldNotBeNull();
            typeParameter.HasConstraints.ShouldBeTrue();
            typeParameter.HasClassConstraint.ShouldBeTrue();
            typeParameter.HasNewableConstraint.ShouldBeTrue();
            typeParameter.HasStructConstraint.ShouldBeFalse();
            typeParameter.TypeConstraints.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldBuildAStructMethodParameter()
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

                        m.SetBody(Empty());
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TStruct");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TStruct");
            param.Type.IsClass().ShouldBeFalse();
            param.Type.IsValueType().ShouldBeTrue();
        }

        [Fact]
        public void ShouldCreateAStructParameterExpression()
        {
            var typeParameter = new TypedGenericParameterExpression(
                typeof(StructParameter<>)
                    .GetGenericTypeArguments()
                    .First());

            typeParameter.ShouldNotBeNull();
            typeParameter.HasConstraints.ShouldBeTrue();
            typeParameter.HasClassConstraint.ShouldBeFalse();
            typeParameter.HasNewableConstraint.ShouldBeFalse();
            typeParameter.HasStructConstraint.ShouldBeTrue();
            typeParameter.TypeConstraints.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldBuildATypeConstrainedMethodParameter()
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

                        m.SetBody(Empty());
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDerived");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.GetBaseType().ShouldBe(typeof(BaseType));
        }

        [Fact]
        public void ShouldCreateATypeConstrainedParameterExpression()
        {
            var typeParameter = new TypedGenericParameterExpression(
                typeof(TypeConstrainedParameter<>)
                    .GetGenericTypeArguments()
                    .First());

            typeParameter.ShouldNotBeNull();
            typeParameter.HasConstraints.ShouldBeTrue();
            typeParameter.HasClassConstraint.ShouldBeFalse();
            typeParameter.HasNewableConstraint.ShouldBeFalse();
            typeParameter.HasStructConstraint.ShouldBeFalse();
            typeParameter.TypeConstraints.ShouldHaveSingleItem().AsType().ShouldBe(typeof(BaseType));
        }

        [Fact]
        public void ShouldBuildAnInterfaceConstrainedMethodParameter()
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

                        m.SetBody(Empty());
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDisposable");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDisposable");
            param.Type.IsAbstract().ShouldBeFalse();
            param.Type.IsClass().ShouldBeFalse();
            param.Type.IsValueType().ShouldBeTrue();
            param.Type.GetAllInterfaces().ShouldHaveSingleItem().ShouldBe(typeof(IDisposable));
        }

        [Fact]
        public void ShouldCreateAnInterfaceConstrainedParameterExpression()
        {
            var typeParameter = new TypedGenericParameterExpression(
                typeof(InterfaceConstrainedParameter<>)
                    .GetGenericTypeArguments()
                    .First());

            typeParameter.ShouldNotBeNull();
            typeParameter.HasConstraints.ShouldBeTrue();
            typeParameter.HasClassConstraint.ShouldBeFalse();
            typeParameter.HasNewableConstraint.ShouldBeFalse();
            typeParameter.HasStructConstraint.ShouldBeFalse();
            typeParameter.TypeConstraints.ShouldHaveSingleItem().AsType().ShouldBe(typeof(IMessager));
        }

        [Fact]
        public void ShouldBuildAnAbstractTypeConstrainedMethodParameter()
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

                        m.SetBody(Empty());
                    });
                });
            });

            param.ShouldNotBeNull();
            param.Name.ShouldBe("TDerived");
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsAbstract().ShouldBeTrue();
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.GetBaseType().ShouldBe(typeof(AbstractBaseType));
        }

        [Fact]
        public void ShouldCreateANewableTypeAndInterfaceConstrainedParameterExpression()
        {
            var typeParameter = new TypedGenericParameterExpression(
                typeof(NewableTypeAndInterfaceConstrainedParameter<>)
                    .GetGenericTypeArguments()
                    .First());

            typeParameter.ShouldNotBeNull();
            typeParameter.HasConstraints.ShouldBeTrue();
            typeParameter.HasClassConstraint.ShouldBeFalse();
            typeParameter.HasNewableConstraint.ShouldBeTrue();
            typeParameter.HasStructConstraint.ShouldBeFalse();
            typeParameter.TypeConstraints.Count.ShouldBe(2);
            typeParameter.TypeConstraints.First().AsType().ShouldBe(typeof(AbstractBaseType));
            typeParameter.TypeConstraints.Last().AsType().ShouldBe(typeof(IMessager));
        }

        [Fact]
        public void ShouldReuseBuiltMethodParameterTypes()
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
                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByName()
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
                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T2");
                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByStructConstraint()
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
                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddStructConstraint();
                        });

                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByClassConstraint()
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

                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByNewableConstraint()
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
                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddNewableConstraint();
                        });

                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByTypeConstraintPresence()
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

                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T");
                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByTypeConstraintDifference()
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

                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint(typeof(StringComparer));
                        });

                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByInterfaceTypeConstraintPresence()
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
                        m.SetBody(Empty());
                    });

                    cls.AddMethod("DoMoarStuff", m =>
                    {
                        param2 = m.AddGenericParameter("T", gp =>
                        {
                            gp.AddTypeConstraint<IDisposable>();
                        });

                        m.SetBody(Empty());
                    });
                });
            });

            param1.ShouldNotBeNull();
            param2.ShouldNotBeNull();
            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldNotBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltMethodParameterTypesByInterfaceTypeConstraintDifference()
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

                        m.SetBody(Empty());
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

                        m.SetBody(Empty());
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

        // ReSharper disable once UnusedTypeParameter
        public class NewableClassParameter<T>
            where T : class, new()
        {
        }

        // ReSharper disable once UnusedTypeParameter
        public class StructParameter<T>
            where T : struct
        {
        }

        // ReSharper disable once UnusedTypeParameter
        public class TypeConstrainedParameter<T>
            where T : BaseType
        {
        }

        // ReSharper disable once UnusedTypeParameter
        public class InterfaceConstrainedParameter<T>
            where T : IMessager
        {
        }

        // ReSharper disable once UnusedTypeParameter
        public class NewableTypeAndInterfaceConstrainedParameter<T>
            where T : AbstractBaseType, IMessager, new()
        {
        }

        #endregion
    }
}
