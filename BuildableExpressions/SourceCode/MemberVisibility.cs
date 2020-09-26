namespace AgileObjects.BuildableExpressions.SourceCode
{
    /// <summary>
    /// Provides options for specifying the visibility of a class member.
    /// </summary>
    public enum MemberVisibility
    {
        /// <summary>
        /// 0. Public - the member will have public accessibility.
        /// </summary>
        Public,

        /// <summary>
        /// 1. Internal - the member will have internal accessibility.
        /// </summary>
        Internal,

        /// <summary>
        /// 2. ProtectedInternal - the member will have protected internal accessibility.
        /// </summary>
        ProtectedInternal,

        /// <summary>
        /// 3. Protected - the member will have protected accessibility.
        /// </summary>
        Protected,

        /// <summary>
        /// 4. Private - the member will have private accessibility.
        /// </summary>
        Private
    }
}