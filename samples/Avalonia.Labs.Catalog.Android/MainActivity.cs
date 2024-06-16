using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Avalonia.Android;
using Avalonia.Labs.Notifications;
using Avalonia.Labs.Notifications.Android;
using Avalonia.ReactiveUI;
using NotificationChannel = Avalonia.Labs.Notifications.NotificationChannel;

namespace Avalonia.Labs.Catalog.Android;

[Activity(Label = "Avalonia.Labs.Catalog.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>, IActivityIntentResultHandler
{
    public event EventHandler<Intent> OnActivityIntent;

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithAndroidNotifications(new AndroidNotificationOptions()
            {
                Channels = new[]
                {
                    new NotificationChannel("basic", "Send Notifications", Notifications.NotificationPriority.High),
                    new NotificationChannel("actions", "Send Notification with Predefined Actions", Notifications.NotificationPriority.High)
                    {
                        Actions = new List<NativeNotificationAction>
                        {
                            new NativeNotificationAction()
                            {
                                Tag = "hello",
                                Caption = "Hello"
                            },
                            new NativeNotificationAction()
                            {
                                Tag = "world",
                                Caption = "world"
                            }
                        }
                    },
                    new NotificationChannel("custom", "Send Notification with Custom Actions", Notifications.NotificationPriority.High),
                    new NotificationChannel("reply", "Send Notification with Reply Action", Notifications.NotificationPriority.High)
                    {
                        Actions = new List<NativeNotificationAction>
                        {
                            new NativeNotificationAction()
                            {
                                Tag = "reply",
                                Caption = "Reply"
                            }
                        }
                    },
                }
            }, this)
            .UseReactiveUI();
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);

        OnActivityIntent?.Invoke(this, intent);
    }
}
