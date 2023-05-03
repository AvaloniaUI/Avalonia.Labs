using Avalonia.Collections;
using Avalonia.Labs.Catalog.Views;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class VirtualizingWrapPanelViewModel : ViewModelBase
    {
        private AvaloniaList<string>? _items;

        static VirtualizingWrapPanelViewModel()
        {
            ViewLocator.Register(typeof(VirtualizingWrapPanelViewModel), () => new VirtualizingWrapPanelView());
        }

        public AvaloniaList<string>? Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public VirtualizingWrapPanelViewModel()
        {
            Title = "Virtualizing WrapPanel";

            _items = new AvaloniaList<string>();

            for (int i = 0; i < 1000; i++)
            {
                _items.Add($"Box {i + 1}");
            }
        }
    }
}
