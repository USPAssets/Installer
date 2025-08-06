using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using USPInstaller.Views;

namespace USPInstaller.ViewModels
{
    public partial class MessageBoxViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Title";

        [ObservableProperty]
        private string _message = "Message";

        [ObservableProperty]
        private string _linkTitle= "Apri Link";

        private string linkTarget = string.Empty;

        [ObservableProperty]
        private bool _showYesButton = true;

        [ObservableProperty]
        private bool _showNoButton = true;

        [ObservableProperty]
        private bool _showOkButton = true;

        [ObservableProperty]
        private bool _showLinkButton = false;

        public Action<bool> ResultCallback { get; set; }

        [RelayCommand]
        private void Yes()
        {
            ResultCallback?.Invoke(true);
        }

        [RelayCommand]
        private void No()
        {
            ResultCallback?.Invoke(false);
        }

        [RelayCommand]
        private void Ok()
        {
            ResultCallback?.Invoke(true);
        }


        [RelayCommand]
        private void OpenLink()
        {
            MainWindowViewModel.OpenBrowser(linkTarget);
        }

        public static Task<bool> Show(string message, string title, bool showYesNo = false)
        {
            var tcs = new TaskCompletionSource<bool>();

            void ShowDialogOnUIThread()
            {
                var window = new MessageBoxWindow();
                var viewModel = new MessageBoxViewModel
                {
                    Title = title,
                    Message = message,
                    ShowYesButton = showYesNo,
                    ShowNoButton = showYesNo,
                    ShowOkButton = !showYesNo,
                    ShowLinkButton = false,
                    ResultCallback = result =>
                    {
                        tcs.SetResult(result);
                        window.Close();
                    }
                };
                window.DataContext = viewModel;

                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    window.ShowDialog(desktop.MainWindow!);
                else
                    window.Show();
            }

            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                ShowDialogOnUIThread();
            }
            else
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(ShowDialogOnUIThread);
            }

            return tcs.Task;
        }

        public static Task<bool> ShowWithLink(string message, string title, string linkTarget, string linkTitle)
        {
            var tcs = new TaskCompletionSource<bool>();

            void ShowDialogOnUIThread()
            {
                var window = new MessageBoxWindow();
                var viewModel = new MessageBoxViewModel
                {
                    Title = title,
                    Message = message,
                    ShowYesButton = false,
                    ShowNoButton = false,
                    ShowOkButton = true,
                    ShowLinkButton = true,
                    linkTarget = linkTarget,
                    LinkTitle = linkTitle,
                    ResultCallback = result =>
                    {
                        tcs.SetResult(result);
                        window.Close();
                    }
                };
                window.DataContext = viewModel;

                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    window.ShowDialog(desktop.MainWindow!);
                else
                    window.Show();
            }

            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                ShowDialogOnUIThread();
            }
            else
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(ShowDialogOnUIThread);
            }

            return tcs.Task;
        }
    }
}