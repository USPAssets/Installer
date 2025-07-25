using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace USPInstaller.ViewModels
{
    partial class ErrorPageViewModel : PageViewModelBase
    {
        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _logContent = string.Empty;

        public ErrorPageViewModel(Exception exception, string logContent)
        {
            ErrorMessage = exception.Message;
            LogContent = exception.ToString() + Environment.NewLine + Environment.NewLine + "---START SCRIPT LOG ---" + Environment.NewLine + logContent;
        }

        [RelayCommand]
        private void BackToMenu()
        {
            OnBackRequested();
        }

        public event Action? BackRequested;
        protected virtual void OnBackRequested() => BackRequested?.Invoke();

        [RelayCommand]
        private static void ContactUs()
        {
            MainWindowViewModel.OpenBrowser("https://undertaleita.net/deltarune.html");
        }
    }
}