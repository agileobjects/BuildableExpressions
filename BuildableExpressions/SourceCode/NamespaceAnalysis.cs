namespace AgileObjects.BuildableExpressions.SourceCode
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

    internal class NamespaceAnalysis : ExpressionVisitor
    {
        private List<string> _requiredNamespaces;

        public NamespaceAnalysis(SourceCodeTranslationSettings settings)
        {
            Settings = settings;
        }

        public static NamespaceAnalysis For(
            Expression expression,
            SourceCodeTranslationSettings settings)
        {
            var analysis = new NamespaceAnalysis(settings);
            analysis.Visit(expression);
            analysis.Finalise();

            return analysis;
        }

        public SourceCodeTranslationSettings Settings { get; }

        public IList<string> RequiredNamespaces => _requiredNamespaces;

        public void Visit(ClassExpression @class)
            => AddNamespacesIfRequired(@class.Interfaces);

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            Visit(constant);
            return base.VisitConstant(constant);
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

        protected override Expression VisitDefault(DefaultExpression @default)
        {
            Visit(@default);
            return base.VisitDefault(@default);
        }

        public void Visit(DefaultExpression @default) => AddNamespaceIfRequired(@default);

        protected override Expression VisitMember(MemberExpression memberAccess)
        {
            Visit(memberAccess);
            return base.VisitMember(memberAccess);
        }

        public void Visit(MemberExpression memberAccess)
        {
            if (memberAccess.Expression == null)
            {
                // Static member access
                AddNamespaceIfRequired(memberAccess.Member.DeclaringType);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            Visit(methodCall);
            return base.VisitMethodCall(methodCall);
        }

        public void Visit(MethodCallExpression methodCall)
        {
            if (methodCall.Method.IsGenericMethod)
            {
                AddNamespacesIfRequired(new BclMethodWrapper(methodCall.Method, Settings)
                    .GetRequiredExplicitGenericArguments(Settings)
                    .Project(arg => arg.Type));
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
                    .GenericArguments
                    .Cast<IGenericArgument>()
                    .Filter(ga => ga.HasConstraints)
                    .SelectMany(ga => ga.TypeConstraints));
            }

            foreach (var parameter in method.Parameters)
            {
                AddNamespaceIfRequired(parameter);
            }

            AddNamespaceIfRequired(method);
        }

        protected override Expression VisitNewArray(NewArrayExpression newArray)
        {
            Visit(newArray);
            return base.VisitNewArray(newArray);
        }

        public void Visit(NewArrayExpression newArray)
            => AddNamespaceIfRequired(newArray.Type.GetElementType());

        protected override Expression VisitNew(NewExpression newing)
        {
            Visit(newing);
            return base.VisitNew(newing);
        }

        public void Visit(NewExpression newing)
            => AddNamespaceIfRequired(newing.Type);

        protected override CatchBlock VisitCatchBlock(CatchBlock @catch)
        {
            Visit(@catch);
            return base.VisitCatchBlock(@catch);
        }

        public void Visit(CatchBlock @catch)
        {
            var catchVariable = @catch.Variable;

            if (catchVariable != null)
            {
                AddNamespaceIfRequired(catchVariable);
            }
        }

        private void AddNamespacesIfRequired(IEnumerable<Type> accessedTypes)
        {
            foreach (var type in accessedTypes)
            {
                AddNamespaceIfRequired(type);
            }
        }

        private void AddNamespaceIfRequired(Expression expression)
            => AddNamespaceIfRequired(expression.Type);

        private void AddNamespaceIfRequired(Type accessedType)
        {
            if (!Settings.CollectRequiredNamespaces ||
                (accessedType == typeof(void)) ||
                (accessedType == typeof(string)) ||
                (accessedType == typeof(object)) ||
                 accessedType.IsPrimitive() ||
                (accessedType.Namespace == BuildConstants.GenericParameterTypeNamespace))
            {
                return;
            }

            if (accessedType.IsGenericType())
            {
                AddNamespacesIfRequired(accessedType.GetGenericTypeArguments());
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

        public void Merge(NamespaceAnalysis otherAnalysis)
        {
            if ((otherAnalysis._requiredNamespaces?.Any()) != true)
            {
                return;
            }

            if (_requiredNamespaces?.Any() != true)
            {
                _requiredNamespaces = new List<string>();
            }

            _requiredNamespaces.AddRange(otherAnalysis._requiredNamespaces
                .Except(_requiredNamespaces));
        }

        public void Finalise()
        {
            if (_requiredNamespaces != null)
            {
                _requiredNamespaces.Sort(UsingsComparer.Instance);
            }
            else if (Settings.CollectRequiredNamespaces)
            {
                _requiredNamespaces = Enumerable<string>.EmptyList;
            }
        }
    }
}