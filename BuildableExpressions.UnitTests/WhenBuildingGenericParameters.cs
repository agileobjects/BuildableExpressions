namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
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
            var param = BuildableExpression.GenericParameter("TStream", gp => gp
                .WithTypeConstraint(typeof(Stream)));

            param.Name.ShouldBe("TStream");
            param.IsClosed.ShouldBeFalse();
            param.Method.ShouldBeNull();
            param.Type.ShouldNotBeNull().Name.ShouldBe("TStream");
            param.Type.IsClass().ShouldBeTrue();
            param.Type.IsValueType().ShouldBeFalse();
            param.Type.BaseType.ShouldBe(typeof(Stream));
        }
    }
}
