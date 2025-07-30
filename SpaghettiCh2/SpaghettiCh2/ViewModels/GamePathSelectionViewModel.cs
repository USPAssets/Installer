using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
            this.gameType = type;

            ChooseFileCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                ExePath = (await OpenGamePathSelectionDialog(type))?.Path.LocalPath;
            });

            var isValidPath = this.WhenAnyValue(
                x => x.ExePath,
                x => !string.IsNullOrWhiteSpace(x) && (Directory.Exists(x) || File.Exists(x)));
            ContinueCommand = ReactiveCommand.Create(() => OnPathSelected(ExePath!), isValidPath);

            GoBackCommand = ReactiveCommand.Create(OnBackSelected);
        }

        private readonly AssetFolder.GameType gameType;

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
            // For learning purposes, we opted to directly get the reference
            // for StorageProvider APIs here inside the ViewModel. 
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.MainWindow?.StorageProvider is { } provider)
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
                        AppleUniformTypeIdentifiers = new[] { "public.executable", "public.application" },
                        MimeTypes = new[] { "application/x-executable", "application/x-sh", "application/x-elf" }
                    }}
                });

                return files.Count >= 1 ? files[0] : null;
            }
            return null;
        }

        private async Task<IStorageFolder?> DoOpenFolderPickerAsync(AssetFolder.GameType type)
        {
            // For learning purposes, we opted to directly get the reference
            // for StorageProvider APIs here inside the ViewModel. 
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
                desktop.MainWindow?.StorageProvider is { } provider)
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
        protected virtual void OnPathSelected(string path)
        {
            PathSelected?.Invoke(path, gameType);
        }


        public event Action? BackSelected;
        protected virtual void OnBackSelected() => BackSelected?.Invoke();
    }
}