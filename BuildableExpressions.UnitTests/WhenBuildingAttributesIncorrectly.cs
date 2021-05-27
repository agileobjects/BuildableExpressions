﻿namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.IO;
    using System.Linq.Expressions;
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

        [Fact]
        public void ShouldErrorIfConstructorArgumentsMismatch()
        {
            var attributeCtorEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var ctorAttr = sc.AddAttribute("CtorAttribute", attr =>
                    {
                        attr.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<object>("value");
                            ctor.SetBody(Expression.Empty());
                        });
                    });

                    sc.AddInterface("IAttributed", itf =>
                    {
                        itf.AddAttribute(ctorAttr, attr =>
                        {
                            attr.SetConstructorArguments(null, 123, DateTime.Now);
                        });
                    });
                });
            });

            attributeCtorEx.Message.ShouldContain("Unable to find 'CtorAttribute' constructor matching");
            attributeCtorEx.Message.ShouldContain("'null, int, DateTime'");
            attributeCtorEx.Message.ShouldContain("Available constructor(s) are");
            attributeCtorEx.Message.ShouldContain("- (object)");
        }

        [Fact]
        public void ShouldErrorIfConstructorIsAmbiguous()
        {
            var attributeCtorEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var ctorAttr = sc.AddAttribute("CtorAttribute", attr =>
                    {
                        attr.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<DateTime?>("value");
                            ctor.SetBody(Expression.Empty());
                        });
                        
                        attr.AddConstructor(ctor =>
                        {
                            ctor.AddParameter<string>("value");
                            ctor.SetBody(Expression.Empty());
                        });
                    });

                    sc.AddInterface("IAttributed", itf =>
                    {
                        itf.AddAttribute(ctorAttr, attr =>
                        {
                            attr.SetConstructorArguments(null);
                        });
                    });
                });
            });

            attributeCtorEx.Message.ShouldContain("Multiple 'CtorAttribute' constructors match");
            attributeCtorEx.Message.ShouldContain("'null'");
            attributeCtorEx.Message.ShouldContain("- (DateTime?)");
            attributeCtorEx.Message.ShouldContain("- (string)");
        }

        [Fact]
        public void ShouldErrorIfArgumentsGivenIfParameterlessOnly()
        {
            var attributeCtorEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var ctorAttr = sc.AddAttribute("CtorAttribute");

                    sc.AddInterface("IAttributed", itf =>
                    {
                        itf.AddAttribute(ctorAttr, attr =>
                        {
                            attr.SetConstructorArguments("Nope");
                        });
                    });
                });
            });

            attributeCtorEx.Message.ShouldContain("Unable to find 'CtorAttribute' constructor");
            attributeCtorEx.Message.ShouldContain("'string'");
            attributeCtorEx.Message.ShouldContain("Only a parameterless constructor");
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