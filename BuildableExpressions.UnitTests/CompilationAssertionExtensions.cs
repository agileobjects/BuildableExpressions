namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
#if FEATURE_COMPILATION
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Common;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NetStandardPolyfills;
#endif

    public static class CompilationAssertionExtensions
    {
        public static void ShouldCompile(this string sourceCode, params Type[] dependedOnAssemblyTypes)
        {
#if FEATURE_COMPILATION

            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            var references = new[]
                {
                    typeof(object),
                    typeof(Enumerable),
                    typeof(Regex),
                    typeof(CompilationAssertionExtensions),
                    typeof(FluentAssertionExtensions)
                }
                .Concat(dependedOnAssemblyTypes)
                .Distinct()
                .Select(t => MetadataReference.CreateFromFile(t.GetAssembly().Location));

            var compilation = CSharpCompilation.Create(
                "BuildableExpressionsTestAssembly" + Guid.NewGuid(),
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var dllStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(dllStream, pdbStream);

                if (emitResult.Success)
                {
                    return;
                }

                var errors = string.Join(
                    Environment.NewLine,
                    emitResult.Diagnostics.Select(d => d.ToString()));

                throw new NotSupportedException(errors);
            }
#endif
        }
    }
}
