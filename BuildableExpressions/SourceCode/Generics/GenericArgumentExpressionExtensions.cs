namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Linq;
    using Api;
    using BuildableExpressions.Extensions;
    using Compilation;
    using static System.Linq.Expressions.Expression;

    internal static class GenericArgumentExpressionExtensions
    {
        public static Type CreateType(
            this ConfiguredGenericParameterExpression parameter)
        {
            var paramSourceCode = BuildableExpression.SourceCode(sc =>
            {
                if (CreateClass(parameter))
                {
                    sc.AddClass(parameter.Name, parameter.ConfigureClass);
                }
                else
                {
                    sc.AddStruct(parameter.Name, parameter.ConfigureStruct);
                }
            });

            var compiledTypes = paramSourceCode
                .Compile()
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
        }

        private static bool CreateClass(ConfiguredGenericParameterExpression parameter)
        {
            if (parameter.HasClassConstraint)
            {
                return true;
            }

            if (parameter.HasStructConstraint)
            {
                return false;
            }

            return parameter.TypeConstraintsAccessor?.Any(t => t.IsClass) == true;
        }

        private static void ConfigureClass(
            this ConfiguredGenericParameterExpression parameter,
            IClassExpressionConfigurator classConfig)
        {
            var @class = (ConfiguredClassExpression)classConfig;

            parameter.ConfigureType(@class, (cls, baseType) =>
            {
                cls.SetBaseType(baseType);

                if (baseType.IsAbstract)
                {
                    cls.SetAbstract();
                }
            });
        }

        private static void ConfigureStruct(
            this ConfiguredGenericParameterExpression parameter,
            IStructExpressionConfigurator structConfig)
        {
            parameter.ConfigureType((StructExpression)structConfig, baseTypeCallback: null);
        }

        private static void ConfigureType<TTypeExpression>(
            this ConfiguredGenericParameterExpression parameter,
            TTypeExpression typeExpression,
            Action<TTypeExpression, ClassExpression> baseTypeCallback)
            where TTypeExpression : TypeExpression
        {
            if (parameter.TypeConstraintsAccessor == null)
            {
                return;
            }

            foreach (var typeConstraintExpression in parameter.TypeConstraintsAccessor)
            {
                if (typeConstraintExpression.IsClass)
                {
                    baseTypeCallback.Invoke(typeExpression, (ClassExpression)typeConstraintExpression);
                    continue;
                }

                AddDefaultImplementations(typeExpression, typeConstraintExpression);
                typeExpression.SetImplements((InterfaceExpression)typeConstraintExpression);
            }
        }

        private static void AddDefaultImplementations(
            TypeExpression typeExpression,
            TypeExpression typeConstraintExpression)
        {
            var methodExpressions = typeConstraintExpression
                .MethodExpressions
                .Filter(m => m.IsAbstract);

            foreach (var methodExpression in methodExpressions)
            {
                var methodLambda =
                    Default(methodExpression.ReturnType)
                        .ToLambdaExpression(methodExpression.Parameters);

                typeExpression.AddMethod(methodExpression.Name, m => m.SetBody(methodLambda));
            }
        }
    }
}