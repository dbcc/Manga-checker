using System;
using System.IO;
using System.Runtime.CompilerServices;
using Manga_checker.Properties;

namespace Manga_checker.Handlers {
    public class DebugText {
        public static void Write(string text, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0) {
            //Read
            Settings.Default.Debug += $"[{DateTime.Now}][method:{callerName} line:{lineNumber}] {text}\n";
            Log(text);
        }

        private static void Log(string text, [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0) {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");
            try {
                File.AppendAllText($"logs/{DateTime.Now.ToShortDateString()}-mc.log", $"[{DateTime.Now}][method:{callerName} line:{lineNumber}] {text}\n");
            }
            catch {
                // ignored
            }
        }
    }
}