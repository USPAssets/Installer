using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib;
using USPInstaller.Models;
using static USPInstaller.Models.AssetFolder;

namespace USPInstaller.ViewModels
{
    partial class InstallationViewModel : PageViewModelBase
    {
        // HACK: Avalonia wants a public empty constructor so that we can use the designer
        // to see how the viewmodel looks at design-time. Not to be used in actual code.
        public InstallationViewModel() 
        {
            this.gameType = GameType.Undertale;
        }

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
                // On Win: C:\Users\{user}\AppData\Local\Temp\USPInstaller\
                var tempAssetsPath = Path.Combine(Path.GetTempPath(), "USPInstaller");

                OverallProgressMessage = "Scarico l'ultima versione della traduzione...";
                string assetPath = await AssetFolder.DownloadLatestAsync("USPAssets", "Online", tempAssetsPath);
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
                    dataPath = Path.Combine(dirn, "assets", "game.unx");
                    if (File.Exists(dataPath))
                    {
                        return dataPath;
                    }
                    // MacOS .app bundle could be a file or a folder, who knows??
                    if (dirn.EndsWith(".app"))
                    {
                        dataPath = Path.Combine(exePath, "Contents", "Resources", "game.ios");
                        if (File.Exists(dataPath))
                        {
                            return dataPath;
                        }
                    }
                    return null;
                }
                else if (extn == ".sh")
                {
                    return Path.Combine(dirn, "assets", "game.unx");
                }
                else if (extn == ".apk")
                {
                    // For Android, we assume the data file is in the same directory as the executable
                    return Path.Combine(dirn, "assets", "game.android");
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
            string dataPath = GetDataFileName(exePath)! ?? throw new FileNotFoundException("Non trovo il file di dati del gioco", exePath);
            string dataFilename = Path.GetFileName(dataPath);
            string dataFolder = Path.GetDirectoryName(dataPath) ?? throw new InvalidOperationException("Il percorso dell'eseguibile non è valido.");

            if (Directory.Exists(Path.Join(dataFolder, "lang")))
            {
                // Assume this is the Deltarune Steam demo, run the demo installation script
                OverallProgressMessage = "Installazione in corso...";
                await RunScriptOn(Path.Join(assetPath, "Deltarune", "InstallScripts", "demo.csx"), dataPath);
                return;
            }

            OverallProgressMessage = "Installo il launcher...";
            await RunScriptOn(Path.Join(scriptsPath, "launcher.csx"), dataPath);

            // Iterate thorugh subdirectories in the game folder that start with "chapter" followed by a number and extract that number
            foreach (string chapterFolder in Directory.EnumerateDirectories(dataFolder)
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