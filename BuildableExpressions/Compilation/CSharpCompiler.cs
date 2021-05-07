﻿namespace AgileObjects.BuildableExpressions.Compilation
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
#if !FEATURE_ASSEMBLY_LOAD_FROM_BYTES
    using System.Runtime.Loader;
#endif
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NetStandardPolyfills;
    using ReadableExpressions;
    using static Microsoft.CodeAnalysis.OutputKind;

    /// <summary>
    /// Exposes C# compilation services.
    /// </summary>
    public static class CSharpCompiler
    {
        /// <summary>
        /// Gets the base set of Assemblies used for C# code compilations. These assemblies and any
        /// others added to this collection are included in every compilation. By default, this
        /// includes the Assemblies defining System.Object, System.Collections.Generic.List{T} and
        /// System.Linq.Enumerable.
        /// </summary>
        public static readonly List<Assembly> CompilationAssemblies = CollectReferencedAssemblies(new[]
        {
            typeof(object).GetAssembly(),
#if NETSTANDARD
            typeof(List<>).GetAssembly(),
#endif
            typeof(Enumerable).GetAssembly(),
            typeof(AssemblyExtensionsPolyfill).GetAssembly(),
            typeof(ReadableExpression).GetAssembly(),
            typeof(BuildableExpression).GetAssembly()
        });

        private static readonly ConcurrentDictionary<string, MetadataReference> _references =
            new ConcurrentDictionary<string, MetadataReference>();

        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(params string[] cSharpSourceCodes)
            => Compile(cSharpSourceCodes.AsEnumerable());

        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/>, returning a
        /// <see cref="CompilationResult"/> describing the results.
        /// </summary>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(IEnumerable<string> cSharpSourceCodes)
            => Compile(Array.Empty<Assembly>(), cSharpSourceCodes);

        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/> using the given
        /// <paramref name="referenceAssemblies"/>, returning a <see cref="CompilationResult"/>
        /// describing the results.
        /// </summary>
        /// <param name="referenceAssemblies">
        /// Zero or more Assemblies required for the compilation. The Assemblies in the
        /// <see cref="CompilationAssemblies"/> collection are automatically included in compilation
        /// and do not need to be passed. By default, this includes the Assemblies defining
        /// System.Object, System.Collections.Generic.List{T} and System.Linq.Enumerable.
        /// </param>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            IEnumerable<Assembly> referenceAssemblies,
            params string[] cSharpSourceCodes)
        {
            return Compile(referenceAssemblies, cSharpSourceCodes.AsEnumerable());
        }

        /// <summary>
        /// Compiles the given <paramref name="cSharpSourceCodes"/> using the given
        /// <paramref name="referenceAssemblies"/>, returning a <see cref="CompilationResult"/>
        /// describing the results.
        /// </summary>
        /// <param name="referenceAssemblies">
        /// Zero or more Assemblies required for the compilation. The Assemblies in the
        /// <see cref="CompilationAssemblies"/> collection are automatically included in compilation
        /// and do not need to be passed. By default, this includes the Assemblies defining
        /// System.Object, System.Collections.Generic.List{T} and System.Linq.Enumerable.
        /// </param>
        /// <param name="cSharpSourceCodes">One or more complete C# source codes to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> describing the result of the compilation.</returns>
        public static CompilationResult Compile(
            IEnumerable<Assembly> referenceAssemblies,
            IEnumerable<string> cSharpSourceCodes)
        {
            var assemblyReferences = CreateReferences(referenceAssemblies);
            var sourceTrees = cSharpSourceCodes.Select(s => SyntaxFactory.ParseSyntaxTree(s));

            using var outputStream = new MemoryStream();

            var compilationResult = CSharpCompilation
                .Create("BuildableExpOutput_" + Path.GetFileNameWithoutExtension(Path.GetTempFileName()))
                .WithOptions(new CSharpCompilationOptions(DynamicallyLinkedLibrary))
                .AddReferences(assemblyReferences)
                .AddSyntaxTrees(sourceTrees)
                .Emit(outputStream);

            if (!compilationResult.Success)
            {
                return new CompilationResult
                {
                    Errors = compilationResult
                        .Diagnostics
                        .Select(ce => $"Error ({ce.Id}): {ce.GetMessage()}, Line {ce.Location.GetLineSpan()}")
                        .ToList()
                };
            }

            outputStream.Position = 0;

            var compiledAssembly =
#if FEATURE_ASSEMBLY_LOAD_FROM_BYTES
                Assembly.Load(outputStream.ToArray());
#else
                AssemblyLoadContext.Default.LoadFromStream(outputStream);
#endif
            return new CompilationResult { CompiledAssembly = compiledAssembly };
        }

        private static IEnumerable<MetadataReference> CreateReferences(
            IEnumerable<Assembly> passedInAssemblies)
        {
            return CompilationAssemblies
                .Concat(CollectReferencedAssemblies(passedInAssemblies))
                .Distinct()
                .Select(CreateReference);
        }

        private static List<Assembly> CollectReferencedAssemblies(
            IEnumerable<Assembly> referencedAssemblies)
        {
            var requiredAssemblies = new List<Assembly>();

            foreach (var assembly in referencedAssemblies)
            {
                CollectReferencedAssemblies(assembly, requiredAssemblies);
            }

            return requiredAssemblies;
        }

        private static void CollectReferencedAssemblies(
            Assembly assembly,
            ICollection<Assembly> assemblies)
        {
            if (string.IsNullOrEmpty(assembly.Location) ||
                assemblies.Contains(assembly))
            {
                return;
            }

            assemblies.Add(assembly);

#if NETSTANDARD
            foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
            {
                CollectReferencedAssemblies(Assembly.Load(referencedAssembly), assemblies);
            }
#endif
        }

        private static MetadataReference CreateReference(Assembly assembly)
        {
            return _references.GetOrAdd(
                assembly.Location,
                loc => MetadataReference.CreateFromFile(loc));
        }
    };
}
