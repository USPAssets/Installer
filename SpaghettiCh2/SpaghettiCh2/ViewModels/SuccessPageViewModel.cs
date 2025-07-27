using CommunityToolkit.Mvvm.Input;
using System;

namespace USPInstaller.ViewModels
{
    partial class SuccessPageViewModel : PageViewModelBase
    {
        [RelayCommand]
        private void VisitWebsite()
        {
            MainWindowViewModel.OpenBrowser("https://undertaleita.net/deltarune.html");
        }

        [RelayCommand]
        private void BackToMenu()
        {
            OnBackRequested();
        }

        public event Action? BackRequested;
        protected virtual void OnBackRequested() => BackRequested?.Invoke();
    }
}