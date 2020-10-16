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
            var param = BuildableExpression.GenericParameter("TNewable", gp => gp
                .WithClassConstraint()
                .WithNewableConstraint());

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
                .WithStructConstraint());

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
                .WithTypeConstraint(typeof(BaseType)));

            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(BaseType));
        }

        [Fact]
        public void ShouldBuildAnInterfaceConstrainedParameterWithAutoImplementedMethods()
        {
            var param = BuildableExpression.GenericParameter("TDisposable", gp => gp
                .WithTypeConstraint(typeof(IDisposable)));

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
        public void ShouldBuildATypeConstrainedParameterWithAutoImplementedMethods()
        {
            var param = BuildableExpression.GenericParameter("TDerived", gp => gp
                .WithTypeConstraint(typeof(AbstractBaseType)));

            param.Name.ShouldBe("TDerived");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TDerived");
            param.Type.IsAbstract().ShouldBeTrue();
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(AbstractBaseType));
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
