using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;

namespace Avalonia.Labs.Notifications.Android
{
    public interface IActivityIntentResultHandler
    {
        event EventHandler<Intent> OnActivityIntent;
    }
}
