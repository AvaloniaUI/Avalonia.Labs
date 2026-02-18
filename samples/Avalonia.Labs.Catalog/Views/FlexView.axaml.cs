using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Labs.Catalog.ViewModels;

namespace Avalonia.Labs.Catalog.Views
{
    public partial class FlexView : UserControl
    {
        public FlexView()
        {
            InitializeComponent();
        }

        private void OnItemTapped(object? sender, RoutedEventArgs e)
        {
            if (sender is ListBoxItem control && control.DataContext is FlexItemViewModel item  && DataContext is FlexViewModel ViewModel)
            {
                if (ViewModel.SelectedItem != null)
                {
                    ViewModel.SelectedItem.IsSelected = false;
                }

                if (ViewModel.SelectedItem == item)
                {
                    ViewModel.SelectedItem = null;
                }
                else
                {
                    ViewModel.SelectedItem = item;
                    ViewModel.SelectedItem.IsSelected = true;
                }
            }
        }
    }
}
