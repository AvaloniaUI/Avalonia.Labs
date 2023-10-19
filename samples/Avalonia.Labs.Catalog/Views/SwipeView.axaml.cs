using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Labs.Controls;
using Avalonia.Labs.Controls.Base;
using Avalonia.Visuals;
using Avalonia.VisualTree;

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

        private void CloseSwipe(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.GetVisualAncestors().OfType<Swipe>().FirstOrDefault() is Labs.Controls.Swipe swipe)
            {
                swipe.SwipeState = SwipeState.Hidden;
            }
        }
    }
}
