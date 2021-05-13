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
            _sourceCodeNamespace = type.SourceCode.Namespace;

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

        public void Visit(FieldExpression field) => HandleReference(field);

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

        public void Visit(PropertyExpression property) => HandleReference(property);

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

        private void HandleReference(Expression expression) => HandleReference(expression.Type);

        private void HandleReference(Type type) => HandleReference(ClrTypeWrapper.For(type));

        private void HandleReference(IType referencedType)
        {
            if (referencedType.IsPrimitive ||
                referencedType.IsNullable ||
                referencedType.Equals(ClrTypeWrapper.Void) ||
                referencedType.Equals(ClrTypeWrapper.String) ||
                referencedType.Equals(ClrTypeWrapper.Object) ||
               (referencedType.Namespace == BuildConstants.GenericParameterTypeNamespace))
            {
                return;
            }

            if (referencedType.IsGeneric)
            {
                HandleReferences(referencedType.GenericTypeArguments);
            }

#if NETFRAMEWORK
            AddAssembliesFor(referencedType);
#else
            AddAssemblyFor(referencedType);
#endif
            var @namespace = referencedType.Namespace;

            if (@namespace == null)
            {
                return;
            }

            _requiredNamespaces ??= new List<string>();

            if (!_requiredNamespaces.Contains(@namespace))
            {
                _requiredNamespaces.Add(@namespace);
            }
        }

#if NETFRAMEWORK
        private void AddAssembliesFor(IType referencedType)
        {
            while (true)
            {
                AddAssemblyFor(referencedType);

                // ReSharper disable once PossibleNullReferenceException
                var baseType = referencedType.BaseType;

                while (baseType != null && !baseType.Equals(ClrTypeWrapper.Object))
                {
                    AddAssemblyFor(baseType);
                    baseType = baseType.BaseType;
                }

                if (!referencedType.IsNested)
                {
                    return;
                }

                referencedType = referencedType.DeclaringType;
            }
        }
#endif
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