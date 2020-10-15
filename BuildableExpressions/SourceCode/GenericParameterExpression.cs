namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;
    using Api;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Reflection;

    /// <summary>
    /// Represents an open class or method generic argument.
    /// </summary>
    public class GenericParameterExpression :
        Expression,
        IGenericParameterNamingContext,
        IGenericParameterExpressionConfigurator,
        IGenericArgument,
        ICustomTranslationExpression
    {
        private string _name;
        private MethodExpression _method;

        /// <summary>
        /// Gets the <see cref="SourceCodeExpressionType"/> value (1004) indicating the type of this
        /// <see cref="GenericParameterExpression"/> as an ExpressionType.
        /// </summary>
        public override ExpressionType NodeType
            => (ExpressionType)SourceCodeExpressionType.GenericArgument;

        /// <summary>
        /// Gets the type of this <see cref="GenericParameterExpression"/>, which is 'void', as this
        /// class represents an open generic argument.
        /// </summary>
        public override Type Type => typeof(void);

        /// <summary>
        /// Visits this <see cref="GenericParameterExpression"/>.
        /// </summary>
        /// <param name="visitor">
        /// The visitor with which to visit this <see cref="GenericParameterExpression"/>.
        /// </param>
        /// <returns>This <see cref="GenericParameterExpression"/>.</returns>
        protected override Expression Accept(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Gets this <see cref="GenericParameterExpression"/>'s parent <see cref="MethodExpression"/>.
        /// </summary>
        public MethodExpression Method
        {
            get => _method;
            internal set
            {
                if (_method == null)
                {
                    _method = value;
                    return;
                }

                throw new InvalidOperationException(
                    $"Unable to add generic parameter to method '{value.Class.Name}.{value.Name}' " +
                    $"as it has already been added to method '{_method.Class.Name}.{_method.Name}'");
            }
        }

        /// <summary>
        /// Gets the name of this <see cref="GenericParameterExpression"/>
        /// </summary>
        public string Name => _name ??= GetName();

        private string GetName()
        {
            return Method.Settings
                .GenericParameterNameFactory
                .Invoke(Method, this)
                .ThrowIfInvalidName<InvalidOperationException>("Generic Parameter");
        }

        #region IGenericParameterNamingContext Members

        int IGenericParameterNamingContext.Index => Method?.GenericArguments.IndexOf(this) ?? 0;

        #endregion

        #region IGenericParameterExpressionConfigurator Members

        IGenericParameterExpressionConfigurator IGenericParameterExpressionConfigurator.Named(
            string name)
        {
            _name = name.ThrowIfInvalidName<ArgumentException>("Generic Parameter");
            return this;
        }

        #endregion

        #region IGenericArgument Members

        string IGenericArgument.TypeName => Name;

        /// <inheritdoc />
        public bool IsClosed => false;

        bool IGenericArgument.HasConstraints { get; }

        bool IGenericArgument.HasClassConstraint { get; }

        bool IGenericArgument.HasStructConstraint { get; }

        bool IGenericArgument.HasNewableConstraint { get; }

        ReadOnlyCollection<Type> IGenericArgument.TypeConstraints { get; }

        #endregion

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
            => GenericArgumentTranslation.For(this, context.Settings);
    }
}