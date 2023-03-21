using System;
using System.Collections;
using System.Collections.Specialized;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;

namespace Avalonia.Labs.Controls
{
    public abstract class MultiPage : Page
    {
        public static readonly DirectProperty<MultiPage, IEnumerable?> PagesProperty =
            AvaloniaProperty.RegisterDirect<MultiPage, IEnumerable?>(nameof(Pages),
                o => o.Pages, (o, v) => o.Pages = v);

        public static readonly StyledProperty<IDataTemplate?> PageTemplateProperty =
            AvaloniaProperty.Register<MultiPage, IDataTemplate?>(nameof(PageTemplate), new DefaultPageDataTemplate());

        private IEnumerable? _pages;

        public virtual IEnumerable? Pages
        {
            get => _pages;
            set
            {
                _pages = value;
                SetAndRaise(PagesProperty, ref _pages, value);
            }
        }

        public IDataTemplate? PageTemplate
        {
            get => GetValue(PageTemplateProperty);
            set => SetValue(PageTemplateProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PagesProperty)
            {
                if (change.OldValue is INotifyCollectionChanged oldNotifyCollection)
                {
                    oldNotifyCollection.CollectionChanged -= NotifyCollection_CollectionChanged;
                }

                LogicalChildren.Clear();

                if (change.NewValue != null)
                {
                    foreach (var page in Pages!)
                    {
                        if (page is ILogical logical)
                        {
                            LogicalChildren.Add(logical);
                        }
                    }
                }

                if (change.NewValue is INotifyCollectionChanged newNotifyCollection)
                {
                    newNotifyCollection.CollectionChanged += NotifyCollection_CollectionChanged;
                }

                UpdateActivePage();
            }
        }

        private void NotifyCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (var old in e.OldItems)
                {
                    if (old is ILogical logical)
                        LogicalChildren.Remove(logical);
                }

            if (e.NewItems != null)
                foreach (var newItem in e.NewItems)
                {
                    if (newItem is ILogical logical)
                        LogicalChildren.Add(logical);
                }
            UpdateActivePage();
        }
    }
}
