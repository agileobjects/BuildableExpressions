namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions.SourceCode;
    using BuildableExpressions.SourceCode.Generics;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingClasses
    {
        [Fact]
        public void ShouldBuildAnImplementationClassAndMethod()
        {
            var sayHello = Lambda<Func<string>>(Constant("Hello!"));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass("Messager", cls =>
                    {
                        cls.SetImplements<IMessager>(impl =>
                        {
                            impl.AddMethod(nameof(IMessager.GetMessage), sayHello);
                        });
                    }))
                .ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class Messager : WhenBuildingClasses.IMessager
    {
        public string GetMessage()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassWithMultipleImplementationMethods()
        {
            var sayHello = Lambda<Func<string>>(Constant("Hello!"));
            var return123 = Lambda<Func<int>>(Constant(123));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls =>
                    {
                        cls.SetImplements(typeof(IMessager), impl =>
                        {
                            impl.AddMethod(nameof(IMessager.GetMessage), sayHello);
                        });

                        cls.SetImplements(typeof(INumberSource), impl =>
                        {
                            impl.AddMethod(nameof(INumberSource.GetNumber), return123);
                        });

                    }))
                .ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass : WhenBuildingClasses.IMessager, WhenBuildingClasses.INumberSource
    {
        public string GetMessage()
        {
            return ""Hello!"";
        }

        public int GetNumber()
        {
            return 123;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassWithABaseType()
        {
            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass("DerivedType", cls =>
                    {
                        cls.SetBaseType<BaseType>();
                        cls.AddMethod("SayHello", Constant("Hello!"));
                    }))
                .ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class DerivedType : WhenBuildingClasses.BaseType
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassWithABaseClassExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var baseType = sc.AddClass("BaseType", cls => { });

                    sc.AddClass("DerivedType", cls =>
                    {
                        cls.SetBaseType(baseType);
                        cls.AddMethod("SayGoodbye", Constant("Goodbye!"));
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class BaseType
    {
    }

    public class DerivedType : BaseType
    {
        public string SayGoodbye()
        {
            return ""Goodbye!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericClassWithAGenericBaseType()
        {
            var sourceCode = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass("GenericDerivedType", cls =>
                    {
                        cls.AddGenericParameter("T");
                        cls.SetBaseType(typeof(GenericBaseType<>));
                        cls.AddMethod("SayHello", Constant("Hello!"));
                    }));

            var genericDerivedClass = sourceCode
                .TypeExpressions
                .ShouldHaveSingleItem()
                .ShouldBeOfType<ClassExpression>();

            genericDerivedClass.Name.ShouldBe("GenericDerivedType");
            genericDerivedClass.IsGeneric.ShouldBeTrue();
            genericDerivedClass.GenericParameters.ShouldHaveSingleItem();
            genericDerivedClass.BaseType.ShouldBe(typeof(GenericBaseType<>));

            var genericBaseType = genericDerivedClass
                .BaseTypeExpression.ShouldNotBeNull();

            genericBaseType.Name.ShouldBe("GenericBaseType<T>");
            genericBaseType.IsGeneric.ShouldBeTrue();
            genericBaseType.GenericParameters.ShouldHaveSingleItem();
            genericBaseType.BaseType.ShouldBe(typeof(object));

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GenericDerivedType<T> : WhenBuildingClasses.GenericBaseType<T>
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassWithADerivedBaseType()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass("GrandchildClass", cls =>
                {
                    cls.SetBaseType<ChildBaseType>();
                    cls.AddMethod("SayHello", Constant("Hello!"));
                });
            });

            var grandchildClass = sourceCode
                .TypeExpressions
                .ShouldHaveSingleItem()
                .ShouldBeOfType<ClassExpression>();

            grandchildClass.Name.ShouldBe("GrandchildClass");
            grandchildClass.BaseType.ShouldBe(typeof(ChildBaseType));

            var parentClassExpression = grandchildClass
                .BaseTypeExpression.ShouldNotBeNull();

            parentClassExpression.Name.ShouldBe(nameof(ChildBaseType));
            parentClassExpression.Type.ShouldBe(typeof(ChildBaseType));
            parentClassExpression.BaseType.ShouldBe(typeof(BaseType));
            parentClassExpression.IsStatic.ShouldBeFalse();
            parentClassExpression.IsSealed.ShouldBeFalse();
            parentClassExpression.IsAbstract.ShouldBeFalse();
            parentClassExpression.IsGeneric.ShouldBeFalse();
            sourceCode.TypeExpressions.ShouldNotContain(parentClassExpression);

            var grandparentClassExpression = parentClassExpression
                .BaseTypeExpression.ShouldNotBeNull();

            grandparentClassExpression.Name.ShouldBe(nameof(BaseType));
            grandparentClassExpression.Type.ShouldBe(typeof(BaseType));
            grandparentClassExpression.BaseType.ShouldBe(typeof(object));
            grandparentClassExpression.IsStatic.ShouldBeFalse();
            grandparentClassExpression.IsSealed.ShouldBeFalse();
            grandparentClassExpression.IsAbstract.ShouldBeTrue();
            grandparentClassExpression.IsGeneric.ShouldBeFalse();
            sourceCode.TypeExpressions.ShouldNotContain(grandparentClassExpression);

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GrandchildClass : WhenBuildingClasses.ChildBaseType
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericClassWithAPartClosedGenericBaseType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var param1 = default(GenericParameterExpression);

                    var baseType = sc.AddClass("Basey", cls =>
                    {
                        cls.SetAbstract();

                        param1 = cls.AddGenericParameter("T1");
                        cls.AddGenericParameter("T2");
                    });

                    sc.AddClass("Derivey", cls =>
                    {
                        cls.AddGenericParameter(param1);

                        cls.SetBaseType(baseType, bt =>
                        {
                            bt.SetGenericArgument<string>("T2");
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class Basey<T1, T2>
    {
    }

    public class Derivey<T1> : Basey<T1, string>
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassImplementingAnInterfaceExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var @interface = sc.AddInterface("IMyInterface", itf =>
                    {
                        itf.AddMethod("GetMessage", typeof(string));
                    });

                    sc.AddClass("ClassImpl", cls =>
                    {
                        cls.SetImplements(@interface, impl =>
                        {
                            impl.AddMethod("GetMessage", m =>
                            {
                                m.SetBody(Constant("Hello!", typeof(string)));
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public interface IMyInterface
    {
        string GetMessage();
    }

    public class ClassImpl : IMyInterface
    {
        public string GetMessage()
        {
            return ""Hello!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassImplementingAClosedGenericInterfaceType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("StringComparer", cls =>
                    {
                        cls.SetImplements<IComparable<string>>(impl =>
                        {
                            impl.AddMethod("CompareTo", m =>
                            {
                                m.AddParameter(Parameter(typeof(string), "other"));
                                m.SetBody(Constant(-1, typeof(int)));
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class StringComparer : IComparable<string>
    {
        public int CompareTo
        (
            string other
        )
        {
            return -1;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassBaseTypeAndDerivedTypes()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.SetNamespace("AgileObjects.Tests.Yo");

                    var baseType = sc.AddClass("MyBaseType", cls =>
                    {
                        cls.SetAbstract();
                    });

                    sc.AddClass("DerivedOne", cls =>
                    {
                        cls.SetBaseType(baseType);
                        cls.AddMethod("GetOne", Constant("One", typeof(string)));
                    });

                    sc.AddClass("DerivedTwo", cls =>
                    {
                        cls.SetBaseType(baseType);
                        cls.AddMethod("GetTwo", Constant("Two", typeof(string)));
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace AgileObjects.Tests.Yo
{
    public abstract class MyBaseType
    {
    }

    public class DerivedOne : MyBaseType
    {
        public string GetOne()
        {
            return ""One"";
        }
    }

    public class DerivedTwo : MyBaseType
    {
        public string GetTwo()
        {
            return ""Two"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldCallAnAbstractBaseTypeMethodFromADerivedType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var baseTypeMethod = default(MethodExpression);

                    var baseType = sc.AddClass("BaseType", cls =>
                    {
                        cls.SetAbstract();

                        baseTypeMethod = cls.AddMethod("GetNameBase", m =>
                        {
                            m.SetVisibility(MemberVisibility.Protected);
                            m.SetBody(Constant("Steve", typeof(string)));
                        });
                    });

                    sc.AddClass("DerivedType", cls =>
                    {
                        cls.SetBaseType(baseType);

                        cls.AddMethod("GetName", m =>
                        {
                            var baseTypeMethodCall = Call(
                                cls.BaseInstanceExpression,
                                baseTypeMethod.MethodInfo);

                            m.SetBody(baseTypeMethodCall);
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class BaseType
    {
        protected string GetNameBase()
        {
            return ""Steve"";
        }
    }

    public class DerivedType : BaseType
    {
        public string GetName()
        {
            return base.GetNameBase();
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAbstractClass()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetAbstract();
                        cls.AddMethod("GetString", Default(typeof(string)));
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class GeneratedExpressionClass
    {
        public string GetString()
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildASealedClass()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MySealedClass", cls =>
                    {
                        cls.SetSealed();
                        cls.AddMethod("GetString", Constant("String!"));
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public sealed class MySealedClass
    {
        public string GetString()
        {
            return ""String!"";
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldUseStaticClassAndMethodScopes()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.SetStatic();

                        cls.AddMethod("GetString", Default(typeof(string)));

                        cls.AddMethod("GetInt", Default(typeof(int)), m =>
                        {
                            m.SetStatic();
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public static class GeneratedExpressionClass
    {
        public static string GetString()
        {
            return null;
        }

        public static int GetInt()
        {
            return default(int);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnEmptyClass()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("EmptyClass", cls => { });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class EmptyClass
    {
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        public abstract class BaseType { }

        // ReSharper disable once UnusedTypeParameter
        public class GenericBaseType<T> { }

        public class ChildBaseType : BaseType { }

        public interface IMessager
        {
            string GetMessage();
        }

        public interface INumberSource
        {
            int GetNumber();
        }

        #endregion
    }
}