﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using Api;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a struct in a piece of source code.
    /// </summary>
    public class StructExpression :
        ConcreteTypeExpression,
        IStructExpressionConfigurator
    {
        internal StructExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IStructExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            configuration.Invoke(this);
        }

        internal override ITranslation GetTranslation(ITranslationContext context)
            => new StructTranslation(this, context);
    }
}