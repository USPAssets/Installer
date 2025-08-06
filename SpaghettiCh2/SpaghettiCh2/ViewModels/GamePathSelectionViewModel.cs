using Avalonia;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using USPInstaller.Models;

namespace USPInstaller.ViewModels
{
    partial class GamePathSelectionViewModel : PageViewModelBase
    {
        public GamePathSelectionViewModel() : this(AssetFolder.GameType.Deltarune)
        {
        }

        public GamePathSelectionViewModel(AssetFolder.GameType type)
        {
            gameType = type;

            AutoFillGamePath();

            var checkIfSelfExtractingExe = (string? exePath) =>
            {
                return exePath != null 
                    && OperatingSystem.IsWindows() 
                    && gameType == AssetFolder.GameType.Undertale 
                    && new FileInfo(exePath).Length > 100 * 1024 * 1024; // 100 MB
            };

            ChooseFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                ExePath = (await OpenGamePathSelectionDialog(type))?.Path.LocalPath;
                // Try to detect (and reject) CAB self-extracting executable by file size - only apply to UNDERTALE
                if (checkIfSelfExtractingExe(ExePath)) // 100 MB
                {
                    string message = "Il file selezionato va estratto prima di poter installare la traduzione." + Environment.NewLine + "Consulta la guida all'installazione per ulteriori informazioni.";
                    await MessageBoxViewModel.ShowWithLink(message, "Avviso", "https://github.com/USPAssets/Installer/blob/main/GUIDA_ESTRAZIONE_UT.md", "Apri la Guida");
                    ExePath = null;
                }
            });

            var isValidPath = this.WhenAnyValue(
                x => x.ExePath,
                x => !string.IsNullOrWhiteSpace(x) && (Directory.Exists(x) || File.Exists(x)));
            ContinueCommand = ReactiveCommand.Create(() => OnPathSelected(ExePath!), isValidPath);

            GoBackCommand = ReactiveCommand.Create(OnBackSelected);
        }

        private readonly AssetFolder.GameType gameType;

        public static bool IsMacOs => OperatingSystem.IsMacOS();

        public string GameName => gameType switch
        {
            AssetFolder.GameType.Undertale => "UNDERTALE",
            AssetFolder.GameType.Deltarune => "DELTARUNE",
            _ => throw new NotImplementedException(),
        };

        public bool IsDeltarune => gameType == AssetFolder.GameType.Deltarune;

        [ObservableProperty]
        private string? _exePath;
        public ReactiveCommand<Unit, Unit> ChooseFileCommand { get; }
        public ReactiveCommand<Unit, Unit> ContinueCommand { get; }
        public ReactiveCommand<Unit, Unit> GoBackCommand { get; }
        public string PathWatermark
        {
            get
            {
                // Undertale has a native Linux version
                if (OperatingSystem.IsLinux() && gameType == AssetFolder.GameType.Undertale)
                {
                    return $"Percorso di run.sh nella directory di {GameName}";
                }

                if (OperatingSystem.IsMacOS())
                {
                    return $"Percorso di {GameName}.app";
                }
                else
                {
                    return $"Percorso di {GameName}.exe";
                }
            }
        }

        private void AutoFillGamePath()
        {
            if (OperatingSystem.IsMacOS())
            {
                string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string steamFolder = Path.Combine(userFolder, "Steam", "steamapps", "common");

                if (gameType == AssetFolder.GameType.Undertale)
                {
                    string undertalePath = Path.Combine(steamFolder, "Undertale", "UNDERTALE.app");
                    if (Path.Exists(undertalePath))
                    {
                        ExePath = undertalePath;
                    }    
                }
                else if (gameType == AssetFolder.GameType.Deltarune)
                {
                    string deltarunePath = Path.Combine(steamFolder, "DELTARUNE", "DELTARUNE.app");
                    string deltaruneDemoPath = Path.Combine(steamFolder, "DELTARUNEDemo", "DELTARUNE.app");

                    if (Path.Exists(deltarunePath))
                    {
                        ExePath = deltarunePath;
                    }
                    else if (Path.Exists(deltaruneDemoPath))
                    {
                        ExePath = deltaruneDemoPath;
                    }
                }
            }
            
            if (OperatingSystem.IsLinux())
            {
                // Should be /home/deck/.local/share/ - it should work at least on Steam Deck
                string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string steamPath = Path.Combine(userFolder, "Steam", "steamapps", "common");

                if (gameType == AssetFolder.GameType.Undertale)
                {
                    string undertalePathLinuxVer = Path.Combine(steamPath, "Undertale", "run.sh");
                    string undertalePathProtonVer = Path.Combine(steamPath, "Undertale", "UNDERTALE.exe");

                    // We try the native version first
                    if (Path.Exists(undertalePathLinuxVer))
                    {
                        ExePath = undertalePathLinuxVer;
                    }
                    else if (Path.Exists(undertalePathProtonVer))
                    {
                        ExePath = undertalePathProtonVer;
                    }
                }
                else if (gameType == AssetFolder.GameType.Deltarune)
                {
                    string deltarunePath = Path.Combine(steamPath, "DELTARUNE", "DELTARUNE.exe");
                    string deltaruneDemoPath = Path.Combine(steamPath, "DELTARUNEDemo", "DELTARUNE.exe");

                    if (Path.Exists(deltarunePath))
                    {
                        ExePath = deltarunePath;
                    }
                    else if (Path.Exists(deltaruneDemoPath))
                    {
                        ExePath = deltaruneDemoPath;
                    }
                }
            }   

            if (OperatingSystem.IsWindows())
            {
                string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                string steamPath = Path.Combine(programFilesPath, "Steam", "steamapps", "common");

                if (gameType == AssetFolder.GameType.Undertale)
                {
                    string undertalePath = Path.Combine(steamPath, "Undertale", "UNDERTALE.exe");

                    if (Path.Exists(undertalePath))
                    {
                        ExePath = undertalePath;
                    }
                }
                else if (gameType == AssetFolder.GameType.Deltarune)
                {
                    string deltarunePath = Path.Combine(steamPath, "DELTARUNE", "DELTARUNE.exe");
                    string deltaruneDemoPath = Path.Combine(steamPath, "DELTARUNEDemo", "DELTARUNE.exe");

                    if (Path.Exists(deltarunePath))
                    {
                        ExePath = deltarunePath;
                    }
                    else if (Path.Exists(deltaruneDemoPath))
                    {
                        ExePath = deltaruneDemoPath;
                    }
                }
            }
        }

        private async Task<IStorageItem?> OpenGamePathSelectionDialog(AssetFolder.GameType type)
        {
            if (OperatingSystem.IsMacOS())
            {
                return await DoOpenFolderPickerAsync(type);
            }
            return await DoOpenFilePickerAsync(type);

        }

        private async Task<IStorageFile?> DoOpenFilePickerAsync(AssetFolder.GameType type)
        {
            if (Application.Current?.GetTopLevel()?.StorageProvider is IStorageProvider provider)
            {
                var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Seleziona l'eseguibile del gioco",
                    SuggestedFileName = GameName + ".exe",
                    AllowMultiple = false,
                    FileTypeFilter = new[] {
                    new FilePickerFileType("Eseguibile")
                    {
                        Patterns = new[] { "*.exe", "*.app", "*.sh" },
                        AppleUniformTypeIdentifiers = new[] { "public.executable", "com.apple.application" },
                        MimeTypes = new[] { "application/x-executable", "application/x-sh", "application/x-elf" }
                    }}
                });

                return files.Count >= 1 ? files[0] : null;
            }
            return null;
        }

        private async Task<IStorageFolder?> DoOpenFolderPickerAsync(AssetFolder.GameType type)
        {
            if (Application.Current?.GetTopLevel()?.StorageProvider is IStorageProvider provider)
            {
                var files = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Seleziona l'applicazione del gioco",
                    SuggestedFileName = GameName + ".app",
                    AllowMultiple = false,
                });

                return files.Count >= 1 ? files[0] : null;
            }
            return null;
        }

        public event Action<string, AssetFolder.GameType>? PathSelected;
        protected virtual void OnPathSelected(string path) => PathSelected?.Invoke(path, gameType);

        public event Action? BackSelected;
        protected virtual void OnBackSelected() => BackSelected?.Invoke();
    }
}