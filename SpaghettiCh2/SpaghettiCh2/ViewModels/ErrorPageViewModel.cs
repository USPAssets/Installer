using Avalonia;
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
            MainWindowViewModel.OpenBrowser("mailto:undertalespaghettiproject@gmail.com");
        }

        [RelayCommand]
        private void CopyLog()
        {
            // Avalonia has abstracted away the clipboard - which is not directly accessible
            // in a viewmodel. We either make a TopLevel accessor (like we did) or make a 
            // abstracted service that we inject in the viewmodel (I can't be bothered).
            var clipboard = Application.Current.GetTopLevel()?.Clipboard;
            clipboard?.SetTextAsync(LogContent);
        }
    }
}