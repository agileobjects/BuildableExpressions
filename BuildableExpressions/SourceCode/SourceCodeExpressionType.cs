﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    /// <summary>
    /// Defines ExpressionType value options for source code Expressions.
    /// </summary>
    public enum SourceCodeExpressionType
    {
        /// <summary>
        /// 1000. A piece of source code.
        /// </summary>
        SourceCode = 1000,

        /// <summary>
        /// 1001. A source code Type.
        /// </summary>
        Type = 1001,

        /// <summary>
        /// 1002. The instance of an object to which the 'this' keyword relates in the current
        /// context.
        /// </summary>
        ThisInstance = 1002,

        /// <summary>
        /// 1003. A source code class method.
        /// </summary>
        Method = 1003,

        /// <summary>
        /// 1004. A source code class or method generic argument.
        /// </summary>
        GenericArgument = 1004
    }
}
