using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace Avalonia.Labs.Controls
{
    public class CarouselPage : SelectingMultiPage
    {
        private static readonly FuncTemplate<Panel> DefaultPanel =
            new FuncTemplate<Panel>(() => new StackPanel()
            {
                Orientation = Orientation.Horizontal
            });

        public static readonly StyledProperty<ITemplate<Panel>> ItemsPanelProperty =
            ItemsControl.ItemsPanelProperty.AddOwner<CarouselPage>();

        public static readonly StyledProperty<Vector> OffsetProperty = AvaloniaProperty.Register<CarouselPage, Vector>(nameof(Offset));

        static CarouselPage()
        {
            ItemsPanelProperty.OverrideDefaultValue<CarouselPage>(DefaultPanel);
        }

        public Vector Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public ITemplate<Panel> ItemsPanel
        {
            get => GetValue(ItemsPanelProperty);
            set => SetValue(ItemsPanelProperty, value);
        }

        internal ScrollViewer? ScrollViewerPart { get; private set; }
    }
}
