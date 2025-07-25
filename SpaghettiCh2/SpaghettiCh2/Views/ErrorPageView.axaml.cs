using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace USPInstaller.Views
{
    public partial class ErrorPageView : UserControl
    {
        public ErrorPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}