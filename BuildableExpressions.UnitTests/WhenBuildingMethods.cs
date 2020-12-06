namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingMethods : TestClassBase
    {
        [Fact]
        public void ShouldBuildAMethodFromAParameterlessLambdaAction()
        {
            var doNothing = Lambda<Action>(Empty());

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
            var doNothing = Empty();

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
        public void ShouldBuildAMethodWithACustomReturnType()
        {

            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("Get1000", m =>
                        {
                            var returnOneThousand = CreateLambda(() => 1000);
                            m.SetBody(returnOneThousand, typeof(object));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object Get1000()
        {
            return 1000;
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

                        var nameConcatCall = Call(
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

                        m.SetBody(Default(typeof(object)));
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

                        m.SetBody(Default(typeof(object)));
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

                        m.SetBody(Default(typeof(object)));
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

                        m.SetBody(Default(typeof(object)));
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
                .SourceCode(sc =>
                {
                    sc.AddClass(cls =>
                    {
                        cls.AddMethod("GetT", m =>
                        {
                            var param = m.AddGenericParameter("T");
                            var paramDefault = Default(param.Type);

                            m.SetBody(paramDefault);
                        });
                    });
                })
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
                            m.SetAbstract(typeof(void));
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

        [Fact]
        public void ShouldBuildAVirtualMethod()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.AddMethod("VirtualMethod", m =>
                        {
                            m.SetVirtual();
                            m.SetBody(Empty());
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class MyClass
    {
        public virtual void VirtualMethod()
        {
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldOverrideABaseAbstractMethodExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var overrideMe = default(MethodExpression);

                    var baseType = sc.AddClass("BaseAbstract", cls =>
                    {
                        cls.SetAbstract();

                        overrideMe = cls.AddMethod("OverrideMe", m =>
                        {
                            m.SetAbstract<string>();
                            m.AddParameter<DateTime>("when");
                        });
                    });

                    sc.AddClass("DerivedAbstract", cls =>
                    {
                        cls.SetBaseType(baseType, impl =>
                        {
                            impl.AddMethod(overrideMe, m =>
                            {
                                m.SetBody(Default(typeof(string)));
                            });
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
using System;

namespace GeneratedExpressionCode
{
    public abstract class BaseAbstract
    {
        public abstract string OverrideMe
        (
            DateTime when
        );
    }

    public class DerivedAbstract : BaseAbstract
    {
        public override string OverrideMe
        (
            DateTime when
        )
        {
            return null;
        }
    }
}";
            EXPECTED.ShouldCompile();
            translated.ShouldBe(EXPECTED.TrimStart());
        }

        [Fact]
        public void ShouldOverrideABaseVirtualMethodExpression()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var overrideMe = default(MethodExpression);

                    var baseType = sc.AddClass("BaseVirtual", cls =>
                    {
                        overrideMe = cls.AddMethod("OverrideMe", p =>
                        {
                            p.SetVirtual();
                            p.SetBody(Default(typeof(string)));
                        });
                    });

                    sc.AddClass("DerivedVirtual", cls =>
                    {
                        cls.SetBaseType(baseType);
                        cls.AddMethod(overrideMe, m =>
                        {
                            m.SetBody(Constant("Hello!", typeof(string)));
                        });
                    });
                })
                .ToCSharpString();

            const string EXPECTED = @"
namespace GeneratedExpressionCode
{
    public class BaseVirtual
    {
        public virtual string OverrideMe()
        {
            return null;
        }
    }

    public class DerivedVirtual : BaseVirtual
    {
        public override string OverrideMe()
        {
            return ""Hello!"";
        }
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