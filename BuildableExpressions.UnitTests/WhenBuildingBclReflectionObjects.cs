namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingBclReflectionObjects
    {
        [Fact]
        public void ShouldAddReferenceAssembliesToSourceCode()
        {
            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls => { });
                });

            sourceCode.ToCSharpString();

            sourceCode.ReferencedAssemblies.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldAddClassBaseTypeReferenceAssembliesToSourceCode()
        {
            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("DerivedClass", cls =>
                    {
                        cls.SetBaseType<BaseType>();
                    });
                });

            sourceCode.ToCSharpString();

            sourceCode.ReferencedAssemblies
                .ShouldHaveSingleItem()
                .ShouldBe(typeof(BaseType).GetAssembly());
        }

        [Fact]
        public void ShouldAddStructInterfaceTypeReferenceAssembliesToSourceCode()
        {
            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("MessagerStruct", str =>
                    {
                        str.SetImplements<IMessager>();

                        var sendMessageLamba = Lambda<Action<object>>(
                            Default(typeof(void)),
                            Parameter(typeof(object), "message"));

                        str.AddMethod(nameof(IMessager.SendMessage), sendMessageLamba);
                    });
                });

            sourceCode.ToCSharpString();

            sourceCode.ReferencedAssemblies
                .ShouldHaveSingleItem()
                .ShouldBe(typeof(IMessager).GetAssembly());
        }

        #region Helper Members

        public abstract class BaseType { }

        public interface IMessager
        {
            void SendMessage(object message);
        }

        #endregion
    }
}
