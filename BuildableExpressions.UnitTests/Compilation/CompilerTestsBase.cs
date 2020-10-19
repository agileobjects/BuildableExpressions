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

    public abstract class CompilerTestsBase
    {
        [Fact]
        public void ShouldCompileSimpleSourceCode()
        {
            var compiler = CreateCompiler();

            const string SOURCE = @"
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
            var result = compiler.Compile(SOURCE);

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
            var compiler = CreateCompiler();

            const string SOURCE1 = @"
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

            const string SOURCE2 = @"
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

            const string SOURCE3 = @"
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
            var result = compiler.Compile(new[] { SOURCE1, SOURCE2, SOURCE3 }.AsEnumerable());

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
            var compiler = CreateCompiler();

            const string SOURCE = @"
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
            var result = compiler.Compile(SOURCE);

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
            var compiler = CreateCompiler();

            const string SOURCE = @"
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
                    var doNothing = Expression.Lambda<Action>(Expression.Default(typeof(void)));
                    
                    cls.AddMethod(""DoNothing"", doNothing, m => { });
                });
            });
        }
    }
}
    ";
            var result = compiler.Compile(new[] { SOURCE });

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

            var classExpression = sourceCodeExpression.Types.ShouldHaveSingleItem();
            classExpression.Name.ShouldBe("MyClass");

            var methodExpression = classExpression.Methods.ShouldHaveSingleItem();
            methodExpression.ReturnType.ShouldBe(typeof(void));
            methodExpression.Name.ShouldBe("DoNothing");
            methodExpression.Parameters.ShouldBeEmpty();
        }

        #region Helper Members

        internal abstract ICompiler CreateCompiler();

        public class TestClass
        {
        }

        #endregion
    }
}