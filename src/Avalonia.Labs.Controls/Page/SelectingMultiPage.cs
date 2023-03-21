using Avalonia.Controls.Presenters;

namespace Avalonia.Labs.Controls
{
    public abstract class SelectingMultiPage : MultiPage
    {
        public static readonly DirectProperty<SelectingMultiPage, ItemsPresenter?> PresenterProperty =
            AvaloniaProperty.RegisterDirect<SelectingMultiPage, ItemsPresenter?>(nameof(Presenter), o => o.Presenter);
        private ItemsPresenter? _presenter;

        public object? SelectedPage { get; set; }

        public ItemsPresenter? Presenter
        {
            get => _presenter;
            protected set => SetAndRaise(PresenterProperty, ref _presenter, value);
        }
    }
}
