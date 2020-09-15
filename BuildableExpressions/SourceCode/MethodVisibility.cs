namespace AgileObjects.BuildableExpressions.SourceCode
{
    /// <summary>
    /// Provides options for specifying the visibility of a Method.
    /// </summary>
    public enum MethodVisibility
    {
        /// <summary>
        /// 0. Public - the method will have public accessibility.
        /// </summary>
        Public,

        /// <summary>
        /// 1. Internal - the method will have internal accessibility.
        /// </summary>
        Internal,

        /// <summary>
        /// 2. ProtectedInternal - the method will have protected internal accessibility.
        /// </summary>
        ProtectedInternal,

        /// <summary>
        /// 3. Protected - the method will have protected accessibility.
        /// </summary>
        Protected,

        /// <summary>
        /// 4. Private - the method will have private accessibility.
        /// </summary>
        Private
    }
}