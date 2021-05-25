namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using Common;
    using Xunit;

    public class WhenBuildingAttributesIncorrectly
    {
        [Fact]
        public void ShouldErrorIfNullTypeGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<ArgumentNullException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddAttribute("NopeAttribute", attr =>
                    {
                        attr.SetBaseType(default, _ => { });
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
                    sc.AddAttribute("NopeAttribute", attr =>
                    {
                        attr.SetBaseType(typeof(BaseAttribute1));
                        attr.SetBaseType(typeof(BaseAttribute2));
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("Unable to set attribute base type");
            baseTypeEx.Message.ShouldContain("already been set to 'BaseAttribute1'");
        }

        [Fact]
        public void ShouldErrorIfNonAttributeTypeGivenAsBaseType()
        {
            var baseTypeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddAttribute("NopeAttribute", attr =>
                    {
                        attr.SetBaseType(typeof(Stream));
                    });
                });
            });

            baseTypeEx.Message.ShouldContain("'Stream' is not a valid base type");
            baseTypeEx.Message.ShouldContain("create a ClassExpression instead");
        }

        [Fact]
        public void ShouldErrorIfMarkedAbstractAndSealed()
        {
            var attributeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddAttribute("NopeAttribute", attr =>
                    {
                        attr.SetAbstract();
                        attr.SetSealed();
                    });
                });
            });

            attributeEx.Message.ShouldContain("cannot be both abstract and sealed");
        }

        [Fact]
        public void ShouldErrorIfMarkedSealedAndAbstract()
        {
            var attributeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddAttribute("NopeAttribute", attr =>
                    {
                        attr.SetSealed();
                        attr.SetAbstract();
                    });
                });
            });

            attributeEx.Message.ShouldContain("cannot be both sealed and abstract");
        }

        [Fact]
        public void ShouldErrorIfNameHasNoAttributeSuffix()
        {
            var attributeEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddAttribute("Nope", _ => { });
                });
            });

            attributeEx.Message.ShouldContain("Attribute 'Nope':");
            attributeEx.Message.ShouldContain("names must end with 'Attribute'");
        }

        #region Helper Members

        public abstract class BaseAttribute1 : Attribute
        {
        }

        public abstract class BaseAttribute2 : Attribute
        {
        }

        #endregion
    }
}