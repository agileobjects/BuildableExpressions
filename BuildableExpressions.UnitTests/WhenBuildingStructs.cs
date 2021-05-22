namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Text;
    using BuildableExpressions.SourceCode.Generics;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingStructs
    {
        [Fact]
        public void ShouldBuildAStruct()
        {
            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddStruct("MyStruct", cls => cls
                        .AddMethod(Empty())))
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public struct MyStruct
    {
        public void DoAction()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructImplementingAnInterfaceExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var @interface = sc.AddInterface("IDoesNothing", itf =>
                    {
                        itf.AddMethod("DoNothing", typeof(void));
                    });

                    sc.AddStruct("StructImpl", cls =>
                    {
                        cls.SetImplements(@interface, impl =>
                        {
                            impl.AddMethod("DoNothing", m =>
                            {
                                m.SetBody(Empty());
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public interface IDoesNothing
    {
        void DoNothing();
    }

    public struct StructImpl : IDoesNothing
    {
        public void DoNothing()
        {
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericStructImplementingAGenericInterfaceExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var param = default(GenericParameterExpression);

                    var @interface = sc.AddInterface("ITypeGetter", itf =>
                    {
                        param = itf.AddGenericParameter("T");

                        itf.AddMethod("GetTypeName", typeof(string));
                    });

                    sc.AddStruct("StructTypeGetter", str =>
                    {
                        str.AddGenericParameter(param);

                        str.SetImplements(@interface, impl =>
                        {
                            impl.AddMethod("GetTypeName", m =>
                            {
                                m.SetBody(BuildableExpression.NameOf(param));
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public interface ITypeGetter<T>
    {
        string GetTypeName();
    }

    public struct StructTypeGetter<T> : ITypeGetter<T>
    {
        public string GetTypeName()
        {
            return nameof(T);
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructImplementingAClosedGenericInterfaceExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var param = default(GenericParameterExpression);

                    var @interface = sc.AddInterface("ITypeGetter", itf =>
                    {
                        param = itf.AddGenericParameter("T");

                        itf.AddMethod("GetTypeName", typeof(string));
                    });

                    sc.AddStruct("StringBuilderTypeGetter", str =>
                    {
                        str.SetImplements(@interface, impl =>
                        {
                            impl.SetGenericArgument<StringBuilder>(param);

                            impl.AddMethod("GetTypeName", m =>
                            {
                                m.SetBody(BuildableExpression.NameOf(param));
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System.Text;

namespace GeneratedExpressionCode
{
    public interface ITypeGetter<T>
    {
        string GetTypeName();
    }

    public struct StringBuilderTypeGetter : ITypeGetter<StringBuilder>
    {
        public string GetTypeName()
        {
            return nameof(StringBuilder);
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnEmptyStruct()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddStruct("EmptyStruct", _ => { });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public struct EmptyStruct
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericStruct()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddStruct("GenericStruct", str =>
                {
                    var param = str.AddGenericParameter("T");
                    var paramType = BuildableExpression.TypeOf(param);

                    str.AddMethod("GetParamType", m =>
                    {
                        m.SetBody(paramType);
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    public struct GenericStruct<T>
    {
        public Type GetParamType()
        {
            return typeof(T);
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }
    }
}