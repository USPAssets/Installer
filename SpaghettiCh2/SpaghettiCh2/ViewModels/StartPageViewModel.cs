using CommunityToolkit.Mvvm.ComponentModel;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
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

        public StartPageViewModel()
        {
            OnUndertaleClick = ReactiveCommand.Create(() => GameSelected?.Invoke(AssetFolder.GameType.Undertale));
            OnDeltaruneClick = ReactiveCommand.Create(() => GameSelected?.Invoke(AssetFolder.GameType.Deltarune));
            OnWebsiteClick = ReactiveCommand.Create(() => MainWindowViewModel.OpenBrowser("https://undertaleita.net/deltarune.html"));
        }  
    }
}
