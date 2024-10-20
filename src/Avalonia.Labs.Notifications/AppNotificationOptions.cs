using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Avalonia.Labs.Notifications;

public class AppNotificationOptions
{
#if INCLUDE_WINDOWS
    /// <summary>
    /// Application display name for notifications.
    /// If not defined, Application.Name if used.
    /// Is ignored for packaged applications.
    /// </summary>
    public string? AppName { [SupportedOSPlatform("windows")] get; init; }

    /// <summary>
    /// Application icon for notifications.
    /// Is ignored for packaged applications.
    /// </summary>
    public string? AppIcon { [SupportedOSPlatform("windows")] get; init; }

    /// <summary>
    /// Overrides AppUserModelId used for notifications.
    /// By default is generated from the
    /// Is ignored for packged applications.
    /// </summary>
    public string? AppUserModelId { [SupportedOSPlatform("windows")] get; init; }

    /// <summary>
    /// Indicates whether ComServer for receiving notification actions should be disabled.
    /// When true, no callbacks can be retrieved, but application makes less footprint on the user machine.
    /// Default is false.
    /// Is ignored for packged applications.
    /// </summary>
    public bool DisableComServer { [SupportedOSPlatform("windows")] get; init; }

    /// <summary>
    /// Windows notifications require each app to registered a unique COM activator.
    /// By default, Avalonia will generate Guid based on the AppUserModelId.
    /// </summary>
    /// <remarks>
    /// Overriding this property is necessary, if you set ToastActivatorCLSID for packaged apps.
    /// </remarks>
    public Guid? ComActivatorGuidOverride { [SupportedOSPlatform("windows")] get; init; }
#endif

    public IReadOnlyList<NotificationChannel>? Channels { get; init; }
}
