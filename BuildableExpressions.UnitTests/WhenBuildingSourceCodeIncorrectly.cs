namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Text;
    using Common;
    using ReadableExpressions;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCodeIncorrectly
    {
        [Fact]
        public void ShouldErrorIfNoClasses()
        {
            var classEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.WithNamespaceOf<CommentExpression>();
                });
            });

            classEx.Message.ShouldContain("type must be specified");
        }

        [Fact]
        public void ShouldErrorIfNullClassName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(null, _ => { }));
            });

            classNameEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfBlankTypeName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass(string.Empty, _ => { }));
            });

            classNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfWhitespaceTypeName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
             {
                 BuildableExpression
                     .SourceCode(sc => sc
                        .AddClass("   ", _ => { }));
             });

            classNameEx.Message.ShouldContain("cannot be blank");
        }

        [Fact]
        public void ShouldErrorIfInvalidTypeName()
        {
            var classNameEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression
                    .SourceCode(sc => sc
                        .AddClass("X-Y-Z", _ => { }));
            });

            classNameEx.Message.ShouldContain("invalid type name");
        }

        [Fact]
        public void ShouldErrorIfDuplicateTypeNames()
        {
            var configEx = Should.Throw<InvalidOperationException>(() =>
            {
                var doNothing = Lambda<Action>(Empty());

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddStruct("Doer", cls => cls.AddMethod(doNothing));
                    sc.AddClass("Doer", _ => { });
                });
            });

            configEx.Message.ShouldContain("Duplicate type name");
            configEx.Message.ShouldContain("Doer");
        }

        [Fact]
        public void ShouldErrorIfNullTypeGivenAsStructInterface()
        {
            var interfaceEx = Should.Throw<ArgumentNullException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddStruct(str =>
                    {
                        str.SetImplements(default, _ => { });
                    });
                });
            });

            interfaceEx.Message.ShouldContain("cannot be null");
        }

        [Fact]
        public void ShouldErrorIfConcreteTypeGivenAsInterface()
        {
            var interfaceEx = Should.Throw<ArgumentException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetImplements(typeof(StringBuilder), _ => { });
                    });
                });
            });

            interfaceEx.Message.ShouldContain("'StringBuilder' is not an interface type");
        }

        [Fact]
        public void ShouldErrorIfInterfaceMethodNotImplemented()
        {
            var interfaceEx = Should.Throw<InvalidOperationException>(() =>
            {
                var getString = Lambda<Func<string>>(Default(typeof(string)));

                BuildableExpression.SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetImplements(typeof(IMessager), impl =>
                        {
                            impl.AddMethod("GetMsg", getString);
                        });
                    });
                });
            });

            interfaceEx.Message.ShouldContain("'string IMessager.GetMessage()'");
            interfaceEx.Message.ShouldContain("has not been implemented");
        }

        [Fact]
        public void ShouldErrorIfMissingClassGenericParameterName()
        {
            var parameterEx = Should.Throw<InvalidOperationException>(() =>
            {
                BuildableExpression.SourceCode(sc =>
                {
                    var baseType = sc.AddClass("MyBaseType", cls =>
                    {
                        cls.SetAbstract();
                        cls.AddGenericParameter("T");
                    });

                    sc.AddClass(cls =>
                    {
                        cls.SetBaseType(baseType, bt =>
                        {
                            bt.SetGenericArgument<int>("X");
                        });
                    });
                });
            });

            parameterEx.Message.ShouldContain("'MyBaseType<T>' ");
            parameterEx.Message.ShouldContain("no generic parameter named 'X'.");
        }

        #region Helper Members

        public interface IMessager
        {
            string GetMessage();
        }

        #endregion
    }
}
