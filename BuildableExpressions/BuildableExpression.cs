namespace AgileObjects.BuildableExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using SourceCode;
    using SourceCode.Extensions;

    /// <summary>
    /// Provides buildable Expression factory methods.
    /// </summary>
    public static class BuildableExpression
    {
        /// <summary>
        /// Create a <see cref="ThisInstanceExpression"/> that represents the instance of an object
        /// to which the 'this' keyword relates in the current context.
        /// </summary>
        /// <param name="class">The <see cref="ClassExpression"/> representing the instance Type.</param>
        /// <returns>The <see cref="ThisInstanceExpression"/> representing the object instance.</returns>
        public static ThisInstanceExpression ThisInstance(ClassExpression @class)
            => new ThisInstanceExpression(@class);

        /// <summary>
        /// Create a <see cref="BuildableMethodCallExpression"/> that represents a call to the given
        /// parameterless <paramref name="method"/>. If the method is instance scoped, it will be
        /// called on the current context's object (i.e. 'this.Method()').
        /// </summary>
        /// <param name="method">The <see cref="MethodExpression"/> representing the method to call.</param>
        /// <returns>The <see cref="BuildableMethodCallExpression"/> representing the method call.</returns>
        public static BuildableMethodCallExpression Call(MethodExpression method)
            => Call(method, Enumerable<Expression>.EmptyArray);

        /// <summary>
        /// Create a <see cref="BuildableMethodCallExpression"/> that represents a call to the given
        /// <paramref name="method"/>. If the method is instance scoped, it will be called on the
        /// current context's object (i.e. 'this.Method(arguments)').
        /// </summary>
        /// <param name="method">The <see cref="MethodExpression"/> representing the method to call.</param>
        /// <param name="arguments">Expressions representing the method call arguments.</param>
        /// <returns>The <see cref="BuildableMethodCallExpression"/> representing the method call.</returns>
        public static BuildableMethodCallExpression Call(
            MethodExpression method,
            IEnumerable<Expression> arguments)
        {
            return Call(method, arguments.ToArray());
        }

        /// <summary>
        /// Create a <see cref="BuildableMethodCallExpression"/> that represents a call to the given
        /// <paramref name="method"/>. If the method is instance scoped, it will be called on the
        /// current context's object (i.e. 'this.Method(arguments)').
        /// </summary>
        /// <param name="method">The <see cref="MethodExpression"/> representing the method to call.</param>
        /// <param name="arguments">One or more Expressions representing the method call arguments.</param>
        /// <returns>The <see cref="BuildableMethodCallExpression"/> representing the method call.</returns>
        public static BuildableMethodCallExpression Call(
            MethodExpression method,
            params Expression[] arguments)
        {
            ThrowIfParameterCountMismatch(method, arguments);
            ThrowIfParameterTypeMismatch(method, arguments);

            var thisInstance = ThisInstance(method.Class);

            return new BuildableMethodCallExpression(thisInstance, method, arguments);
        }

        private static void ThrowIfParameterCountMismatch(
            MethodExpression method,
            ICollection<Expression> arguments)
        {
            if (method.Parameters.Count != arguments.Count)
            {
                throw new ArgumentException(
                    $"Expected {method.Parameters.Count} argument(s), got {arguments.Count}");
            }
        }

        private static void ThrowIfParameterTypeMismatch(
            MethodExpression method,
            IList<Expression> arguments)
        {
            var typedMismatchParameter = method
                .Parameters
                .Select((p, i) => new { p.Type, Index = i })
                .FirstOrDefault(_ => !arguments[_.Index].Type.IsAssignableTo(_.Type));

            if (typedMismatchParameter != null)
            {
                throw new ArgumentException(
                    $"Parameter {typedMismatchParameter.Index + 1} requires an Expression of Type " +
                    $"'{typedMismatchParameter.Type.GetFriendlyName()}' - the supplied argument is " +
                    $"of Type '{arguments[typedMismatchParameter.Index].Type.GetFriendlyName()}'");
            }
        }
    }
}
