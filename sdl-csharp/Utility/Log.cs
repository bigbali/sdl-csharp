using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace sdl_csharp.Utility
{
    public static class Logger
    {
        public static void Log(string text = null,
        [CallerFilePath] string file = "",
        [CallerMemberName] string member = "",
        [CallerLineNumber] int line = 0)
        {
            Trace.WriteLine($"{Path.GetFileName(file)} {member} line {line}{(text is null ? "" : $": {text}")}");
        }
    }
}
