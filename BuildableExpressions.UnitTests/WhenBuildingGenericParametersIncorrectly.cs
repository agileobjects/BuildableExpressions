namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using Common;
    using Xunit;

    public class WhenBuildingGenericParametersIncorrectly
    {
        [Fact]
        public void ShouldErrorIfConstrainedToStructAndClass()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.GenericParameter("T", gp => gp
                    .WithStructConstraint()
                    .WithClassConstraint());
            });

            configEx.Message.ShouldContain("both struct and class constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToStructAndNewable()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.GenericParameter("T", gp => gp
                    .WithStructConstraint()
                    .WithNewableConstraint());
            });

            configEx.Message.ShouldContain("both struct and new() constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToStructAndType()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.GenericParameter("T", gp => gp
                    .WithStructConstraint()
                    .WithTypeConstraint<Stream>());
            });

            configEx.Message.ShouldContain("both struct and Stream constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToClassAndType()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.GenericParameter("T", gp => gp
                    .WithClassConstraint()
                    .WithTypeConstraint<StringComparer>());
            });

            configEx.Message.ShouldContain("both class and StringComparer constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToMultipleTypesConcurrently()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.GenericParameter("T", gp => gp
                    .WithTypeConstraints(typeof(StringComparer), typeof(Stream)));
            });

            configEx.Message.ShouldContain("both StringComparer and Stream constraints");
        }

        [Fact]
        public void ShouldErrorIfConstrainedToMultipleTypesSequentially()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.GenericParameter("T", gp => gp
                    .WithTypeConstraint<Stream>()
                    .WithTypeConstraint<StringComparer>());
            });

            configEx.Message.ShouldContain("both Stream and StringComparer constraints");
        }
    }
}