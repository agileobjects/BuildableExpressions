// See Program.cs for notes

namespace AgileObjects.BuildableExpressions.Generator.Samples.HelloWorld
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using SourceCode;

    public class OutputFilesGenerator : ISourceCodeExpressionBuilder
    {
        public IEnumerable<SourceCodeExpression> Build(IExpressionBuildContext context)
        {
            yield return BuildableExpression.SourceCode(sc =>
            {
                sc.AddClass("OutputAssemblies", cls =>
                {
                    cls.SetVisibility(TypeVisibility.Internal);

                    cls.AddField<List<string>>("All", fld =>
                    {
                        fld.SetVisibility(MemberVisibility.Public);
                        fld.SetStatic();
                        fld.SetReadonly();

                        var outputAssemblyNames = Expression.ListInit(
                            Expression.New(typeof(List<string>).GetConstructor(Type.EmptyTypes)!),
                            context
                                .OutputAssemblies
                                .Select(oa => oa.GetName().Name)
                                .OrderBy(name => name)
                                .Select(Expression.Constant));

                        fld.SetInitialValue(outputAssemblyNames);
                    });
                });
            });
        }
    }
}
