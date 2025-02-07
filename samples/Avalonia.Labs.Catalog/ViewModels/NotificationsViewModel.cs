using System.Threading.Tasks;
using System;
using System.Reactive;
using Avalonia.Controls.Notifications;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels;

internal class NotificationsViewModel : ViewModelBase
{
    static NotificationsViewModel()
    {
        ViewLocator.Register(typeof(NotificationsViewModel), () => new NotificationsView());
    }

    public NotificationsViewModel()
    {
        Title = "Notification";
        ShowNotificationCommand = ReactiveCommand.CreateFromTask(ShowNotificationAsync);
    }

    private string? _notificationText = null;
    public string? NotificationText
    {
        get => _notificationText; 
        set => this.RaiseAndSetIfChanged(ref _notificationText, value);
    }
    private string? _actionButtonText = "action";
    public string? ActionButtonText
    {
        get => _actionButtonText; 
        set => this.RaiseAndSetIfChanged(ref _actionButtonText, value);
    }

    private int _notificationDurationSeconds = 3;
    public int NotificationDurationSeconds
    {
        get => _notificationDurationSeconds; 
        set => this.RaiseAndSetIfChanged(ref _notificationDurationSeconds, value);
    }
    private NotificationType _notificationType = Avalonia.Controls.Notifications.NotificationType.Information;
    public NotificationType NotificationType
    {
        get => _notificationType; 
        set => this.RaiseAndSetIfChanged(ref _notificationType, value);
    }
    private NotificationPosition _notificationPosition = NotificationPosition.TopLeft;
    public NotificationPosition NotificationPosition
    {
        get => _notificationPosition; 
        set => this.RaiseAndSetIfChanged(ref _notificationPosition, value);
    }

    public ReactiveCommand<Unit,Unit> ShowNotificationCommand { get; private set; }
    
    private Task ShowNotificationAsync()
    {
        if (NotificationPosition == NotificationPosition.TopCenter)
        {
            NotificationManager.Default.ShowNotification(new NotificationOptions()
            {
                Type=NotificationType.Error,
                Content="NotificationPosition.BottomCenter is not yet supported by Avalonia.Labs.Controls since it targets avalonia 11.0.0"
            });
            return Task.CompletedTask;
        }

        if (NotificationPosition == NotificationPosition.BottomCenter)
        {
            NotificationManager.Default.ShowNotification(new NotificationOptions()
            {
                Type=NotificationType.Error,
                Content="NotificationPosition.BottomCenter is not yet supported by Avalonia.Labs.Controls since it targets avalonia 11.0.0"
            });
            return Task.CompletedTask;
        }

        var o = new NotificationOptions()
        {
            Content = NotificationText ?? $"Hello {_notificationCount:00}",
            Type = NotificationType,
            Duration = NotificationDurationSeconds == 0 ?
                TimeSpan.FromDays(30) // "infinitely open"
                :
                TimeSpan.FromSeconds(NotificationDurationSeconds),
            Position = NotificationPosition,
            ClickActionText = ActionButtonText ?? string.Empty,
            DismissAction = () =>
            {
                NotificationManager.Default.ShowNotification(new NotificationOptions
                {
                    Content = $"Notification dismissed: '{NotificationText}'", Type = NotificationType.Success,
                });
                return Task.CompletedTask;
            }
        };
        if (!string.IsNullOrEmpty(o.ClickActionText))
            o.ClickAction = () =>
            {
                NotificationManager.Default.ShowNotification(new NotificationOptions
                {
                    Content = $"Action '{ActionButtonText}' clicked",
                });
                return Task.CompletedTask;
            };
        
        NotificationManager.Default.ShowNotification(o);

        _notificationCount++;
        
        return Task.CompletedTask;
    }
    
    private int _notificationCount = 1;
}
