using System;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.Styling;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// An item in  a <see cref="TabControl"/>/>.
    /// </summary>
    [PseudoClasses(":pressed", ":selected")]
    public class TabItem : ListBoxItem, ISelectable, IHeadered
    {
        private IDisposable? _boundsObservable;

        /// <summary>
        /// Defines the <see cref="Header"/> property.
        /// </summary>
        public static readonly StyledProperty<object?> HeaderProperty =
            AvaloniaProperty.Register<HeaderedContentControl, object?>(nameof(Header));

        /// <summary>
        /// Defines the <see cref="HeaderTemplate"/> property.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
            AvaloniaProperty.Register<HeaderedContentControl, IDataTemplate?>(nameof(HeaderTemplate));

        /// <summary>
        /// Defines the <see cref="HeaderTheme"/> property.
        /// </summary>
        public static readonly StyledProperty<ControlTheme?> HeaderThemeProperty =
            AvaloniaProperty.Register<TabItem, ControlTheme?>(nameof(HeaderTheme));

        /// <summary>
        /// Gets or sets the theme to be applied to this item's header.
        /// </summary>
        public ControlTheme? HeaderTheme
        {
            get => GetValue(HeaderThemeProperty);
            set => SetValue(HeaderThemeProperty, value);
        }

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets the header presenter from the control's template.
        /// </summary>
        public IContentPresenter? HeaderPresenter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the data template used to display the header content of the control.
        /// </summary>
        public IDataTemplate? HeaderTemplate
        {
            get => GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Initializes static members of the <see cref="TabItem"/> class.
        /// </summary>
        static TabItem()
        {
            PressedMixin.Attach<TabItem>();
            FocusableProperty.OverrideDefaultValue<TabItem>(true);
            DataContextProperty.Changed.AddClassHandler<TabItem>((x, e) => x.UpdateHeader(e));
            AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<TabItem>(AutomationControlType.TabItem);
        }

        protected override AutomationPeer OnCreateAutomationPeer() => new ListItemAutomationPeer(this);

        private void UpdateHeader(AvaloniaPropertyChangedEventArgs obj)
        {
            if (Header == null)
            {
                if (obj.NewValue is IHeadered headered)
                {
                    if (Header != headered.Header)
                    {
                        Header = headered.Header;
                    }
                }
                else
                {
                    if (!(obj.NewValue is Control))
                    {
                        Header = obj.NewValue;
                    }
                }
            }
            else
            {
                if (Header == obj.OldValue)
                {
                    Header = obj.NewValue;
                }
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);

            _boundsObservable?.Dispose();
            _boundsObservable = null;

            if (Parent is TabControl pivot)
            {
                _boundsObservable = pivot.GetObservable(BoundsProperty).Subscribe(new AnonymousObserver<Rect>(ParentBoundsChanged));
            }
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);

            _boundsObservable?.Dispose();
            _boundsObservable = null;
        }

        private void ParentBoundsChanged(Rect bounds)
        {
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Parent is TabControl pivot && pivot.BorderPart != null)
            {
                var size = pivot.BorderPart.Bounds.Size;

                base.MeasureOverride(size);

                return size;
            }
            return base.MeasureOverride(availableSize);
        }
    }
}
