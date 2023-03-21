using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using System;

namespace Avalonia.Labs.Controls
{
    [TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
    public class ContentPage : Page
    {
        public static readonly StyledProperty<object?> ContentProperty =
           ContentControl.ContentProperty.AddOwner<ContentPage>();

        public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =            
           ContentControl.ContentTemplateProperty.AddOwner<ContentPage>();

        public static readonly StyledProperty<bool> AutomaticallyApplySafeAreaPaddingProperty =
            AvaloniaProperty.Register<ContentPage, bool>(nameof(AutomaticallyApplySafeAreaPadding), true);

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            ContentControl.HorizontalContentAlignmentProperty.AddOwner<ContentPage>();

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            ContentControl.VerticalContentAlignmentProperty.AddOwner<ContentPage>();

        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get { return GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        private ContentPresenter? _contentPresenter;

        [Content]
        [DependsOn(nameof(ContentTemplate))]
        public object? Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public IDataTemplate? ContentTemplate
        {
            get { return GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public bool AutomaticallyApplySafeAreaPadding
        {
            get { return GetValue(AutomaticallyApplySafeAreaPaddingProperty); }
            set { SetValue(AutomaticallyApplySafeAreaPaddingProperty, value); }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");

            if(_contentPresenter == null )
            {
                throw new NullReferenceException("PART_ContentPresenter isn't found the template");
            }

            UpdateContentSafeAreaPadding();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == AutomaticallyApplySafeAreaPaddingProperty)
            {
                UpdateContentSafeAreaPadding();
            }
            else if (change.Property == ContentProperty)
            {
                UpdateContentSafeAreaPadding();

                if (change.OldValue is ILogical oldLogical)
                {
                    LogicalChildren.Remove(oldLogical);
                }
                if (change.NewValue is ILogical newLogical)
                {
                    LogicalChildren.Add(newLogical);
                }
            }
        }

        protected override void UpdateContentSafeAreaPadding()
        {
            if (_contentPresenter != null)
            {
                _contentPresenter.Padding = AutomaticallyApplySafeAreaPadding ? Padding.ApplySafeAreaPadding(SafeAreaPadding) : Padding;

                if (ActiveChildPage != null)
                    ActiveChildPage.SafeAreaPadding = Padding.GetRemainingSafeAreaPadding(SafeAreaPadding);
            }
        }
    }
}
