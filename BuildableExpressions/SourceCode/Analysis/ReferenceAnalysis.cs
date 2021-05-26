namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BuildableExpressions.Extensions;
    using Extensions;
    using Generics;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;
    using static SourceCodeConstants;
    using static SourceCodeTranslationSettings;

    internal class ReferenceAnalysis
    {
        private List<Assembly> _requiredAssemblies;
        private List<string> _requiredNamespaces;
        private string _sourceCodeNamespace;

        public IList<Assembly> RequiredAssemblies => _requiredAssemblies;

        public IList<string> RequiredNamespaces => _requiredNamespaces;

        public void Visit(TypeExpression type)
        {
            _sourceCodeNamespace ??= type.SourceCode.Namespace;

            if (type.AttributesAccessor != null)
            {
                HandleReferences(type.AttributesAccessor);
            }

            if (type.IsGeneric)
            {
                HandleReferences(type.GenericParameters);
            }

            HandleReferences(type.ImplementedTypeExpressions);
        }

        public void Visit(ConstantExpression constant)
        {
            if (constant.Type.IsAssignableTo(typeof(Type)))
            {
                HandleReference((Type)constant.Value);
            }
            else if (constant.Type != typeof(MethodInfo))
            {
                HandleReference(constant);
            }
        }

        public void Visit(DefaultExpression @default) => HandleReference(@default);

        public FieldExpression Visit(FieldExpression field) => HandleReference(field);

        public void Visit(MemberExpression memberAccess)
        {
            if (memberAccess.Expression == null)
            {
                // Static member access
                HandleReference(memberAccess.Member.DeclaringType);
            }
        }

        public void Visit(MethodCallExpression methodCall)
        {
            if (methodCall.Method.IsGenericMethod)
            {
                HandleReferences(new ClrMethodWrapper(methodCall.Method, Settings)
                    .GetRequiredExplicitGenericArguments(Settings));
            }

            if (methodCall.Method.IsStatic)
            {
                HandleReference(methodCall.Method.DeclaringType);
            }
        }

        public void Visit(MethodExpressionBase method)
        {
            if (method.IsGeneric)
            {
                HandleReferences(method.GenericParameters);
            }

            foreach (var parameter in method.Parameters)
            {
                Visit(parameter);
            }

            HandleReference(method);
        }

        public PropertyExpression Visit(PropertyExpression property)
            => HandleReference(property);

        public void Visit(NewArrayExpression newArray)
            => HandleReference(newArray.Type.GetElementType());

        public void Visit(NewExpression newing) => HandleReference(newing);

        public void Visit(CatchBlock @catch)
        {
            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                Visit(catchVariable);
            }
            else if (@catch.Test != typeof(Exception))
            {
                HandleReference(@catch.Test);
            }
        }

        private void HandleReferences(IEnumerable<AppliedAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                HandleReference(attribute.AttributeExpression);

                if (attribute.ArgumentsAccessor == null)
                {
                    continue;
                }

                foreach (var argument in attribute.ArgumentsAccessor)
                {
                    HandleReference(argument);
                }
            }
        }

        private void HandleReferences(IEnumerable<GenericParameterExpression> genericParameters)
        {
            HandleReferences(genericParameters
                .Filter(gp => gp.HasConstraints)
                .SelectMany(gp => gp.TypeConstraints));
        }

        private void HandleReferences(IEnumerable<IType> accessedTypes)
        {
            foreach (var type in accessedTypes)
            {
                HandleReference(type);
            }
        }

        public void Visit(Expression expression) => HandleReference(expression);

        private TExpression HandleReference<TExpression>(TExpression expression)
            where TExpression : Expression
        {
            HandleReference(expression.Type);
            return expression;
        }

        private void HandleReference(Type type) => HandleReference(ClrTypeWrapper.For(type));

        private void HandleReference(IType referencedType)
        {
            if (referencedType.IsPrimitive ||
                referencedType.IsNullable ||
                referencedType.IsArray ||
                referencedType.Equals(ClrTypeWrapper.Void) ||
                referencedType.Equals(ClrTypeWrapper.String) ||
                referencedType.Equals(ClrTypeWrapper.Object) ||
               (referencedType.Namespace == GenericParameterTypeNamespace))
            {
                return;
            }

            if (referencedType.IsGeneric)
            {
                HandleReferences(referencedType.GenericTypeArguments);
            }

            AddAssemblyFor(referencedType);

            var @namespace = referencedType.Namespace;

            if ((@namespace == null) || (@namespace == _sourceCodeNamespace))
            {
                return;
            }

            _requiredNamespaces ??= new List<string>();

            if (!_requiredNamespaces.Contains(@namespace))
            {
                _requiredNamespaces.Add(@namespace);
            }
        }

        private void AddAssemblyFor(IType referencedType)
        {
            var assembly = referencedType is TypeExpression typeExpression
                ? typeExpression.ClrTypeAssembly
                : referencedType.Assembly;

            if (assembly == null)
            {
                return;
            }

            _requiredAssemblies ??= new List<Assembly>();

            if (!_requiredAssemblies.Contains(assembly))
            {
                _requiredAssemblies.Add(assembly);
            }
        }

        public void Finalise()
        {
            _requiredAssemblies ??= Enumerable<Assembly>.EmptyList;
            _requiredNamespaces ??= Enumerable<string>.EmptyList;

            if (_requiredNamespaces.Count == 0)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_sourceCodeNamespace))
            {
                _requiredNamespaces.Remove(_sourceCodeNamespace);
            }

            _requiredNamespaces.Sort(UsingsComparer.Instance);
        }
    }
}