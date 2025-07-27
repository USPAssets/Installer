using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace USPInstaller.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        MinWidth = 800;
        MinHeight = 450;
        MaxWidth = 800;
        MaxHeight = 450;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}