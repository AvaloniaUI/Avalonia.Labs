using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace Avalonia.Labs.Controls;

public class NotificationOptions
{
    private TimeSpan? _duration;
    public object? Content { get; set; }
    public NotificationType Type { get; set; } = NotificationType.Information;
    public TimeSpan? Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            if (value is not null)
            {
                ExpiresAt = DateTime.UtcNow + value;
            }
        }
    }
    public DateTime? ExpiresAt { get; private set; } = null;
    public Func<bool>? IsExpired { get; set; }
    public Func<Task>? ClickAction { get; set; }
    public Func<Task>? DismissAction { get; set; }
    public NotificationPosition? Position { get; set; }
    public string? ClickActionText { get; set; }
}
