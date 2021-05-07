namespace AgileObjects.BuildableExpressions.SourceCode.Extensions
{
    using System;
    using System.Linq;
    using static System.StringComparison;

    internal static class SourceCodeValidationExtensions
    {
        public static string ThrowIfInvalidName(
            this string name,
            string symbolType,
            bool throwIfNull = true)
        {
            if (name == null)
            {
                if (throwIfNull)
                {
                    throw Create<ArgumentNullException>(symbolType + " names cannot be null");
                }

                return null;
            }

            if (name.Trim() == string.Empty)
            {
                throw Create<ArgumentException>(symbolType + " names cannot be blank");
            }

            if (char.IsDigit(name[0]) || name.Any(IsInvalidTypeNameCharacter))
            {
                throw Create<ArgumentException>($"'{name}' is an invalid {symbolType.ToLowerInvariant()} name");
            }

            return name;
        }

        private static bool IsInvalidTypeNameCharacter(char character)
        {
            switch (character)
            {
                case '_':
                case '<':
                case '>':
                    return false;

                default:
                    return !char.IsLetterOrDigit(character);
            }
        }

        private static Exception Create<TException>(string message)
            => (Exception)Activator.CreateInstance(typeof(TException), message);

        public static void ValidateSetStatic<TMember>(this TMember memberExpression)
            where TMember : MemberExpression, IConcreteTypeExpression, IHasSignature
        {
            memberExpression.ThrowIfAbstract("static");
            memberExpression.ThrowIfVirtual("static");
        }

        public static void ValidateSetAbstract<TMember>(
            this TMember memberExpression)
            where TMember : MemberExpression, IConcreteTypeExpression, IHasSignature
        {
            memberExpression.ThrowIfNonAbstractClass();
            memberExpression.ThrowIfStatic("abstract");
            memberExpression.ThrowIfVirtual("abstract");
        }

        private static void ThrowIfNonAbstractClass<TMember>(this TMember memberExpression)
            where TMember : MemberExpression, IHasSignature
        {
            var declaringTypeExpression = memberExpression.DeclaringTypeExpression;

            if (((ClassExpression)declaringTypeExpression).IsAbstract)
            {
                return;
            }

            var memberName = GetMemberTypeName<TMember>();
            var memberTypeName = char.ToLowerInvariant(memberName[0]) + memberName.Substring(1);
            var signature = memberExpression.GetSignature();

            throw new InvalidOperationException(
                $"Unable to add abstract {memberTypeName} '{signature}' " +
                $"to non-abstract declaring type '{declaringTypeExpression.Name}'.");
        }

        public static void ValidateSetVirtual<TMember>(this TMember memberExpression)
            where TMember : MemberExpression, IConcreteTypeExpression, IHasSignature
        {
            memberExpression.ThrowIfStatic("virtual");
            memberExpression.ThrowIfAbstract("virtual");
        }

        private static void ThrowIfStatic<TMember>(
            this TMember memberExpression,
            string conflictingModifier)
            where TMember : MemberExpression, IHasSignature
        {
            if (memberExpression.IsStatic)
            {
                memberExpression.ThrowModifierConflict("static", conflictingModifier);
            }
        }

        private static void ThrowIfAbstract<TMember>(
            this TMember memberExpression,
            string conflictingModifier)
            where TMember : MemberExpression, IConcreteTypeExpression, IHasSignature
        {
            if (memberExpression.IsAbstract)
            {
                memberExpression.ThrowModifierConflict("abstract", conflictingModifier);
            }
        }

        private static void ThrowIfVirtual<TMember>(
            this TMember memberExpression,
            string conflictingModifier)
            where TMember : MemberExpression, IConcreteTypeExpression, IHasSignature
        {
            if (memberExpression.IsVirtual)
            {
                memberExpression.ThrowModifierConflict("virtual", conflictingModifier);
            }
        }

        public static void ThrowModifierConflict<TMember>(
            this TMember memberExpression,
            string modifier,
            string conflictingModifier)
            where TMember : MemberExpression, IHasSignature
        {
            var memberName = GetMemberTypeName<TMember>();

            throw new InvalidOperationException(
                $"{memberName} '{memberExpression.GetSignature()}' " +
                $"cannot be both {modifier} and {conflictingModifier}.");
        }

        private static string GetMemberTypeName<TMember>()
        {
            var memberName = typeof(TMember).Name;
            var memberType = memberName.Substring(0, memberName.Length - "Expression".Length);

            if (memberType.StartsWith("Configured", Ordinal))
            {
                memberType = memberType.Substring("Configured".Length);
            }

            return memberType;
        }
    }
}