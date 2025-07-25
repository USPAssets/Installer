using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace USPInstaller.Views
{
    public partial class SuccessPageView : UserControl
    {
        public SuccessPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}