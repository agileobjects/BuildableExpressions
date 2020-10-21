namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using NetStandardPolyfills;
    using Xunit;

    public class WhenBuildingGenericParameters
    {
        [Fact]
        public void ShouldBuildAnUnconstrainedParameter()
        {
            var param = BuildableExpression.GenericParameter("T");

            param.Name.ShouldBe("T");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("T");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
        }

        [Fact]
        public void ShouldBuildANewableClassParameter()
        {
            var param = BuildableExpression.GenericParameter("TNewable", gp =>
            {
                gp.AddClassConstraint();
                gp.AddNewableConstraint();
            });

            param.Name.ShouldBe("TNewable");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TNewable");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            Activator.CreateInstance(param.Type).ShouldNotBeNull().GetType().ShouldBe(param.Type);
        }

        [Fact]
        public void ShouldBuildAStructParameter()
        {
            var param = BuildableExpression.GenericParameter("TStruct", gp => gp
                .AddStructConstraint());

            param.Name.ShouldBe("TStruct");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TStruct");
            param.Type.IsClass().ShouldBeFalse();
            param.Type.IsValueType().ShouldBeTrue();
        }

        [Fact]
        public void ShouldBuildATypeConstrainedParameter()
        {
            var param = BuildableExpression.GenericParameter("TDerived", gp => gp
                .AddTypeConstraint(typeof(BaseType)));

            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(BaseType));
        }

        [Fact]
        public void ShouldBuildAnInterfaceConstrainedParameter()
        {
            var param = BuildableExpression.GenericParameter("TDisposable", gp => gp
                .AddTypeConstraint(typeof(IDisposable)));

            param.Name.ShouldBe("TDisposable");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
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
            var param = BuildableExpression.GenericParameter("TDerived", gp => gp
                .AddTypeConstraint(typeof(AbstractBaseType)));

            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsAbstract().ShouldBeTrue();
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(AbstractBaseType));
        }

        [Fact]
        public void ShouldReuseBuiltParameterTypes()
        {
            var param1 = BuildableExpression.GenericParameter("T");
            var param2 = BuildableExpression.GenericParameter("T");

            param1.ShouldNotBeSameAs(param2);
            param1.Type.ShouldNotBeNull().ShouldBeSameAs(param2.Type);
        }

        [Fact]
        public void ShouldVaryBuiltParameterTypesByStructConstraint()
        {
            var param1 = BuildableExpression.GenericParameter("T");
            var param2 = BuildableExpression.GenericParameter("T", gp => gp.AddStructConstraint());

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
