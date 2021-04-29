namespace AgileObjects.BuildableExpressions.UnitTests
{
    using System;
#if FEATURE_COMPILATION
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Common;
    using Microsoft.CSharp;
    using NetStandardPolyfills;
#endif

    public static class CompilationAssertionExtensions
    {
        public static void ShouldCompile(this string cSharpSourceCode, params Type[] dependedOnAssemblyTypes)
        {
#if FEATURE_COMPILATION

            var codeProvider = new CSharpCodeProvider();

            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                TreatWarningsAsErrors = false
            };

            var referenceAssemblyPaths = new[]
                {
                    typeof(object),
                    typeof(Enumerable),
                    typeof(Regex),
                    typeof(CompilationAssertionExtensions),
                    typeof(FluentAssertionExtensions)
                }
                .Concat(dependedOnAssemblyTypes)
                .Distinct()
                .Select(t => t.GetAssembly().Location)
                .ToArray();

            parameters.ReferencedAssemblies.AddRange(referenceAssemblyPaths);

            var compilationResult = codeProvider
                .CompileAssemblyFromSource(parameters, cSharpSourceCode);

            if (!compilationResult.Errors.HasErrors)
            {
                return;
            }

            var errors = string.Join(
                Environment.NewLine,
                compilationResult
                    .Errors
                    .Cast<CompilerError>()
                    .Select(ce => $"Error ({ce.ErrorNumber}): {ce.ErrorText}"));

            throw new NotSupportedException(errors);
#endif
        }
    }
}
