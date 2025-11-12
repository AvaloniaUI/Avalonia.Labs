using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Avalonia.Labs.Controls;


/// <summary>
/// Presents an in-app notification to the user
/// </summary>
public partial class NotificationControl : ContentControl
{
    public NotificationControl()
    {
        _options = new NotificationOptions();
    }
    
    public NotificationControl(NotificationOptions options)
    {
        _options = options;
        Type = options.Type;
        Content = options.Content ?? string.Empty;
        ActionButtonContent = options.ClickActionText;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        _layoutRoot = e.NameScope.Find<Panel>("PART_LayoutRoot");

        if (_options.ClickAction != null)
        {
            var actionButton = e.NameScope.Find<Button>("PART_ActionButton");
            if (actionButton != null)
            {
                actionButton.IsVisible = true;
                actionButton.Click += async (s, e) =>
                {
                    await _options.ClickAction();
                    Close();
                };
            }
        }
        
        _border = e.NameScope.Find<Border>("PART_NotificationBorder");
        if (_border is not null)
        {
            _border.PointerPressed += (_, __) => Close();
        }
    }

    public void Open()
    {
        PseudoClasses.Set(":open", true);
        PseudoClasses.Set(":hidden", false);
    }

    public async Task Close()
    {
        if (_isClosing) return;
        _isClosing = true;

        PseudoClasses.Set(":open", false);
        PseudoClasses.Set(":hidden", true);

        // Get the layout root from template
        if (_layoutRoot is Panel layoutRoot)
        {
            _animationCompletionSource = new TaskCompletionSource<bool>();
            
            // Subscribe to animation completion
            layoutRoot.PropertyChanged += LayoutRootPropertyChanged;
            
            // Start animation by setting pseudo classes
            PseudoClasses.Set(":open", false);
            PseudoClasses.Set(":hidden", true);

            // Wait for animation completion
            await _animationCompletionSource.Task;
            
            // Cleanup
            layoutRoot.PropertyChanged -= LayoutRootPropertyChanged;
        }
    
        _isClosing = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    internal bool ShouldExpire(DateTime now)
    {
        if (_options.IsExpired is { } exp)
            return exp();

        return _options.ExpiresAt.HasValue && now >= _options.ExpiresAt.Value;
    }

    internal async void OnNotificationPressed(object? sender, PointerPressedEventArgs e)
    {
        try
        {
            await Close();

            if (_options?.DismissAction is { } cancelCallback)
                await cancelCallback();
        }
        // async void => so we catch all
        catch 
        {   
        }
    }
    
    private void LayoutRootPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(Panel.IsVisible) && e.NewValue is bool b && b == false)
        {
            _animationCompletionSource?.TrySetResult(true);
        }
    }

    
    private readonly NotificationOptions _options;
    private bool _isClosing;
    private Panel? _layoutRoot;
    private TaskCompletionSource<bool>? _animationCompletionSource;
    private Border? _border;
}
