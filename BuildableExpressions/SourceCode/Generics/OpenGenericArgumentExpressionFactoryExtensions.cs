namespace AgileObjects.BuildableExpressions.SourceCode.Generics
{
    using System;
    using System.Linq;
    using Api;
    using BuildableExpressions.Extensions;
    using Compilation;
    using NetStandardPolyfills;
    using ReadableExpressions.Translations.Reflection;
    using static System.Linq.Expressions.Expression;

    internal static class OpenGenericArgumentExpressionFactoryExtensions
    {
        public static Type ToType(this OpenGenericArgumentExpression parameter)
        {
            var paramSourceCode = BuildableExpression.SourceCode(sc =>
            {
                sc.SetNamespace(BuildConstants.GenericParameterTypeNamespace);

                if (((IGenericArgument)parameter).HasStructConstraint)
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
            this OpenGenericArgumentExpression parameter,
            IStructExpressionConfigurator structConfig)
        {
            parameter.ConfigureType((StructExpression)structConfig, baseTypeCallback: null);
        }

        private static void ConfigureClass(
            this OpenGenericArgumentExpression parameter,
            IClassExpressionConfigurator classConfig)
        {
            var @class = (ClassExpression)classConfig;

            parameter.ConfigureType(@class, (cfg, baseType) =>
            {
                cfg.SetBaseType(baseType);

                if (baseType.IsAbstract())
                {
                    cfg.SetAbstract();
                }
            });
        }

        private static void ConfigureType<TTypeExpression>(
            this OpenGenericArgumentExpression parameter,
            TTypeExpression typeExpression,
            Action<TTypeExpression, Type> baseTypeCallback)
            where TTypeExpression : TypeExpression
        {
            if (parameter.TypeConstraintsAccessor == null)
            {
                return;
            }

            foreach (var type in parameter.TypeConstraintsAccessor)
            {
                if (type.IsInterface())
                {
                    AddDefaultImplementations(typeExpression, type);
                    typeExpression.SetImplements(type);
                    continue;
                }

                baseTypeCallback.Invoke(typeExpression, type);
            }
        }

        private static void AddDefaultImplementations(
            TypeExpression typeExpression,
            Type type)
        {
            var methods = type
                .GetPublicInstanceMethods()
                .Concat(type.GetNonPublicInstanceMethods())
                .Filter(m => m.IsAbstract);

            foreach (var method in methods)
            {
                var parameters = method
                    .GetParameters()
                    .ProjectToArray(p => Parameter(p.ParameterType, p.Name));

                var methodLambda = Default(method.ReturnType).ToLambdaExpression(parameters);

                typeExpression.AddMethod(method.Name, methodLambda);
            }
        }

        #endregion
    }
}