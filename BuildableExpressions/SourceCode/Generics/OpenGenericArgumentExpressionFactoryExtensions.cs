namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using Api;
    using BuildableExpressions.Extensions;
    using Compilation;

    internal static class OpenGenericArgumentExpressionFactoryExtensions
    {
        public static Type CreateType(
            this ConfiguredOpenGenericParameterExpression parameter)
        {
            var paramSourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace(BuildConstants.GenericParameterTypeNamespace);

                if (parameter.HasStructConstraint)
                {
                    sc.AddStruct(parameter.Name, parameter.ConfigureStruct);
                }
                else
                {
                    sc.AddClass(parameter.Name, parameter.ConfigureClass);
                }
            });

            var compiledTypes = paramSourceCode
                .Compile()
                .CompiledAssembly
                .GetTypes();

            return compiledTypes[0];
        }

        #region ToType Helpers

        private static void ConfigureStruct(
            this ConfiguredOpenGenericParameterExpression parameter,
            IStructExpressionConfigurator structConfig)
        {
            parameter.ConfigureType((StructExpression)structConfig, baseTypeCallback: null);
        }

        private static void ConfigureClass(
            this ConfiguredOpenGenericParameterExpression parameter,
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

        private static void ConfigureType<TTypeExpression>(
            this ConfiguredOpenGenericParameterExpression parameter,
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
                typeExpression.AddMethod(methodExpression);
            }
        }

        #endregion
    }
}