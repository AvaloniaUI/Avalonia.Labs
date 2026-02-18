using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Panels;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class FlexViewModel : ViewModelBase
    {
        private readonly ObservableCollection<FlexItemViewModel> _numbers;

        private int _currentNumber = 41;

        static FlexViewModel()
        {
            ViewLocator.Register(typeof(FlexViewModel), () => new FlexView());
        }

        public FlexViewModel()
        {
            Title = "Flex View";

            _numbers = new ObservableCollection<FlexItemViewModel>(Enumerable.Range(1, 40).Select(x => new FlexItemViewModel(x)));

            Numbers = new ReadOnlyObservableCollection<FlexItemViewModel>(_numbers);

            AddItemCommand = new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand(RemoveItem, () => SelectedItem != null);
        }

        public IEnumerable DirectionValues { get; } = Enum.GetValues(typeof(FlexDirection));

        public IEnumerable JustifyContentValues { get; } = Enum.GetValues(typeof(JustifyContent));

        public IEnumerable AlignItemsValues { get; } = Enum.GetValues(typeof(AlignItems));

        public IEnumerable AlignContentValues { get; } = Enum.GetValues(typeof(AlignContent));

        public IEnumerable WrapValues { get; } = Enum.GetValues(typeof(FlexWrap));

        public IEnumerable FlexBasisKindValues { get; } = Enum.GetValues(typeof(FlexBasisKind));

        public IEnumerable HorizontalAlignmentValues { get; } = Enum.GetValues(typeof(HorizontalAlignment));

        public IEnumerable VerticalAlignmentValues { get; } = Enum.GetValues(typeof(VerticalAlignment));

        public IEnumerable AlignSelfValues { get; } = Enum.GetValues(typeof(AlignItems)).Cast<AlignItems>().Prepend(FlexItemViewModel.AlignSelfAuto);

        [ObservableProperty]
        public partial FlexDirection Direction { get; set; }

        [ObservableProperty]
        public partial JustifyContent JustifyContent { get; set; }

        [ObservableProperty]
        public partial AlignItems AlignItems { get; set; }

        [ObservableProperty]
        public partial AlignContent AlignContent { get; set; }

        [ObservableProperty]
        public partial FlexWrap Wrap { get; set; }

        [ObservableProperty]
        public partial int ColumnSpacing { get; set; } = 64;

        [ObservableProperty]
        public partial int RowSpacing { get; set; } = 32;

        public ReadOnlyObservableCollection<FlexItemViewModel> Numbers { get; }

        [ObservableProperty]
        public partial FlexItemViewModel? SelectedItem { get; set; }

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
