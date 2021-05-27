﻿namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using System.Collections.Generic;
    using BuildableExpressions.SourceCode.Generics;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingInterfaces
    {
        [Fact]
        public void ShouldBuildAnEmptyInterface()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IMarker");
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public interface IMarker
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceWithAMethod()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("ILogger", itf =>
                    {
                        itf.AddMethod("Log", typeof(void), m =>
                        {
                            m.AddParameter(Parameter(typeof(Exception), "ex"));
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    public interface ILogger
    {
        void Log
        (
            Exception ex
        );
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceImplementingAnInterfaceType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IDisposableMarker", itf =>
                    {
                        itf.SetImplements<IDisposable>();
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    public interface IDisposableMarker : IDisposable
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceImplementingAClosedInterfaceType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IStringEquatable", itf =>
                    {
                        itf.SetImplements<IEquatable<string>>();
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System;

namespace GeneratedExpressionCode
{
    public interface IStringEquatable : IEquatable<string>
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAnInterfaceImplementingAPartClosedGenericInterfaceType()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddInterface("IStringDictionary", itf =>
                    {
                        itf.AddGenericParameter("TValue");

                        itf.SetImplements(typeof(IDictionary<,>), impl =>
                        {
                            impl.SetGenericArgument<string>("TKey");
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
using System.Collections.Generic;

namespace GeneratedExpressionCode
{
    public interface IStringDictionary<TValue> : IDictionary<string, TValue>
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldBuildAGenericInterfacePartClosingAnotherGenericInterface()
        {
            var translated = BuildableExpression
                .SourceCode(sc =>
                {
                    var typeParam1 = default(GenericParameterExpression);

                    var baseInterface = sc.AddInterface("IBaseGeneric", itf =>
                    {
                        typeParam1 = itf.AddGenericParameter("T1");
                        itf.AddGenericParameter("T2");
                    });

                    sc.AddInterface("IDerivedGeneric", itf =>
                    {
                        itf.AddGenericParameter(typeParam1);

                        itf.SetImplements(baseInterface, impl =>
                        {
                            impl.SetGenericArgument("T2", typeof(double));
                        });
                    });
                })
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public interface IBaseGeneric<T1, T2>
    {
    }

    public interface IDerivedGeneric<T1> : IBaseGeneric<T1, double>
    {
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }
    }
}
