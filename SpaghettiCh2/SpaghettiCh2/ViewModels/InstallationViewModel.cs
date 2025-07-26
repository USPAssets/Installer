using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib;
using USPInstaller.Models;
using static USPInstaller.Models.AssetFolder;

namespace USPInstaller.ViewModels
{
    partial class InstallationViewModel : PageViewModelBase
    {
        public InstallationViewModel(AssetFolder.GameType gameType)
        {
            this.gameType = gameType;
        }

        [ObservableProperty] private double _progressValue;
        [ObservableProperty] private string? _overallProgressMessage = "Installazione in corso...";
        [ObservableProperty] private string? _scriptProgressMessage = "...";

        private AssetFolder.GameType gameType;
        private StringBuilder log = new StringBuilder();

        public void UpdateProgress(string message, string subMessage, double value, double maxValue)
        {
            ScriptProgressMessage = message;
            ProgressValue = value / maxValue * 100;
        }

        public void Log(string s)
        {
            log.AppendLine(s);
        }

        public event Action<Exception, string>? InstallationError;
        public event Action? InstallationSuccess;

        public async Task InstallGame(string exePath)
        {
            try
            {
                string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                OverallProgressMessage = "Scarico l'ultima versione della traduzione...";
                string assetPath = await AssetFolder.DownloadLatestAsync("USPAssets", "Online", executablePath);
                switch (gameType)
                {
                    case GameType.Undertale:
                        await InstallUndertale(assetPath, exePath);
                        break;
                    case GameType.Deltarune:
                        await InstallDeltarune(assetPath, exePath);
                        break;
                }
                InstallationSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                InstallationError?.Invoke(ex, log.ToString());
            }
        }

        private string? GetDataFileName(string exePath)
        {
            if (Path.GetExtension(exePath) is string extn && Path.GetDirectoryName(exePath) is string dirn)
            {
                if (extn == ".exe" || extn == string.Empty)
                {
                    string dataPath = Path.Combine(dirn, "data.win");
                    if (File.Exists(dataPath))
                    {
                        return dataPath;
                    }
                    // Try linux style path
                    dataPath = Path.Combine(dirn, "game.unx");
                    if (File.Exists(dataPath))
                    {
                        return dataPath;
                    }
                    return null;
                }
                else if (extn == ".app")
                {
                    return Path.Combine(exePath, "Contents", "Resources", "game.ios");
                }
            }
            return null;
       }

        private async Task InstallUndertale(string assetPath, string exePath)
        {
            string scriptPath = Path.Join(assetPath, "Undertale", "installer.csx");

            OverallProgressMessage = "Installazione in corso...";
            await RunScriptOn(scriptPath, GetDataFileName(exePath)!);
        }

        private async Task InstallDeltarune(string assetPath, string exePath)
        {
            string scriptsPath = Path.Join(assetPath, "Deltarune", "InstallScripts");
            string gameFolder = Path.GetDirectoryName(exePath) ?? throw new InvalidOperationException("Il percorso dell'eseguibile non è valido.");
            string dataPath = GetDataFileName(exePath)!;
            string dataFilename = Path.GetFileName(dataPath);

            if (Directory.Exists(Path.Join(gameFolder, "lang")))
            {
                // Assume this is the Deltarune Steam demo, run the demo installation script
                OverallProgressMessage = "Installazione in corso...";
                await RunScriptOn(Path.Join(assetPath, "Deltarune", "InstallScripts", "demo.csx"), dataPath);
                return;
            }

            OverallProgressMessage = "Installo il launcher...";
            await RunScriptOn(Path.Join(scriptsPath, "launcher.csx"), dataPath);

            // Iterate thorugh subdirectories in the game folder that start with "chapter" followed by a number and extract that number
            foreach (string chapterFolder in Directory.EnumerateDirectories(gameFolder)
                .Where(d => Path.GetFileName(d).StartsWith("chapter", StringComparison.OrdinalIgnoreCase)))
            {
                // Ignore _windows or _mac suffix
                string chapterFolderName = Path.GetFileName(chapterFolder);
                string chapterName = chapterFolderName.Substring(0, chapterFolderName.IndexOf('_'));
                string scriptPath = Path.Join(scriptsPath, $"{chapterName}.csx");
                if (!File.Exists(scriptPath))
                {
                    Debug.WriteLine($"No script found for {chapterName}, skipping.");
                    continue;
                }
                int chapterNumber = int.Parse(chapterName.Substring("chapter".Length));
                OverallProgressMessage = $"Installo capitolo {chapterNumber}...";
                await RunScriptOn(scriptPath, Path.Join(chapterFolder, dataFilename));
            }

            OverallProgressMessage = "Installazione completata!";
        }
        private async Task RunScriptOn(string scriptPath, string dataPath)
        {
            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException("Non trovo il data.win del gioco", dataPath);
            }

            ScriptProgressMessage = "Carico i dati di gioco...";

            UndertaleData data;
            using (Stream dataStream = File.OpenRead(dataPath))
            {
                data = await Task.Run(() => UndertaleIO.Read(dataStream, (s, imp) => Log(s), Log));
            }

            InstallScript installScript = new InstallScript(scriptPath);
            ScriptContext context = new(data, dataPath, scriptPath, this);
            await Task.Run(async () => await installScript.RunAsync(context));

            ScriptProgressMessage = "Sovrascrivo i dati di gioco...";

            using (Stream dataStream = new FileStream(dataPath, FileMode.Create, FileAccess.Write))
            {
                await Task.Run(() => UndertaleIO.Write(dataStream, data, Log));
            }

            ScriptProgressMessage = null;
        }
    }
}