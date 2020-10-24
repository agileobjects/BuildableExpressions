namespace AgileObjects.BuildableExpressions.Compilation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using NetStandardPolyfills;
    using ReadableExpressions;

    /// <summary>
    /// Exposes C# compilation services.
    /// </summary>
    public static class CSharpCompiler
    {
        /// <summary>
        /// Gets the base set of Assemblies used for C# code compilations. These assemblies and any
        /// others added to this collection are included in every <see cref="ICSharpCompiler"/>
        /// compilation. By default, this includes the Assemblies defining System.Object,
        /// System.Collections.Generic.List{T} and System.Linq.Enumerable.
        /// </summary>
        public static readonly List<Assembly> CompilationAssemblies = new List<Assembly>
        {
            typeof(object).GetAssembly(),
#if NET_STANDARD
            typeof(List<>).GetAssembly(),
#endif
            typeof(Enumerable).GetAssembly(),
            typeof(AssemblyExtensionsPolyfill).GetAssembly(),
            typeof(ReadableExpression).GetAssembly(),
            typeof(BuildableExpression).GetAssembly()
        };

        /// <summary>
        /// Gets the Singleton <see cref="ICSharpCompiler"/> instance.
        /// </summary>
        public static readonly ICSharpCompiler Instance =
#if NETFRAMEWORK
            new NetFrameworkCSharpCompiler();
#else
            new NetStandardCSharpCompiler();
#endif
    };
}
