using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Avalonia.Android;
using Avalonia.Labs.Notifications;

namespace Avalonia.Labs.Catalog.Android;

[Activity(
    Label = "Avalonia.Labs.Catalog.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    public event EventHandler<Intent> OnActivityIntent;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        NativeNotificationManager.Current?.SetPermissionActivity(this);
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);

        OnActivityIntent?.Invoke(this, intent);
    }
}
