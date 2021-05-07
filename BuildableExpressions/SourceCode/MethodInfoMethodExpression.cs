namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
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
            if (methodInfo.IsAbstract)
            {
                SetAbstract();
            }
            else if (methodInfo.IsVirtual)
            {
                SetVirtual();
            }

            AddParameters(methodInfo.GetParameters());
            SetMethodInfo(methodInfo);
        }

        public override Type ReturnType => MethodInfo.ReturnType;

        public override LambdaExpression Definition
            => _definition ??= CreateDefinition();

        private LambdaExpression CreateDefinition()
            => Default(ReturnType).ToLambdaExpression(ParametersAccessor);

        internal override bool HasBody => true;

        public override bool IsOverride => _isOverride ??= this.IsOverride();

        #region Validation

        protected override IEnumerable<MethodExpressionBase> SiblingMethodExpressions => null;

        #endregion

        internal override void ResetMemberInfo()
        {
        }
    }
}