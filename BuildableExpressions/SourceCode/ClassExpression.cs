﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;
    using Api;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations;
    using Translations;

    /// <summary>
    /// Represents a class in a piece of source code.
    /// </summary>
    public class ClassExpression :
        ConcreteTypeExpression,
        IClassExpressionConfigurator
    {
        internal ClassExpression(
            SourceCodeExpression sourceCode,
            string name,
            Action<IClassExpressionConfigurator> configuration)
            : base(sourceCode, name)
        {
            BaseType = typeof(object);
            configuration.Invoke(this);
            Validate();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is static.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ClassExpression"/> is abstract.
        /// </summary>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets the base type from which this <see cref="ClassExpression"/> derives.
        /// </summary>
        public Type BaseType { get; private set; }

        /// <summary>
        /// Gets the non-object base type and interfaces types implemented by this
        /// <see cref="ClassExpression"/>.
        /// </summary>
        public override IEnumerable<Type> ImplementedTypes
        {
            get
            {
                if (BaseType != typeof(object))
                {
                    yield return BaseType;
                }

                foreach (var @interface in InterfaceTypes)
                {
                    yield return @interface;
                }
            }
        }

        #region IClassExpressionConfigurator Members

        void IClassExpressionConfigurator.SetStatic()
        {
            if (IsAbstract)
            {
                ThrowModifierConflict("abstract", "static");
            }

            IsStatic = true;
        }

        void IClassExpressionConfigurator.SetAbstract() => SetAbstract();

        internal void SetAbstract()
        {
            if (IsStatic)
            {
                ThrowModifierConflict("static", "abstract");
            }

            IsAbstract = true;
        }

        private void ThrowModifierConflict(string modifier, string conflictingModifier)
        {
            throw new InvalidOperationException(
                $"Class '{Name}' cannot be both {modifier} and {conflictingModifier}.");
        }

        void IClassExpressionConfigurator.SetBaseType<TBase>() where TBase : class
            => SetBaseType(typeof(TBase));

        void IClassExpressionConfigurator.SetBaseType(Type baseType)
            => SetBaseType(baseType);

        internal void SetBaseType(Type baseType)
        {
            if (BaseType == typeof(object))
            {
                BaseType = baseType;
                return;
            }

            throw new InvalidOperationException(
                $"Unable to set class base type to {baseType.GetFriendlyName()} " +
                $"as it has already been set to {BaseType.GetFriendlyName()}");
        }

        MethodExpression IClassExpressionConfigurator.AddMethod(
            string name,
            Action<IClassMethodExpressionConfigurator> configuration)
        {
            return AddMethod(name, configuration);
        }

        internal override StandardMethodExpression Add(StandardMethodExpression method)
        {
            if (IsStatic)
            {
                ((IConcreteTypeMethodExpressionConfigurator)method).SetStatic();
            }

            return base.Add(method);
        }

        #endregion

        internal override ITranslation GetTranslation(ITranslationContext context)
            => new ClassTranslation(this, context);
    }
}