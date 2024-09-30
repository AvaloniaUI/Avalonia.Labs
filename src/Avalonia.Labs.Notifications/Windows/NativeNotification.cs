using System.Runtime.Versioning;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.Media.Imaging;
using MicroCom.Runtime;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal unsafe class NativeNotification : INativeNotification
    {
        private static uint s_currentId = 0;
        private readonly NativeNotificationManager _manager;
        private readonly NotificationChannel _channel;

        public NativeNotification(NotificationChannel channel, NativeNotificationManager nativeNotificationManager)
        {
            _channel = channel;
            _manager = nativeNotificationManager;
            Id = GetNextId();

            Actions = channel.Actions;
        }

        public uint Id { get; }

        public string Category => _channel.Id;

        public string? Title { get; set; }

        public string? Tag { get; set; }
        public string? Message { get; set; }
        public TimeSpan? Expiration { get; set; }

        public Bitmap? Icon { get; set; }

        public IReadOnlyList<NativeNotificationAction> Actions { get; private set; }
        public IToastNotification? CurrentNotification { get; private set; }
        public string? ReplyActionTag { get; set; }

        public void Close()
        {
            _manager.Close(this);
        }

        public void SetActions(IReadOnlyList<NativeNotificationAction> actions)
        {
            Actions = actions;
        }

        public void Show()
        {
            string actions = "";

            if (Actions.Count > 0)
            {
                var subActions = "";
                var inputAction = "";

                bool hasInput = false;

                foreach (var action in Actions)
                {
                    if (ReplyActionTag == action.Tag)
                    {
                        hasInput = true;
                    }
                    subActions += $"<action content='{action.Caption}' {(ReplyActionTag == action.Tag && Actions.Count == 1 ? $"hint-inputId='input'" : "")} arguments='action=user;userAction={action.Tag};notificationId={Id}'/>";
                }

                if (hasInput)
                {
                    inputAction = $"<input id='input' type='text'/>";
                }

                actions = $"""
                    <actions>
                        ${inputAction}
                        ${subActions}
                    </actions>
                    """;
            }

            string image = "";
            if(Icon is { }  icon)
            {
                var name = _manager.SaveBitmapToAppPath(icon);

                image = $"<image placement='appLogoOverride' src='{name}'/>";
            }

            var xml = $"""
              <toast launch='action=activate;notificationId={Id};tag={Tag}'>">
                <visual>
                  <binding template="ToastGeneric">
                    <text>{Title}</text>
                    <text>{Message}</text>
                    {image}
                  </binding>
                </visual>
                {actions}
              </toast>
            """;
            using var xmlDoc = NativeWinRTMethods.CreateInstance<IXmlDocument>("Windows.Data.Xml.Dom.XmlDocument");
            using var xmlIO = xmlDoc.QueryInterface<IXmlDocumentIO>();
            using (var xmlIntPtr = new HStringWrapper(xml))
            {
                xmlIO.LoadXml(xmlIntPtr);
            }

            using var factory = NativeWinRTMethods.CreateActivationFactory<IToastNotificationFactory>("Windows.UI.Notifications.ToastNotification");
            CurrentNotification = factory.CreateToastNotification(xmlDoc);
            if (CurrentNotification.QueryInterface<IToastNotification2>() is { } toastNotification2)
            {
                using var idPtr = new HStringWrapper(Id.ToString());
                toastNotification2.SetTag(idPtr);
            }

            _manager.Show(this);
            CurrentNotification?.Dispose();
            CurrentNotification = null;
        }

        private static uint GetNextId()
        {
            return Interlocked.Increment(ref s_currentId);
        }
    }
}
