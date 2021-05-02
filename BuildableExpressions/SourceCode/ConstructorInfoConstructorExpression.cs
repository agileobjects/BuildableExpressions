namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class ConstructorInfoConstructorExpression : ConstructorExpression
    {
        public ConstructorInfoConstructorExpression(
            TypeExpression declaringType,
            ConstructorInfo ctorInfo)
            : base(declaringType)
        {
            ConstructorInfo = ctorInfo;
            AddParameters(ctorInfo.GetParameters());
        }

        public override ConstructorInfo ConstructorInfo { get; }

        #region Validation

        protected override IEnumerable<MethodExpressionBase> SiblingMethodExpressions => null;

        #endregion
    }
}