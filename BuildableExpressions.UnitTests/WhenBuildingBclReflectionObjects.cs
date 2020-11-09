namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using AgileObjects.BuildableExpressions.SourceCode.Extensions;
    using Common;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations.Reflection;
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
                        str.SetImplements<IMessager>(impl =>
                        {
                            var sendMessageLamba = Lambda<Action<object>>(
                                Default(typeof(void)),
                                Parameter(typeof(object), "message"));

                            impl.AddMethod(nameof(IMessager.SendMessage), sendMessageLamba);
                        });
                    });
                });

            sourceCode.ToCSharpString();

            sourceCode.ReferencedAssemblies
                .ShouldHaveSingleItem()
                .ShouldBe(typeof(IMessager).GetAssembly());
        }

        [Fact]
        public void ShouldCreateAnInterfaceType()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace("MyStuff.Tests");

                sc.AddInterface("IMyInterface", itf =>
                {
                    itf.AddMethod("DoNothing", typeof(void), m =>
                    {
                        m.AddParameters(
                            Parameter(typeof(string), "str1"),
                            Parameter(typeof(string), "str2"));
                    });
                });
            });

            var @interface = sourceCode.TypeExpressions.FirstOrDefault().ShouldNotBeNull();

            @interface.Type.ShouldNotBeNull();
            @interface.Type.Namespace.ShouldBe("MyStuff.Tests");
            @interface.Type.Name.ShouldBe("IMyInterface");
            @interface.Type.IsInterface().ShouldBeTrue();
            @interface.Type.IsGenericType().ShouldBeFalse();

            var method = @interface.Type.GetPublicInstanceMethod("DoNothing").ShouldNotBeNull();

            method.Name.ShouldBe("DoNothing");
            method.ReturnType.ShouldBe(typeof(void));
            method.GetParameters()[0].ParameterType.ShouldBe(typeof(string));
            method.GetParameters()[0].Name.ShouldBe("str1");
            method.GetParameters()[1].ParameterType.ShouldBe(typeof(string));
            method.GetParameters()[1].Name.ShouldBe("str2");
            method.IsGenericMethod.ShouldBeFalse();
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

        [Fact]
        public void ShouldBuildAnEnumType()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddEnum("Title", enm =>
                {
                    enm.AddMember("Dr");
                    enm.AddMember("Mr");
                    enm.AddMember("Mrs");
                });
            });

            var @enum = sourceCode.TypeExpressions.FirstOrDefault().ShouldNotBeNull();

            @enum.Type.ShouldNotBeNull();
            @enum.Type.Name.ShouldBe("Title");
            @enum.Type.IsValueType().ShouldBeTrue();
            @enum.Type.IsGenericType().ShouldBeFalse();
            @enum.Type.IsEnum().ShouldBeTrue();

            Enum.IsDefined(@enum.Type, "Dr").ShouldBeTrue();
            Enum.IsDefined(@enum.Type, 0).ShouldBeTrue();
            Enum.IsDefined(@enum.Type, "Mr").ShouldBeTrue();
            Enum.IsDefined(@enum.Type, 1).ShouldBeTrue();
            Enum.IsDefined(@enum.Type, "Mrs").ShouldBeTrue();
            Enum.IsDefined(@enum.Type, 2).ShouldBeTrue();
        }

        [Fact]
        public void ShouldCreateAParameterlessVoidClassMethodInfo()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass("MyClass", cls =>
                {
                    cls.AddMethod("DoNothing", Default(typeof(void)));
                });
            });

            var @class = sourceCode
                .TypeExpressions
                .FirstOrDefault()
                .ShouldNotBeNull();

            var method = @class
                .MethodExpressions
                .FirstOrDefault()
                .ShouldNotBeNull()
                .MethodInfo
                .ShouldNotBeNull();

            method.DeclaringType.ShouldBe(@class.Type);
            method.ReturnType.ShouldBe(typeof(void));
            method.Name.ShouldBe("DoNothing");
            method.GetParameters().ShouldBeEmpty();
            method.IsGenericMethod.ShouldBeFalse();
            method.IsPublic.ShouldBeTrue();
            method.IsStatic.ShouldBeFalse();
            method.IsAbstract.ShouldBeFalse();
        }

        [Fact]
        public void ShouldCreateAnInstanceGetSetStructPropertyInfo()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddStruct("MyStruct", str =>
                {
                    str.AddProperty<string>("Name", p =>
                    {
                        p.SetAutoProperty();
                    });
                });
            });

            var @struct = sourceCode
                .TypeExpressions
                .FirstOrDefault()
                .ShouldNotBeNull();

            var property = @struct
                .PropertyExpressions
                .FirstOrDefault()
                .ShouldNotBeNull();

            var iProperty = (IProperty)property;
            iProperty.IsReadable.ShouldBeTrue();
            iProperty.Getter.ShouldNotBeNull();
            iProperty.Getter.IsPublic.ShouldBeTrue();
            iProperty.Getter.Type.ShouldBe(typeof(string));
            iProperty.Getter.Name.ShouldBe("get");
            iProperty.IsWritable.ShouldBeTrue();
            iProperty.Setter.ShouldNotBeNull();
            iProperty.Setter.IsPublic.ShouldBeTrue();
            iProperty.Setter.Type.ShouldBe(typeof(void));
            iProperty.Setter.Name.ShouldBe("set");

            var propertyInfo = property
                .PropertyInfo.ShouldNotBeNull();

            propertyInfo.DeclaringType.ShouldBe(@struct.Type);
            propertyInfo.PropertyType.ShouldBe(typeof(string));
            propertyInfo.Name.ShouldBe("Name");
            propertyInfo.IsPublic().ShouldBeTrue();
            propertyInfo.IsStatic().ShouldBeFalse();
        }

        [Fact]
        public void ShouldCreateReflectionObjectsAtBuildTime()
        {
            var buildTimeClassType = default(Type);
            var buildTimePropertyInfo = default(PropertyInfo);

            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass("MyClass", cls =>
                {
                    // This type won't contain any of the properties:
                    buildTimeClassType = cls.ThisInstanceExpression.Type;

                    var propertyExpression = cls.AddProperty("Me", buildTimeClassType, p =>
                    {
                        p.SetAutoProperty();
                    });

                    // This will regenerate the class Type for its
                    // DeclaringType, including the 'Me' property:
                    buildTimePropertyInfo = propertyExpression.PropertyInfo;

                    // Add another property referencing the first to
                    // reset the class Type:
                    cls.AddProperty("MeAgain", buildTimeClassType, p =>
                    {
                        p.SetGetter(g =>
                        {
                            g.SetBody(Property(
                                cls.ThisInstanceExpression,
                                buildTimePropertyInfo));
                        });
                    });
                });

            });

            buildTimeClassType.ShouldNotBeNull();
            buildTimePropertyInfo.ShouldNotBeNull();

            var finalClassType = sourceCode
                .TypeExpressions
                .FirstOrDefault()
                .ShouldNotBeNull()
                .Type
                .ShouldNotBeNull();

            finalClassType.ShouldNotBeSameAs(buildTimeClassType);
            finalClassType.GetProperties().Length.ShouldBe(2);

            var finalPropertyInfo = finalClassType.GetProperties()[0];
            finalPropertyInfo.ShouldNotBeSameAs(buildTimePropertyInfo);
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
