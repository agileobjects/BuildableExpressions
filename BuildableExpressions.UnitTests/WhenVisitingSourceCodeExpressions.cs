namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using BuildableExpressions;
    using BuildableExpressions.SourceCode;
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

            var sourceCode = BuildableExpression
                .SourceCode(sc =>
                {
                    sc.AddClass("MyClass", cls =>
                    {
                        cls.SetSummary("This is my class");
                        cls.AddMethod("Get1000", returnOneThousand);
                    });
                });

            var visitor = new VisitationHelper();

            visitor.Visit(sourceCode);

            visitor.VisitedExpressions.ShouldContain(returnOneThousand.Body);
            visitor.SourceCodeVisited.ShouldBeTrue();
            visitor.ClassVisited.ShouldBeTrue();
            visitor.MethodVisited.ShouldBeTrue();
        }

        [Fact]
        public void ShouldVisitABlockMethodCallExpression()
        {
            var blockMethodCall = default(Expression);

            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    var blockMethod = new BlockMethodExpression((TypeExpression)cls, cfg =>
                    {
                        cfg.SetBody(Default(typeof(void)));
                    });

                    cls.AddMethod(blockMethod);

                    blockMethodCall = blockMethod.CallExpression;
                    cls.AddMethod("CallBlockMethod", blockMethodCall);
                });
            });

            blockMethodCall.ShouldNotBeNull();

            var visitor = new VisitationHelper();

            visitor.Visit(sourceCode);
            visitor.VisitedExpressions.ShouldContain(blockMethodCall);
        }

        [Fact]
        public void ShouldVisitAThisInstanceExpression()
        {
            var classInstance = default(Expression);

            var sourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass(cls =>
                {
                    cls.AddMethod("GetThis", m =>
                    {
                        classInstance = cls.ThisInstanceExpression;

                        m.SetBody(classInstance);
                    });
                });
            });

            classInstance.ShouldNotBeNull();

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
