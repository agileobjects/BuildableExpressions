namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Reflection;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;

    internal class MethodInfoMethodExpression : MethodExpression
    {
        private bool? _isOverride;

        public MethodInfoMethodExpression(
            TypeExpression declaringType,
            MethodInfo methodInfo)
            : base(declaringType, methodInfo.Name)
        {
            MethodInfo = methodInfo;

            if (methodInfo.IsAbstract)
            {
                SetAbstract();
            }

            var parameters = methodInfo
                .GetParameters()
                .ProjectToArray(p => Parameter(p.ParameterType, p.Name));

            var methodLambda =
                Default(methodInfo.ReturnType).ToLambdaExpression(parameters);

            SetBody(methodLambda, methodInfo.ReturnType);
        }

        public override MethodInfo MethodInfo { get; }

        internal override bool HasGeneratedName => false;

        internal override bool HasBody => true;

        public override bool IsOverride => _isOverride ??= this.IsOverride();
    }
}