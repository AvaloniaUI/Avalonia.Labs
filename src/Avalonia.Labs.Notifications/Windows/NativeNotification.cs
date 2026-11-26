#if INCLUDE_WINDOWS
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Security;
using System.Threading;
using Avalonia.Labs.Notifications.Windows.WinRT;
using Avalonia.Media.Imaging;

namespace Avalonia.Labs.Notifications.Windows
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    internal class NativeNotification : INativeNotification
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
                    subActions += $"<action content='{SecurityElement.Escape(action.Caption)}' {(ReplyActionTag == action.Tag && Actions.Count == 1 ? $"hint-inputId='input'" : "")} arguments='{SecurityElement.Escape(BuildArguments("user", action.Tag))}'/>";
                }

                if (hasInput)
                {
                    inputAction = $"<input id='input' type='text'/>";
                }

                actions = $"""
                    <actions>
                        {inputAction}
                        {subActions}
                    </actions>
                    """;
            }

            string image = "";
            if(Icon is { }  icon)
            {
                var name = _manager.SaveBitmapToAppPath(icon);

                image = $"<image placement='appLogoOverride' src='{SecurityElement.Escape(name)}'/>";
            }

            var xml = $"""
              <toast launch='{SecurityElement.Escape(BuildArguments("activate", null))}'>
                <visual>
                  <binding template="ToastGeneric">
                    <text>{SecurityElement.Escape(Title)}</text>
                    <text>{SecurityElement.Escape(Message)}</text>
                    {image}
                  </binding>
                </visual>
                {actions}
              </toast>
            """;
            var xmlDoc = NativeWinRTMethods.CreateInstance<IXmlDocument>("Windows.Data.Xml.Dom.XmlDocument");
            using (var xmlIntPtr = new HStringWrapper(xml))
            {
                ((IXmlDocumentIO)xmlDoc).LoadXml(xmlIntPtr);
            }

            var factory = NativeWinRTMethods.CreateActivationFactory<IToastNotificationFactory>("Windows.UI.Notifications.ToastNotification");
            CurrentNotification = factory.CreateToastNotification(xmlDoc);
            if (CurrentNotification is IToastNotification2 toastNotification2)
            {
                using var idPtr = new HStringWrapper(Id.ToString());
                toastNotification2.SetTag(idPtr);

                using var groupStr = new HStringWrapper(NativeNotificationManager.NotificationsGroupName);
                toastNotification2.SetGroup(groupStr);
            }

            _manager.Show(this);
            CurrentNotification = null;
        }

        private string BuildArguments(string action, string? userAction)
        {
            // Percent-encoded, so ';' and '=' in values can't break OnNotificationReceived parsing.
            var arguments = $"action={action};notificationId={Id}";
            if (userAction is not null)
                arguments += $";userAction={Uri.EscapeDataString(userAction)}";
            if (Tag is not null)
                arguments += $";tag={Uri.EscapeDataString(Tag)}";
            return arguments;
        }

        private static uint GetNextId()
        {
            return Interlocked.Increment(ref s_currentId);
        }
    }
}
#endif
