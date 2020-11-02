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
        Public = 0,

        /// <summary>
        /// 1. ProtectedInternal - the member will have protected internal accessibility.
        /// </summary>
        ProtectedInternal = 1,

        /// <summary>
        /// 2. Internal - the member will have internal accessibility.
        /// </summary>
        Internal = 2,

        /// <summary>
        /// 3. Protected - the member will have protected accessibility.
        /// </summary>
        Protected = 3,

        /// <summary>
        /// 4. PrivateProtected - the member will have private protected accessibility.
        /// </summary>
        PrivateProtected = 4,

        /// <summary>
        /// 5. Private - the member will have private accessibility.
        /// </summary>
        Private = 5
    }
}