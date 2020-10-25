namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Common;
    using NetStandardPolyfills;
    using SourceCode;
    using Xunit;

    public class WhenBuildingMethods : TestClassBase
    {
        [Fact]
        public void ShouldBuildAMethodFromAParameterlessLambdaAction()
        {
            var doNothing = Expression.Lambda<Action>(Expression.Default(typeof(void)));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod(doNothing)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void DoAction()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMethodFromADefaultVoidExpression()
        {
            var doNothing = Expression.Default(typeof(void));

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod(doNothing)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public void DoAction()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMethodFromAParameterlessLambdaFunc()
        {
            var returnOneThousand = CreateLambda(() => 1000);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("Get1000", returnOneThousand)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int Get1000()
        {
            return 1000;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMethodFromSingleParameterLambdaFunc()
        {
            var returnGivenLong = CreateLambda((long arg) => arg);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetLong", returnGivenLong)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public long GetLong
        (
            long arg
        )
        {
            return arg;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMethodFromATwoParameterLambdaAction()
        {
            var subtractShortFromInt = CreateLambda((int value1, short value2) => value1 - value2);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddStruct(str => str
                        .AddMethod("Subtract", subtractShortFromInt)))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct GeneratedExpressionStruct
    {
        public int Subtract
        (
            int value1,
            short value2
        )
        {
            return value1 - ((int)value2);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetType", m =>
                    {
                        var param = m.AddGenericParameter("T");
                        var paramType = BuildableExpression.TypeOf(param);

                        m.SetBody(paramType);
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public Type GetType<T>()
        {
            return typeof(T);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAMultipleGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetNames", m =>
                    {
                        var param1 = m.AddGenericParameter("T1");
                        var param1Name = BuildableExpression.NameOf(param1);

                        var param2 = m.AddGenericParameter("TParam2");
                        var param2Name = BuildableExpression.NameOf(param2);

                        var param3 = m.AddGenericParameter("T3");
                        var param3Name = BuildableExpression.NameOf(param3);

                        var concatMethod = typeof(string).GetPublicStaticMethod(
                            nameof(string.Concat),
                            typeof(string),
                            typeof(string),
                            typeof(string));

                        var nameConcatCall = Expression.Call(
                            concatMethod,
                            param1Name,
                            param2Name,
                            param3Name);

                        m.SetBody(nameConcatCall);
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public string GetNames<T1, TParam2, T3>()
        {
            return nameof(T1) + nameof(TParam2) + nameof(T3);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructConstrainedGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddStruct(str =>
                {
                    str.AddMethod("GetObject", m =>
                    {
                        m.AddGenericParameter("TStruct", gp =>
                        {
                            gp.AddStructConstraint();
                        });

                        m.SetBody(Expression.Default(typeof(object)));
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public struct GeneratedExpressionStruct
    {
        public object GetObject<TStruct>()
            where TStruct : struct
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildANewableClassConstrainedGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetObject", m =>
                    {
                        m.AddGenericParameter("TNewable", gp =>
                        {
                            gp.AddClassConstraint();
                            gp.AddNewableConstraint();
                        });

                        m.SetBody(Expression.Default(typeof(object)));
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetObject<TNewable>()
            where TNewable : class, new()
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAStructInterfaceConstrainedGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetMarker", m =>
                    {
                        m.AddGenericParameter("TMarker", gp =>
                        {
                            gp.AddStructConstraint();
                            gp.AddTypeConstraint<IMarker1>();
                        });

                        m.SetBody(Expression.Default(typeof(object)));
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetMarker<TMarker>()
            where TMarker : struct, WhenBuildingMethods.IMarker1
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAClassAndInterfaceConstrainedGenericParameterMethod()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetDerived", m =>
                    {
                        m.AddGenericParameter("TDerived", gp =>
                        {
                            gp.AddTypeConstraints(new List<Type>
                            {
                                typeof(BaseType), typeof(IMarker1), typeof(IMarker2)
                            });
                        });

                        m.SetBody(Expression.Default(typeof(object)));
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string EXPECTED = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetDerived<TDerived>()
            where TDerived : WhenBuildingMethods.BaseType, WhenBuildingMethods.IMarker1, WhenBuildingMethods.IMarker2
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildADefaultGenericParameterValueMethod()
        {
            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("GetT", m =>
                        {
                            var param = m.AddGenericParameter("T");
                            var paramDefault = Expression.Default(param.Type);

                            m.SetBody(paramDefault);
                        })))
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public T GetT<T>()
        {
            return default(T);
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnAbstractClassAbstractMethod()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("BaseClass", cls =>
                    {
                        cls.SetAbstract();

                        cls.AddMethod("AbstractMethod", m =>
                        {
                            m.SetAbstract();
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public abstract class BaseClass
    {
        public abstract void AbstractMethod();
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        #region Helper Members

        public class BaseType { }

        public interface IMarker1 { }

        public interface IMarker2 { }

        #endregion
    }
}