namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using Common;
    using SourceCode;
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
    }
}