namespace AgileObjects.BuildableExpressions.SourceCode.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using BuildableExpressions.Extensions;
    using Extensions;
    using NetStandardPolyfills;
    using ReadableExpressions.Extensions;
    using ReadableExpressions.Translations.Reflection;
    using static SourceCodeTranslationSettings;

    internal class NamespaceAnalysis
    {
        private List<string> _requiredNamespaces;
        private string _sourceCodeNamespace;

        public IList<string> RequiredNamespaces => _requiredNamespaces;

        public void Visit(TypeExpression type)
        {
            _sourceCodeNamespace = type.SourceCode.Namespace;
            AddNamespacesIfRequired(type.ImplementedTypeExpressions);
        }

        public void Visit(ConstantExpression constant)
        {
            if (constant.Type.IsEnum())
            {
                AddNamespaceIfRequired(constant);
            }
            else if (constant.Type.IsAssignableTo(typeof(Type)))
            {
                AddNamespaceIfRequired((Type)constant.Value);
            }
        }

        public void Visit(DefaultExpression @default) => AddNamespaceIfRequired(@default);

        public void Visit(FieldExpression field) => AddNamespaceIfRequired(field);

        public void Visit(MemberExpression memberAccess)
        {
            if (memberAccess.Expression == null)
            {
                // Static member access
                AddNamespaceIfRequired(memberAccess.Member.DeclaringType);
            }
        }

        public void Visit(MethodCallExpression methodCall)
        {
            if (methodCall.Method.IsGenericMethod)
            {
                AddNamespacesIfRequired(new BclMethodWrapper(methodCall.Method, Settings)
                    .GetRequiredExplicitGenericArguments(Settings));
            }

            if (methodCall.Method.IsStatic)
            {
                AddNamespaceIfRequired(methodCall.Method.DeclaringType);
            }
        }

        public void Visit(MethodExpression method)
        {
            if (method.IsGeneric)
            {
                AddNamespacesIfRequired(method
                    .GenericParameters
                    .Filter(gp => gp.HasConstraints)
                    .SelectMany(gp => gp.TypeConstraints));
            }

            foreach (var parameter in method.Parameters)
            {
                AddNamespaceIfRequired(parameter);
            }

            AddNamespaceIfRequired(method);
        }

        public void Visit(PropertyExpression property)
            => AddNamespaceIfRequired(property);

        public void Visit(NewArrayExpression newArray)
            => AddNamespaceIfRequired(newArray.Type.GetElementType());

        public void Visit(NewExpression newing)
            => AddNamespaceIfRequired(newing);

        public void Visit(CatchBlock @catch)
        {
            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                AddNamespaceIfRequired(catchVariable);
            }
        }

        private void AddNamespacesIfRequired(IEnumerable<IType> accessedTypes)
        {
            foreach (var type in accessedTypes)
            {
                AddNamespaceIfRequired(type);
            }
        }

        private void AddNamespaceIfRequired(Expression expression)
            => AddNamespaceIfRequired(expression.Type);

        private void AddNamespaceIfRequired(Type type)
            => AddNamespaceIfRequired(BclTypeWrapper.For(type));

        private void AddNamespaceIfRequired(IType accessedType)
        {
            if (accessedType.IsPrimitive ||
                accessedType.Equals(BclTypeWrapper.Void) ||
                accessedType.Equals(BclTypeWrapper.String) ||
                accessedType.Equals(BclTypeWrapper.Object) ||
               (accessedType.Namespace == BuildConstants.GenericParameterTypeNamespace))
            {
                return;
            }

            if (accessedType.IsGeneric)
            {
                AddNamespacesIfRequired(accessedType.GenericTypeArguments);
            }

            var @namespace = accessedType.Namespace;

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

        public void Finalise()
        {
            if (_requiredNamespaces == null)
            {
                _requiredNamespaces = Enumerable<string>.EmptyList;
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