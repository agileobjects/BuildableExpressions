﻿namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using AgileObjects.BuildableExpressions.SourceCode.Extensions;
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

        [Fact]
        public void ShouldCreateAClassType()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace("MyStuff.Tests");

                sc.AddClass("MyClass", cls =>
                {
                    cls.AddMethod("DoNothing", Default(typeof(void)));
                });
            });

            var @class = sourceCode.TypeExpressions.FirstOrDefault().ShouldNotBeNull();

            @class.Type.ShouldNotBeNull();
            @class.Type.Namespace.ShouldBe("MyStuff.Tests");
            @class.Type.Name.ShouldBe("MyClass");
            @class.Type.IsClass().ShouldBeTrue();
            @class.Type.IsGenericType().ShouldBeFalse();

            var method = @class.Type.GetPublicInstanceMethod("DoNothing").ShouldNotBeNull();

            method.Name.ShouldBe("DoNothing");
            method.ReturnType.ShouldBe(typeof(void));
            method.GetParameters().ShouldBeEmpty();
            method.IsGenericMethod.ShouldBeFalse();

            var typeInstance = Activator.CreateInstance(@class.Type).ShouldNotBeNull();
            var methodResult = method.Invoke(typeInstance, Enumerable<object>.EmptyArray);
            methodResult.ShouldBeNull();
        }

        [Fact]
        public void ShouldNotCreateAnIncompleteClassType()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass("ThisClass", cls =>
                {
                    cls.AddMethod("GetThis", m =>
                    {
                        m.SetBody(cls.ThisInstanceExpression);
                    });
                });
            });

            var @class = sourceCode.TypeExpressions.FirstOrDefault().ShouldNotBeNull();

            @class.Type.ShouldNotBeNull();
            @class.Type.Name.ShouldBe("ThisClass");

            var method = @class.Type.GetPublicInstanceMethod("GetThis").ShouldNotBeNull();

            method.Name.ShouldBe("GetThis");
            method.ReturnType.ShouldBe(@class.Type);
            method.GetParameters().ShouldBeEmpty();
            method.IsGenericMethod.ShouldBeFalse();

            var typeInstance = Activator.CreateInstance(@class.Type).ShouldNotBeNull();
            var methodResult = method.Invoke(typeInstance, Enumerable<object>.EmptyArray);
            methodResult.ShouldBe(typeInstance);
        }

        [Fact]
        public void ShouldCreateAStructType()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace("MyStuff.Tests");

                sc.AddStruct("MyStruct", cls =>
                {
                    cls.AddMethod("DoNothing", Default(typeof(void)));
                });
            });

            var @struct = sourceCode.TypeExpressions.FirstOrDefault().ShouldNotBeNull();

            @struct.Type.ShouldNotBeNull();
            @struct.Type.Namespace.ShouldBe("MyStuff.Tests");
            @struct.Type.Name.ShouldBe("MyStruct");
            @struct.Type.IsValueType().ShouldBeTrue();
            @struct.Type.IsGenericType().ShouldBeFalse();

            var method = @struct.Type.GetPublicInstanceMethod("DoNothing").ShouldNotBeNull();

            method.Name.ShouldBe("DoNothing");
            method.ReturnType.ShouldBe(typeof(void));
            method.GetParameters().ShouldBeEmpty();
            method.IsGenericMethod.ShouldBeFalse();

            var typeInstance = Activator.CreateInstance(@struct.Type).ShouldNotBeNull();
            var methodResult = method.Invoke(typeInstance, Enumerable<object>.EmptyArray);
            methodResult.ShouldBeNull();
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
