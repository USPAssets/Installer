using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SpaghettiCh2;

public partial class Window1 : Window
{
    public Window1()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}