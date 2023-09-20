using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Panels;

using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public sealed class FlexViewModel : ViewModelBase
    {
        private readonly ObservableCollection<FlexItemViewModel> _numbers;

        private FlexDirection _direction = FlexDirection.Row;
        private JustifyContent _justifyContent = JustifyContent.FlexStart;
        private AlignItems _alignItems = AlignItems.FlexStart;
        private AlignContent _alignContent = AlignContent.FlexStart;
        private FlexWrap _wrap = FlexWrap.Wrap;

        private int _columnSpacing = 8;
        private int _rowSpacing = 32;

        private int _currentNumber = 41;

        private FlexItemViewModel? _selectedItem;

        static FlexViewModel()
        {
            ViewLocator.Register(typeof(FlexViewModel), () => new FlexView());
        }

        public FlexViewModel()
        {
            Title = "Flex View";

            _numbers = new ObservableCollection<FlexItemViewModel>(Enumerable.Range(1, 40).Select(x => new FlexItemViewModel(x)));

            Numbers = new ReadOnlyObservableCollection<FlexItemViewModel>(_numbers);

            AddItemCommand = ReactiveCommand.Create(AddItem);
            RemoveItemCommand = ReactiveCommand.Create(RemoveItem, this.WhenAnyValue(vm => vm.SelectedItem).Select(x => x != null));
        }

        public IEnumerable DirectionValues { get; } = Enum.GetValues(typeof(FlexDirection));

        public IEnumerable JustifyContentValues { get; } = Enum.GetValues(typeof(JustifyContent));

        public IEnumerable AlignItemsValues { get; } = Enum.GetValues(typeof(AlignItems));

        public IEnumerable AlignContentValues { get; } = Enum.GetValues(typeof(AlignContent));

        public IEnumerable WrapValues { get; } = Enum.GetValues(typeof(FlexWrap));

        public IEnumerable AlignSelfValues { get; } = Enum.GetValues(typeof(AlignItems)).Cast<AlignItems>().Prepend(FlexItemViewModel.AlignSelfAuto);
        
        public FlexDirection Direction
        {
            get => _direction;
            set => this.RaiseAndSetIfChanged(ref _direction, value);
        }

        public JustifyContent JustifyContent
        {
            get => _justifyContent;
            set => this.RaiseAndSetIfChanged(ref _justifyContent, value);
        }

        public AlignItems AlignItems
        {
            get => _alignItems;
            set => this.RaiseAndSetIfChanged(ref _alignItems, value);
        }

        public AlignContent AlignContent
        {
            get => _alignContent;
            set => this.RaiseAndSetIfChanged(ref _alignContent, value);
        }

        public FlexWrap Wrap
        {
            get => _wrap;
            set => this.RaiseAndSetIfChanged(ref _wrap, value);
        }

        public int ColumnSpacing
        {
            get => _columnSpacing;
            set => this.RaiseAndSetIfChanged(ref _columnSpacing, value);
        }

        public int RowSpacing
        {
            get => _rowSpacing;
            set => this.RaiseAndSetIfChanged(ref _rowSpacing, value);
        }

        public ReadOnlyObservableCollection<FlexItemViewModel> Numbers { get; }

        public FlexItemViewModel? SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        private void AddItem() => _numbers.Add(new FlexItemViewModel(_currentNumber++));

        private void RemoveItem()
        {
            if (SelectedItem is null)
            {
                throw new InvalidOperationException();
            }

            _numbers.Remove(SelectedItem);

            SelectedItem.IsSelected = false;
            SelectedItem = null;
        }
    }
}
