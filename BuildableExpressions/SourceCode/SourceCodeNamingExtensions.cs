﻿namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Linq;

    internal static class SourceCodeNamingExtensions
    {
        public static string ThrowIfInvalidName<TException>(
            this string name,
            string symbolType,
            bool throwIfNull = true)
            where TException : Exception
        {
            if (name == null)
            {
                if (throwIfNull)
                {
                    throw Create<TException>(symbolType + " names cannot be null");
                }

                return null;
            }

            if (name.Trim() == string.Empty)
            {
                throw Create<TException>(symbolType + " names cannot be blank");
            }

            if (char.IsDigit(name[0]) ||
                name.ToCharArray().Any(c => !char.IsLetterOrDigit(c) && c != '_'))
            {
                throw Create<TException>($"'{name}' is an invalid {symbolType.ToLowerInvariant()} name");
            }

            return name;
        }

        private static Exception Create<TException>(string message)
            => (Exception)Activator.CreateInstance(typeof(TException), message);
    }
}