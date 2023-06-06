using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    /// A navigation page that supports simple stack-based navigation
    /// </summary>
    [TemplatePart("PART_NavigationBar", typeof(Border))]
    [TemplatePart("PART_BackButton", typeof(Button))]
    [TemplatePart("PART_ForwardButton", typeof(Button))]
    [TemplatePart("PART_ContentPresenter", typeof(TransitioningContentControl))]
    public class NavigationPage : MultiPage
    {
        private Button? _backButton;

        internal static readonly StyledProperty<object?> ContentProperty =
           ContentControl.ContentProperty.AddOwner<ContentPage>();

        public static readonly StyledProperty<IBrush?> BarBackgroundProperty =
            AvaloniaProperty.Register<NavigationPage, IBrush?>(nameof(BarBackground));

        public static readonly StyledProperty<IBrush?> BarTextColorProperty =
            AvaloniaProperty.Register<NavigationPage, IBrush?>(nameof(BarTextColor));

        /// <summary>
        /// Defines the <see cref="IsBackButtonVisible"/> property.
        /// </summary>
        public static readonly StyledProperty<bool?> IsBackButtonVisibleProperty =
            AvaloniaProperty.Register<NavigationControl, bool?>(nameof(IsBackButtonVisible), true);

        /// <summary>
        /// Defines the <see cref="IsNavBarVisible"/> property.
        /// </summary>
        public static readonly StyledProperty<bool?> IsNavBarVisibleProperty =
            AvaloniaProperty.Register<NavigationPage, bool?>(nameof(IsNavBarVisible), true);

        public static readonly AttachedProperty<string?> BackButtonTitleProperty =
            AvaloniaProperty.RegisterAttached<NavigationPage, Page, string?>("BackButtonTitle");

        public static readonly AttachedProperty<bool> HasBackButtonProperty =
            AvaloniaProperty.RegisterAttached<NavigationPage, Page, bool>("HasBackButton", true);

        public static readonly AttachedProperty<object?> TitleViewProperty =
            AvaloniaProperty.RegisterAttached<NavigationPage, Page, object?>("TitleView");

        public static readonly AttachedProperty<IImage?> TitleIconProperty =
            AvaloniaProperty.RegisterAttached<NavigationPage, Page, IImage?>("TitleIcon");

        public IBrush? BarBackground
        {
            get { return GetValue(BarBackgroundProperty); }
            set { SetValue(BarBackgroundProperty, value); }
        }
        public IBrush? BarTextColor
        {
            get { return GetValue(BarTextColorProperty); }
            set { SetValue(BarTextColorProperty, value); }
        }

        public bool? IsBackButtonVisible
        {
            get => ActiveChildPage != null && GetValue(IsBackButtonVisibleProperty) == true && GetHasBackButton(ActiveChildPage);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }

        [Content]
        [DependsOn(nameof(PageTemplate))]
        internal object? Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of the navigation bar
        /// </summary>
        public bool? IsNavBarVisible
        {
            get => GetValue(IsNavBarVisibleProperty);
            set => SetValue(IsNavBarVisibleProperty, value);
        }

        public static string? GetBackButtonTitle(Page page)
        {
            return page.GetValue(BackButtonTitleProperty);
        }

        public static void SetBackButtonTitle(Page page, string title)
        {
            page.SetValue(BackButtonTitleProperty, title);
        }

        public static bool GetHasBackButton(Page page)
        {
            return page.GetValue(HasBackButtonProperty);
        }

        public static void SetHasBackButton(Page page, bool hasBackButton)
        {
            page.SetValue(BackButtonTitleProperty, hasBackButton);
        }

        public static object? GetTitleView(Page page)
        {
            return page.GetValue(TitleViewProperty);
        }

        public static void SetTitleView(Page page, object? titleView)
        {
            page.SetValue(TitleViewProperty, titleView);
        }

        public static IImage? GetTitleIcon(Page page)
        {
            return page.GetValue(TitleIconProperty);
        }

        public static void SetTitleIcon(Page page, IImage titleIcon)
        {
            page.SetValue(TitleIconProperty, titleIcon);
        }

        static NavigationPage()
        {
            PageNavigationSystemBackButtonPressedEvent.AddClassHandler<NavigationPage>( (sender, eventArgs) =>
            {
                if(sender.IsBackButtonVisible == true)
                {
                    eventArgs.Handled = sender.Pop() != null;
                }
            });
        }

        public NavigationPage()
        {
            Pages = new Stack<object>();
        }

        /// <summary>
        /// Gets or sets the BackButton template part.
        /// </summary>
        private Button? BackButton
        {
            get { return _backButton; }
            set
            {
                if (_backButton != null)
                {
                    _backButton.Click -= BackButton_Clicked;
                }
                _backButton = value;
                if (_backButton != null)
                {
                    _backButton.Click += BackButton_Clicked;
                }
            }
        }

        private ContentPresenter? _contentPresenter;
        private Border? _navBar;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            BackButton = e.NameScope.Get<Button>("PART_BackButton");

            _contentPresenter = e.NameScope.Get<ContentPresenter>("PART_ContentPresenter");
            _navBar = e.NameScope.Get<Border>("PART_NavigationBar");

            UpdateActivePage();
        }

        protected override void UpdateContentSafeAreaPadding()
        {
            if (_contentPresenter != null && _navBar != null)
            {
                _navBar.Padding = new Thickness(SafeAreaPadding.Left, SafeAreaPadding.Top, SafeAreaPadding.Right, 0);

                _contentPresenter.Padding = Padding;

                if (ActiveChildPage != null)
                {
                    var remainingSafeArea = Padding.GetRemainingSafeAreaPadding(SafeAreaPadding);
                    ActiveChildPage.SafeAreaPadding = new Thickness(remainingSafeArea.Left, 0, remainingSafeArea.Right, remainingSafeArea.Bottom);
                }
            }
        }

        private async void BackButton_Clicked(object? sender, RoutedEventArgs eventArgs)
        {
            Pop();
        }

        public void Push(object page)
        {
            if (Pages is Stack<object> pages)
            {
                pages.Push(page);
            }
            else if (Pages is IList list)
            {
                list.Add(page);
            }

            if (page is Page p && !p.IsSet(TitleViewProperty))
            {
                SetTitleView(p, new Label()
                {
                    Content = p.Title,
                    VerticalContentAlignment = Layout.VerticalAlignment.Center,
                });
            }

            UpdateActivePage();
        }

        public object? Pop()
        {
            object? old = null;

            if (Pages is Stack<object> pages)
            {
                old = pages.Pop();
            }
            else if(Pages is IList list)
            {
                if(list.Count > 0)
                {
                    old = list[list.Count - 1];
                    list.Remove(old);
                }
            }

            UpdateActivePage();

            return old;
        }

        protected override void UpdateActivePage()
        {
            if (Pages is Stack<object> pages)
            {
                if(pages.TryPeek(out var page))
                {
                    Content = page;
                }
                else
                {
                    Content = null;
                }
            }
            else if (Pages is IList list)
            {
                if (list.Count > 0)
                {
                    Content = list[list.Count - 1];
                }
                else
                {
                    Content = null;
                }
            }
            else
            {
                Content = null;
            }

            ActiveChildPage = _contentPresenter?.Child as Page;

            UpdateContentSafeAreaPadding();
        }
    }
}
