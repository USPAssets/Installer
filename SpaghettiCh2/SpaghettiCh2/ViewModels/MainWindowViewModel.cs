using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using USPInstaller.Models;

namespace USPInstaller.ViewModels
{
    partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            // Initialize the current page to the ChooseGamePageViewModel
            var model = new StartPageViewModel();
            model.GameSelected += OnGameSelected;
            Current = model;
        }

        private void OnGameSelected(AssetFolder.GameType type)
        {
            var pathSelectionPage = new GamePathSelectionViewModel(type);
            pathSelectionPage.PathSelected += OnGamePathSelected;
            pathSelectionPage.BackSelected += ReturnToMainMenu;
            Current = pathSelectionPage;
        }

        private void ReturnToMainMenu()
        {
            var model = new StartPageViewModel();
            model.GameSelected += OnGameSelected;
            Current = model;
        }

        private void OnGamePathSelected(string path, AssetFolder.GameType gameType)
        {
            var progressPage = new InstallationViewModel(gameType);
            progressPage.InstallationError += OnInstallationError;
            progressPage.InstallationSuccess += OnInstallationSuccess;
            Current = progressPage;

            // Start the installation process
            _ = progressPage.InstallGame(path);
        }

        private void OnInstallationError(Exception ex, string log)
        {
            var errorPage = new ErrorPageViewModel(ex, log);
            errorPage.BackRequested += ReturnToMainMenu;
            Current = errorPage;
        }

        private void OnInstallationSuccess()
        {
            var successPage = new SuccessPageViewModel();
            successPage.BackRequested += ReturnToMainMenu;
            Current = successPage;
        }

        [ObservableProperty]
        public PageViewModelBase _current;

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
