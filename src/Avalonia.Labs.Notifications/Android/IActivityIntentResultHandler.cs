#if ANDROID
using System;
using Android.Content;

namespace Avalonia.Labs.Notifications.Android
{
    public interface IActivityIntentResultHandler
    {
        event EventHandler<Intent> OnActivityIntent;
    }
}
#endif
