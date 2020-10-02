namespace AgileObjects.BuildableExpressions.SourceCode.Api
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides information with which to name a <see cref="ClassExpression"/>.
    /// </summary>
    public interface IClassNamingContext
    {
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