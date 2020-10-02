namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using BuildableExpressions;
    using Common;
    using SourceCode;
    using Xunit;
    using static System.Linq.Expressions.Expression;

    public class WhenVisitingSourceCodeExpressions : TestClassBase
    {
        [Fact]
        public void ShouldVisitSourceCodeExpressions()
        {
            var returnOneThousand = CreateLambda(() => 1000);

            var sourceCode = SourceCodeFactory.Default
                .CreateSourceCode(sc => sc
                    .WithClass(cls => cls
                        .Named("MyClass")
                        .WithSummary("This is my class")
                        .WithMethod(returnOneThousand)));

            var visitor = new VisitationHelper();

            visitor.Visit(sourceCode);

            visitor.VisitedExpressions.ShouldContain(returnOneThousand.Body);
            visitor.SourceCodeVisited.ShouldBeTrue();
            visitor.ClassVisited.ShouldBeTrue();
            visitor.MethodVisited.ShouldBeTrue();
        }

        [Fact]
        public void ShouldVisitABuildableMethodCallExpression()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();
            var method1 = @class.AddMethod(Default(typeof(void)));

            var method1Call = BuildableExpression.Call(method1);
            @class.AddMethod(method1Call);

            var visitor = new VisitationHelper();

            visitor.Visit(sourceCode);
            visitor.VisitedExpressions.ShouldContain(method1Call);
        }

        [Fact]
        public void ShouldVisitAThisInstanceExpression()
        {
            var sourceCode = SourceCodeFactory.Default.CreateSourceCode();
            var @class = sourceCode.AddClass();
            var classInstance = BuildableExpression.ThisInstance(@class);
            @class.AddMethod(classInstance);

            var visitor = new VisitationHelper();

            visitor.Visit(sourceCode);
            visitor.VisitedExpressions.ShouldContain(classInstance);
        }

        #region Helper Members

        private class VisitationHelper : ExpressionVisitor
        {
            public VisitationHelper()
            {
                VisitedExpressions = new List<Expression>();
            }

            public IList<Expression> VisitedExpressions { get; }

            public bool SourceCodeVisited { get; private set; }

            public bool ClassVisited { get; private set; }

            public bool MethodVisited { get; private set; }

            public override Expression Visit(Expression node)
            {
                VisitedExpressions.Add(node);

                switch (node)
                {
                    case SourceCodeExpression _:
                        SourceCodeVisited = true;
                        break;

                    case ClassExpression _:
                        ClassVisited = true;
                        break;

                    case MethodExpression _:
                        MethodVisited = true;
                        break;
                }

                return base.Visit(node);
            }
        }

        #endregion
    }
}
