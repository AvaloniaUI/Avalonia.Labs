using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace Avalonia.Labs.Catalog.Android;

[Activity(Label = "Avalonia.Labs.Catalog.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
}
