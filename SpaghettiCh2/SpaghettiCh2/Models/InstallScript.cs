using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UndertaleModLib;
using UndertaleModLib.Scripting;

namespace USPInstaller.Models
{
    public class InstallScript(string scriptPath)
    {
        public async Task RunAsync(IScriptInterface scriptInterface)
        {
            var scriptOptions = ScriptOptions.Default
                .AddImports("UndertaleModLib", "UndertaleModLib.Models", "UndertaleModLib.Decompiler",
                            "UndertaleModLib.Scripting", "UndertaleModLib.Compiler",
                            "System", "System.IO", "System.Collections.Generic", "System.Linq",
                            "System.Text.RegularExpressions")
                .AddReferences(typeof(UndertaleObject).GetTypeInfo().Assembly,
                                Assembly.GetExecutingAssembly(),
                                typeof(System.Text.RegularExpressions.Regex).GetTypeInfo().Assembly
                                )
                .WithEmitDebugInformation(true)
                .WithLanguageVersion(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest)
                .WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, Path.GetDirectoryName(scriptPath)))
                .WithAllowUnsafe(true); //when script throws an exception, add a exception location (line number)

            using var stream = File.OpenRead(scriptPath);
            Script script = CSharpScript.Create(stream, scriptOptions,  typeof(IScriptInterface));
            await script.RunAsync(scriptInterface);
        }
    }
}
