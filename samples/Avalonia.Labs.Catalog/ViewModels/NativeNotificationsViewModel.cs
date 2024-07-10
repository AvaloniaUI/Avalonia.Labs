using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Notifications;
using Avalonia.Platform;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class NativeNotificationsViewModel : ViewModelBase
    {
        static NativeNotificationsViewModel()
        {
            ViewLocator.Register(typeof(NativeNotificationsViewModel), () => new NativeNotificationsView());
        }

        private string _customCaption = "";
        private string _response = "";
        private INativeNotification? _currentNotification;

        public string CustomCaption
        {
            get => _customCaption;
            set => this.RaiseAndSetIfChanged(ref _customCaption, value);
        }

        public string Response
        {
            get => _response;
            set => this.RaiseAndSetIfChanged(ref _response, value);
        }

        public NativeNotificationsViewModel()
        {
            Title = "Native Notifications";
            var manager = NativeNotificationManager.Current;

            if(manager != null)
            {
                manager.NotificationCompleted += Manager_NotificationCompleted;
            }
        }

        private void Manager_NotificationCompleted(object? sender, NativeNotificationCompletedEventArgs e)
        {
            if(e.UserData is string data)
            {
                Response = $"Reply: {data}";
            }
            else if(e.ActionTag != null)
            {
                Response = e.ActionTag + " action was selected";
            }
            else if(e.IsActivated)
            {
                Response = "Notification was tapped";
            }
            else if (e.IsCancelled)
            {
                Response = "Notification was dismissed";
            }
            _currentNotification?.Close();
        }

        private INativeNotification? GenerateNotification(string? category = null)
        {
            _currentNotification = NativeNotificationManager.Current?.CreateNotification(category);
            if (_currentNotification != null)
                _currentNotification.Icon = new Media.Imaging.Bitmap(AssetLoader.Open(new System.Uri("avares://Avalonia.Labs.Catalog/Assets/avalonia-32.png")));
            return _currentNotification;
        }

        public void SendBasicNotification()
        {
            if (GenerateNotification() is { } notification)
            {
                notification.Title = "Hello Avalonia";
                notification.Message = "Hello, this is a basic notification. It doesn't pop out.";
                
                notification.Show();
            }
        }

        public void SendPredefinedActionsNotification()
        {
            if (GenerateNotification("actions") is { } notification)
            {
                notification.Title = "Hello Avalonia";
                notification.Message = "Hello, this is a basic notification with actions defined at launch";

                notification.Show();
            }
        }

        public void SendCustomActionsNotification()
        {
            if (GenerateNotification("actions") is { } notification)
            {
                notification.Title = "Hello Avalonia";
                notification.Message = "Hello, this is a basic notification with custom actions. This is not supported on iOS";
                notification.SetActions([new NativeNotificationAction("hey", string.IsNullOrWhiteSpace(_customCaption) ? "Hey" : _customCaption)]);

                notification.Show();
            }
        }

        public void SendReplyActionNotification()
        {
            if (GenerateNotification("reply") is { } notification)
            {
                notification.Title = "Hello Avalonia";
                notification.Message = "Hello, this is a basic notification with a reply action";
                notification.ReplyActionTag = "reply";

                notification.Show();
            }
        }
    }
}
