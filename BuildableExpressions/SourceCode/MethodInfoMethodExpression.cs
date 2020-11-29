namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using BuildableExpressions.Extensions;
    using ReadableExpressions.Extensions;

    internal class MethodInfoMethodExpression : MethodExpression
    {
        private LambdaExpression _definition;
        private bool? _isOverride;

        public MethodInfoMethodExpression(
            TypeExpression declaringType,
            MethodInfo methodInfo)
            : base(declaringType, methodInfo.Name)
        {
            MethodInfo = methodInfo;
        }

        public override MethodInfo MethodInfo { get; }

        public override Type ReturnType => MethodInfo.ReturnType;

        public override LambdaExpression Definition
            => _definition ??= CreateDefinition();

        private LambdaExpression CreateDefinition()
        {
            var parameters = MethodInfo
                .GetParameters()
                .ProjectToArray(p => Parameter(p.ParameterType, p.Name));

            AddParameters(parameters);

            return Default(ReturnType).ToLambdaExpression(parameters);
        }

        internal override bool HasGeneratedName => false;

        internal override bool HasBody => true;

        public override bool IsAbstract => MethodInfo.IsAbstract;

        public override bool IsVirtual => MethodInfo.IsVirtual;

        public override bool IsOverride => _isOverride ??= this.IsOverride();
    }
}