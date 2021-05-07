namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
    using BuildableExpressions;
    using Common;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenBuildingSourceCode : TestClassBase
    {
        [Fact]
        public void ShouldRecogniseBlockVariablesAsInScope()
        {
            var intParameter = Parameter(typeof(int), "scopedInt");
            var intVariable = Variable(typeof(int), "blockScopedInt");
            var assignBlockInt = Assign(intVariable, Constant(1));
            var addInts = Add(intVariable, intParameter);
            var block = Block(new[] { intVariable }, assignBlockInt, addInts);
            var addIntsLambda = Lambda<Func<int, int>>(block, intParameter);

            var translated = BuildableExpression
                .SourceCode(sc => sc
                    .AddClass(cls => cls
                        .AddMethod("AddInts", addIntsLambda)))
                .ToCSharpString();

            const string expected = @"
namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public int AddInts
        (
            int scopedInt
        )
        {
            var blockScopedInt = 1;

            return blockScopedInt + scopedInt;
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        [Fact]
        public void ShouldPlaceTypesFirstInConstraintsList()
        {
            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetDerived", m =>
                    {
                        m.AddGenericParameter("TDerived", gp =>
                        {
                            gp.AddTypeConstraints(typeof(IMarker1), typeof(BaseType));
                        });

                        m.SetBody(Default(typeof(object)));
                    });
                });
            });

            var translated = sourceCode.ToCSharpString();

            const string expected = @"
using AgileObjects.BuildableExpressions.UnitTests;

namespace GeneratedExpressionCode
{
    public class GeneratedExpressionClass
    {
        public object GetDerived<TDerived>()
            where TDerived : WhenBuildingSourceCode.BaseType, WhenBuildingSourceCode.IMarker1
        {
            return null;
        }
    }
}";
            translated.ShouldBe(expected.TrimStart());
        }

        #region Helper Members

        public class BaseType { }

        public interface IMarker1 { }

        #endregion
    }
}
