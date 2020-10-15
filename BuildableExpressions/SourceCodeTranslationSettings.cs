namespace AgileObjects.BuildableExpressions
{
    using System;
    using ReadableExpressions;
    using SourceCode;
    using SourceCode.Api;

    internal class SourceCodeTranslationSettings :
        TranslationSettings,
        ISourceCodeTranslationSettings
    {
        public static readonly SourceCodeTranslationSettings Default = Create();

        #region Factory Methods

        public static SourceCodeTranslationSettings Create()
            => SetDefaultSourceCodeOptions(new SourceCodeTranslationSettings());

        public static SourceCodeTranslationSettings SetDefaultSourceCodeOptions(SourceCodeTranslationSettings settings)
        {
            settings.CollectRequiredNamespaces = true;
            settings.CollectInlineBlocks = true;
            settings.ClassNameFactory = (sc, classCtx) => sc.GetClassName(classCtx);
            settings.MethodNameFactory = (sc, cls, methodCtx) => cls.GetMethodName(methodCtx);
            settings.GenericParameterNameFactory = (m, paramCtx) => m.GetGenericParameterName(paramCtx);

            return settings;
        }

        #endregion

        public bool CollectRequiredNamespaces { get; private set; }

        public bool CollectInlineBlocks { get; private set; }

        #region Class Naming

        public ISourceCodeTranslationSettings NameClassesUsing(
            Func<SourceCodeExpression, IClassNamingContext, string> nameFactory)
        {
            return SetClassNamingFactory(nameFactory);
        }

        ISourceCodeTranslationSettings ISourceCodeTranslationSettings.NameClassesUsing(
            Func<IClassNamingContext, string> nameFactory)
        {
            return SetClassNamingFactory((sc, @class) => nameFactory.Invoke(@class));
        }

        private SourceCodeTranslationSettings SetClassNamingFactory(
            Func<SourceCodeExpression, IClassNamingContext, string> nameFactory)
        {
            ClassNameFactory = nameFactory;
            return this;
        }

        public Func<SourceCodeExpression, IClassNamingContext, string> ClassNameFactory { get; private set; }

        #endregion

        #region Method Naming

        public ISourceCodeTranslationSettings NameMethodsUsing(
            Func<SourceCodeExpression, ClassExpression, IMethodNamingContext, string> nameFactory)
        {
            return SetMethodNamingFactory(nameFactory);
        }

        ISourceCodeTranslationSettings ISourceCodeTranslationSettings.NameMethodsUsing(
            Func<ClassExpression, IMethodNamingContext, string> nameFactory)
        {
            return SetMethodNamingFactory(nameFactory);
        }

        ISourceCodeTranslationSettings ISourceCodeTranslationSettings.NameMethodsUsing(
            Func<IMethodNamingContext, string> nameFactory)
        {
            return SetMethodNamingFactory(nameFactory);
        }

        private SourceCodeTranslationSettings SetMethodNamingFactory(
            Func<IMethodNamingContext, string> nameFactory)
        {
            return SetMethodNamingFactory((cCtx, mCtx) => nameFactory.Invoke(mCtx));
        }

        private SourceCodeTranslationSettings SetMethodNamingFactory(
            Func<ClassExpression, IMethodNamingContext, string> nameFactory)
        {
            return SetMethodNamingFactory(
                (sc, cls, ctx) => nameFactory.Invoke(cls, ctx));
        }

        private SourceCodeTranslationSettings SetMethodNamingFactory(
            Func<SourceCodeExpression, ClassExpression, IMethodNamingContext, string> nameFactory)
        {
            MethodNameFactory = nameFactory;
            return this;
        }

        public Func<SourceCodeExpression, ClassExpression, IMethodNamingContext, string> MethodNameFactory
        {
            get;
            private set;
        }

        #endregion

        #region Generic Parameter Naming

        ISourceCodeTranslationSettings ISourceCodeTranslationSettings.NameGenericParametersUsing(
            Func<MethodExpression, IGenericParameterNamingContext, string> nameFactory)
        {
            GenericParameterNameFactory = nameFactory;
            return this;
        }

        public Func<MethodExpression, IGenericParameterNamingContext, string> GenericParameterNameFactory
        {
            get;
            private set;
        }

        #endregion
    }
}
