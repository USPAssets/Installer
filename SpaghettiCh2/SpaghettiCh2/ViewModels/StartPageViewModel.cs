using ReactiveUI;
using System;
using System.Reactive;
using System.Reflection;
using USPInstaller.Models;

namespace USPInstaller.ViewModels
{
    class StartPageViewModel : PageViewModelBase
    {
        public delegate void GameSelectedEventHandler(AssetFolder.GameType gameType);
        public GameSelectedEventHandler? GameSelected;

        public ReactiveCommand<Unit, Unit> OnUndertaleClick { get; }
        public ReactiveCommand<Unit, Unit> OnDeltaruneClick { get; }
        public ReactiveCommand<Unit, Unit> OnWebsiteClick { get; }
        public ReactiveCommand<Unit, Unit> OnBlueSkyClick { get; }
        public ReactiveCommand<Unit, Unit> OnDiscordClick { get; }

        public Version InstallerVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0);

        public StartPageViewModel()
        {
            OnUndertaleClick = ReactiveCommand.Create(() => GameSelected?.Invoke(AssetFolder.GameType.Undertale));
            OnDeltaruneClick = ReactiveCommand.Create(() => GameSelected?.Invoke(AssetFolder.GameType.Deltarune));
            OnWebsiteClick = ReactiveCommand.Create(() => MainWindowViewModel.OpenBrowser("https://undertaleita.net/deltarune.html"));
            OnBlueSkyClick = ReactiveCommand.Create(() => MainWindowViewModel.OpenBrowser("https://bsky.app/profile/undertaleita.net"));
            OnDiscordClick = ReactiveCommand.Create(() => MainWindowViewModel.OpenBrowser("https://discord.gg/YrEkAJ5MrG"));
        }
    }
}
