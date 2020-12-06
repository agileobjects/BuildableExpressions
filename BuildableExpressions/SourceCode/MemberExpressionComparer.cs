namespace AgileObjects.BuildableExpressions.SourceCode
{
    using System;
    using System.Collections.Generic;

    internal class MemberExpressionComparer : IComparer<MemberExpression>
    {
        public static readonly MemberExpressionComparer Instance =
            new MemberExpressionComparer();

        private static readonly int[] _orderedTypes =
        {
            (int)SourceCodeExpressionType.Field,
            (int)SourceCodeExpressionType.Constructor,
            (int)SourceCodeExpressionType.Property,
            (int)SourceCodeExpressionType.Method
        };

        public int Compare(MemberExpression x, MemberExpression y)
        {
            // ReSharper disable PossibleNullReferenceException
            if (x.NodeType == y.NodeType)
            {
                return x.MemberIndex > y.MemberIndex ? 1 : -1;
            }

            var xTypeIndex = Array.IndexOf(_orderedTypes, (int)x.NodeType);
            var yTypeIndex = Array.IndexOf(_orderedTypes, (int)y.NodeType);
            // ReSharper restore PossibleNullReferenceException

            return xTypeIndex > yTypeIndex ? -1 : 1;
        }
    }
}