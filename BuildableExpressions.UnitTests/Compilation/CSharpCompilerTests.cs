namespace AgileObjects.BuildableExpressions.UnitTests.Compilation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BuildableExpressions.Compilation;
    using BuildableExpressions.SourceCode;
    using Common;
    using NetStandardPolyfills;
    using Xunit;

    public class CSharpCompilerTests
    {
        [Fact]
        public void ShouldCompileSimpleSourceCode()
        {
            const string source = @"
namespace MyNamespace
{
    public class MyClass
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";
            var result = CSharpCompiler.Compile(source);

            var compiledAssembly = result
                .ShouldNotBeNull()
                .CompiledAssembly
                .ShouldNotBeNull();

            var testType = compiledAssembly.GetType("MyNamespace.MyClass").ShouldNotBeNull();
            var testInstance = Activator.CreateInstance(testType).ShouldNotBeNull();
            var testMethod = testType.GetPublicInstanceMethod("SayHello").ShouldNotBeNull();
            testMethod.Invoke(testInstance, Array.Empty<object>()).ShouldBe("Hello!");
        }

        [Fact]
        public void ShouldCompileMultipleSimpleSourceCodeFiles()
        {
            const string source1 = @"
namespace MyNamespace
{
    public class MyClass1
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";

            const string source2 = @"
namespace MyNamespace
{
    public class MyClass2
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";

            const string source3 = @"
namespace MyNamespace
{
    public class MyClass3
    {
        public string SayHello()
        {
            return ""Hello!"";
        }
    }
}";
            var result = CSharpCompiler
                .Compile(new[] { source1, source2, source3 }.AsEnumerable());

            var compiledAssembly = result
                .ShouldNotBeNull()
                .CompiledAssembly
                .ShouldNotBeNull();

            compiledAssembly.GetType("MyNamespace.MyClass1").ShouldNotBeNull();
            compiledAssembly.GetType("MyNamespace.MyClass2").ShouldNotBeNull();
            compiledAssembly.GetType("MyNamespace.MyClass3").ShouldNotBeNull();
        }

        [Fact]
        public void ShouldCompileSourceCodeWithPassedInDependencies()
        {
            const string source = @"
namespace MyNamespace
{
    using System.Collections.Generic;
    using System.Linq;

    public static class MyClass
    {
        public static IEnumerable<int> GetInts()
        {
            return new[] { 1, 2, 3 }.ToList();
        }
    }
}";
            var result = CSharpCompiler.Compile(source);

            var compiledAssembly = result
                .ShouldNotBeNull()
                .CompiledAssembly
                .ShouldNotBeNull();

            var testType = compiledAssembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == "MyClass")
                .ShouldNotBeNull();

            var testMethod = testType.GetPublicStaticMethod("GetInts").ShouldNotBeNull();

            testMethod
                .Invoke(null, Array.Empty<object>())
                .ShouldNotBeNull()
                .ShouldBeOfType<IEnumerable<int>>()
                .ShouldBe(1, 2, 3);
        }

        [Fact]
        public void ShouldCompileSourceCodeExpressionSourceCode()
        {
            const string source = @"
namespace MyNamespace
{
    using System;
    using System.Linq.Expressions;
    using AgileObjects.BuildableExpressions;
    using AgileObjects.BuildableExpressions.SourceCode;

    /// <summary>
    /// Supplies an input <see cref=""SourceCodeExpression""/> to compile to source code.
    /// </summary>
    public static class ExpressionBuilder
    {
        public static SourceCodeExpression Build()
        {
            return BuildableExpression.SourceCode(sc => 
            {
                sc.AddClass(""MyClass"", cls => 
                {
                    var doNothing = Expression.Lambda<Action>(Expression.Empty());
                    
                    cls.AddMethod(nameof(Build), doNothing, m => { });
                });
            });
        }
    }
}
    ";
            var result = CSharpCompiler.Compile(source);

            var compiledAssembly = result
                .ShouldNotBeNull()
                .CompiledAssembly
                .ShouldNotBeNull();

            var testType = compiledAssembly.GetType("MyNamespace.ExpressionBuilder").ShouldNotBeNull();
            var testMethod = testType.GetPublicStaticMethod("Build").ShouldNotBeNull();

            var sourceCodeExpression = testMethod
                .Invoke(null, Array.Empty<object>())
                .ShouldNotBeNull()
                .ShouldBeOfType<SourceCodeExpression>();

            var classExpression = sourceCodeExpression.TypeExpressions.ShouldHaveSingleItem();
            classExpression.Name.ShouldBe("MyClass");

            var methodExpression = classExpression.MethodExpressions.ShouldHaveSingleItem();
            methodExpression.ReturnType.ShouldBe(typeof(void));
            methodExpression.Name.ShouldBe("Build");
            methodExpression.Parameters.ShouldBeEmpty();
        }
    }
}