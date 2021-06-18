namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Extensions;
    using ReadableExpressions.Translations;
    using ReadableExpressions.Translations.Formatting;

    internal class LiteralSourceCodeExpression :
        SourceCodeExpression,
        ICustomTranslationExpression
    {
        private readonly string _sourceCode;
        private static readonly Regex _namespaceMatcher = new(@"namespace[\s]+(?<Namespace>[\S]+)");
        private static readonly Regex _typeMatcher = new(@"(?:class|struct|interface)[\s]+(?<TypeName>[\S]+)");

        private LiteralSourceCodeExpression(
            string @namespace,
            string typeName,
            string sourceCode)
            : base(@namespace)
        {
            _sourceCode = sourceCode;
            TypeName = typeName;
        }

        #region Factory Method

        public static SourceCodeExpression Parse(string sourceCode)
        {
            var typeNameMatch = _typeMatcher.Match(sourceCode);

            if (!typeNameMatch.Success)
            {
                throw new NotSupportedException();
            }

            var typeName = typeNameMatch.Groups["TypeName"].Value;

            var namespaceMatch = _namespaceMatcher.Match(sourceCode);

            var @namespace = namespaceMatch.Success
                ? namespaceMatch.Groups["Namespace"].Value
                : string.Empty;

            return new LiteralSourceCodeExpression(
                @namespace,
                typeName,
                sourceCode.Trim());
        }

        #endregion

        internal override bool IsComplete => true;

        public override ReadOnlyCollection<Assembly> ReferencedAssemblies
            => Enumerable<Assembly>.EmptyReadOnlyCollection;

        public override ReadOnlyCollection<TypeExpression> TypeExpressions
            => Enumerable<TypeExpression>.EmptyReadOnlyCollection;

        public override string TypeName { get; }

        public override string ToSourceCodeString() => _sourceCode;

        ITranslation ICustomTranslationExpression.GetTranslation(ITranslationContext context)
        {
            return new FixedValueTranslation(
                NodeType,
                _sourceCode,
                Type,
                TokenType.Default,
                context);
        }
    }
}