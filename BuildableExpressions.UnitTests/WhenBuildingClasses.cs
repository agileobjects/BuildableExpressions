namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Linq;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using SourceCode;
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
                        cls.SetImplements<IMessager>();
                        cls.AddMethod(nameof(IMessager.GetMessage), sayHello);
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
                        cls.SetImplements(typeof(IMessager));
                        cls.SetImplements(typeof(INumberSource));
                        cls.AddMethod(nameof(IMessager.GetMessage), sayHello);
                        cls.AddMethod(nameof(INumberSource.GetNumber), return123);
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
                        cls.SetImplements(@interface);

                        cls.AddMethod("GetMessage", m =>
                        {
                            m.SetBody(Constant("Hello!", typeof(string)));
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
                        cls.SetImplements<IComparable<string>>();

                        cls.AddMethod("CompareTo", m =>
                        {
                            var parameterType = typeof(IComparable<>)
                                .GetGenericTypeArguments()
                                .First();

                            m.AddParameter(Parameter(parameterType, "T"));
                            m.SetBody(Constant(-1, typeof(int)));
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

        public class BaseType { }

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