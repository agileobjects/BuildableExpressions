#if NETFRAMEWORK
namespace AgileObjects.BuildableExpressions.Generator.ProjectManagement.NonSdk
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using EnvDTE;

    internal class DevToolsFactory
    {
        public static DTE GetDevToolsOrNullFor(string solutionName)
        {
            return EnumerateActiveDevTools().FirstOrDefault(devTools =>
                Path.GetFileName(devTools.Solution.FullName) == solutionName);
        }

        public static IEnumerable<DTE> EnumerateActiveDevTools()
        {
            var monikers = new IMoniker[1];

            GetRunningObjectTable(0, out var runningObjectTable);

            runningObjectTable.EnumRunning(out var enumerator);
            enumerator.Reset();

            while (enumerator.Next(1, monikers, default) == 0)
            {
                var runningObjectRef = monikers[0];

                CreateBindCtx(0, out var ctx);
                runningObjectRef.GetDisplayName(ctx, null, out var name);

                if (!name.StartsWith("!VisualStudio.DTE", StringComparison.Ordinal))
                {
                    continue;
                }

                runningObjectTable.GetObject(runningObjectRef, out var runningObject);

                if (runningObject is not DTE devTools)
                {
                    continue;
                }

                try
                {
                    if (string.IsNullOrEmpty(devTools.Solution.FullName))
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }

                yield return devTools;
            }
        }

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
    }
}
#endif