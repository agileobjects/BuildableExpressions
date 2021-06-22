using DefaultNamespace;

namespace AgileObjects.BuildableExpressions.Generator.Console
{
    public class ExpressionBuilderOutputClass
    {
        public static readonly string TypeAssemblyLocation = "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.1.28\\System.Private.CoreLib.dll";

        public string GetNameNetCore3()
        {
            return typeof(ExpressionBuilder).Name;
        }
    }
}