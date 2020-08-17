﻿namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Provides information with which to name a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassNamingContext
    {
        /// <summary>
        /// Gets the ExpressionType of the Expression from which this <see cref="IClassNamingContext"/>
        /// was created.
        /// </summary>
        ExpressionType NodeType { get; }

        /// <summary>
        /// Gets the return type of the Expression from which the main method of the class to which
        /// this <see cref="IClassNamingContext"/> relates was created.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets a PascalCase, class-name-friendly translation of the name of the return type of the
        /// Expression from which the main method of the class to which this
        /// <see cref="IClassNamingContext"/> relates was created.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// Gets the Expression from which this <see cref="IClassNamingContext"/> was created.
        /// </summary>
        Expression Body { get; }

        /// <summary>
        /// Gets the <see cref="MethodExpression"/>s which make up this
        /// <see cref="IClassNamingContext"/>'s methods.
        /// </summary>
        ReadOnlyCollection<MethodExpression> Methods { get; }

        /// <summary>
        /// Gets the index of the class in the set of generated classes to which this
        /// <see cref="IClassNamingContext"/> relates.
        /// </summary>
        int Index { get; }
    }
}