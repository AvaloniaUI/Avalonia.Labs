using System.Reactive.Linq;

using Avalonia.Labs.Panels;

using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public sealed class FlexItemViewModel : ReactiveObject
    {
        internal const AlignItems AlignSelfAuto = (AlignItems)(-1);

        private readonly ObservableAsPropertyHelper<AlignItems?> _alignSelf;

        private bool _isSelected;
        private bool _isVisible = true;

        private AlignItems _alignSelfItem = AlignSelfAuto;
        private int _order;

        public FlexItemViewModel(int value)
        {
            Value = value;

            _alignSelf = (from item in this.WhenAnyValue(vm => vm.AlignSelfItem)
                          select item == AlignSelfAuto ? default(AlignItems?) : item).ToProperty(this, nameof(AlignSelf));
        }

        public int Value { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => this.RaiseAndSetIfChanged(ref _isVisible, value);
        }

        public AlignItems AlignSelfItem
        {
            get => _alignSelfItem;
            set => this.RaiseAndSetIfChanged(ref _alignSelfItem, value);
        }

        public AlignItems? AlignSelf => _alignSelf.Value;

        public int Order
        {
            get => _order;
            set => this.RaiseAndSetIfChanged(ref _order, value);
        }
    }
}
