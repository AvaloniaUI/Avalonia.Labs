using System;
using Avalonia.Labs.Panels;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class FlexItemViewModel : ViewModelBase
    {
        internal const AlignItems AlignSelfAuto = (AlignItems)(-1);

        public FlexItemViewModel(int value)
        {
            Value = value;

            AlignSelf = AlignSelfItem == AlignSelfAuto ? default(AlignItems) : AlignSelfItem;

            var color = Random.Shared.Next();

            Color = new SolidColorBrush((uint)color);
        }

        public int Value { get; }

        public Brush Color { get; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }

        [ObservableProperty]
        public partial bool IsVisible { get; set; } = true;

        [ObservableProperty]
        public partial AlignItems AlignSelfItem { get; set; } = AlignSelfAuto;

        public AlignItems? AlignSelf { get; }

        [ObservableProperty]
        public partial int Order { get; set; }

        [ObservableProperty]
        public partial double Shrink { get; set; } = 1.0;

        [ObservableProperty]
        public partial double Grow { get; set; }

        [ObservableProperty]
        public partial double BasisValue { get; set; } = 100.0;

        [ObservableProperty]
        public partial FlexBasisKind BasisKind { get; set; }

        public FlexBasis Basis => new(BasisValue, BasisKind);

        [ObservableProperty]
        public partial HorizontalAlignment HorizontalAlignment { get; set; }

        [ObservableProperty]
        public partial VerticalAlignment VerticalAlignment { get; set; }
    }
}
