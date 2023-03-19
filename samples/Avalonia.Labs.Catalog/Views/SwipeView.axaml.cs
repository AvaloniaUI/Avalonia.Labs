using System.Linq;
using Avalonia.Controls;
using Avalonia.Labs.Controls.Base;

namespace Avalonia.Labs.Catalog.Views
{
    public partial class SwipeView : UserControl
    {
        public SwipeView()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_OnOnTap(object? sender, TapEventArgs e)
        {
            if (sender is StackPanel panel)
            {
                var label = panel.Children.OfType<Label>().First();
                label.Content = "Clicked";
            }
        }
    }
}
