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
    public class TabItem : HeaderedContentControl, ISelectable
    {
        private IDisposable? _boundsObservable;

        /// <summary>
        /// Defines the <see cref="IsSelected"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsSelectedProperty =
            SelectingItemsControl.IsSelectedProperty.AddOwner<TabItem>();

        /// <summary>
        /// Defines the <see cref="HeaderTheme"/> property.
        /// </summary>
        public static readonly StyledProperty<ControlTheme?> HeaderThemeProperty =
            AvaloniaProperty.Register<TabItem, ControlTheme?>(nameof(HeaderTheme));

        /// <summary>
        /// Gets or sets the selection state of the item.
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the theme to be applied to this item's header.
        /// </summary>
        public ControlTheme? HeaderTheme
        {
            get => GetValue(HeaderThemeProperty);
            set => SetValue(HeaderThemeProperty, value);
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
            SelectableMixin.Attach<TabItem>(IsSelectedProperty);
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
                if (obj.NewValue is HeaderedContentControl headered)
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
