#if INCLUDE_WINDOWS
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Avalonia.Labs.Notifications.Windows.WinRT;

internal enum TrustLevel
{
    BaseTrust,
    PartialTrust,
    FullTrust
}

[GeneratedComInterface]
[Guid("AF86E2E0-B12D-4C6A-9C5A-D7AA65101E90")]
internal partial interface IInspectable
{
    void GetIids(out uint iidCount, out nint iids);
    nint GetRuntimeClassName();
    TrustLevel GetTrustLevel();
}

[GeneratedComInterface]
[Guid("F7F3A506-1E87-42D6-BCFB-B8C809FA5494")]
internal partial interface IXmlDocument : IInspectable
{
}

[GeneratedComInterface]
[Guid("6CD0E74E-EE65-4489-9EBF-CA43E87BA637")]
internal partial interface IXmlDocumentIO : IInspectable
{
    void LoadXml(nint xml);
}

[GeneratedComInterface]
[Guid("997E2675-059E-4E60-8B06-1760917C8B80")]
internal partial interface IToastNotification : IInspectable
{
}

[GeneratedComInterface]
[Guid("9DFB9FD1-143A-490E-90BF-B9FBA7132DE7")]
internal partial interface IToastNotification2 : IInspectable
{
    void SetTag(nint value);
    nint GetTag();
    void SetGroup(nint value);
}

[GeneratedComInterface]
[Guid("04124B20-82C6-4229-B109-FD9ED4662B53")]
internal partial interface IToastNotificationFactory : IInspectable
{
    IToastNotification CreateToastNotification(IXmlDocument content);
}

[GeneratedComInterface]
[Guid("50AC103F-D235-4598-BBEF-98FE4D1A3AD4")]
internal partial interface IToastNotificationManagerStatics : IInspectable
{
    IToastNotifier CreateToastNotifier();
    IToastNotifier CreateToastNotifierWithId(nint applicationId);
}

[GeneratedComInterface]
[Guid("7AB93C52-0E48-4750-BA9D-1A4113981847")]
internal partial interface IToastNotificationManagerStatics2 : IInspectable
{
    IToastNotificationHistory GetHistory();
}

[GeneratedComInterface]
[Guid("5CADDC63-01D3-4C97-986F-0533483FEE14")]
internal partial interface IToastNotificationHistory : IInspectable
{
    void RemoveGroup(nint group);
    void RemoveGroupWithId(nint group, nint applicationId);
    void RemoveGroupedTagWithId(nint tag, nint group, nint applicationId);
    void RemoveGroupedTag(nint tag, nint group);
    void Remove(nint tag);
    void Clear();
    void ClearWithId(nint applicationId);
}

[GeneratedComInterface]
[Guid("75927B93-03F3-41EC-91D3-6E5BAC1B38E7")]
internal partial interface IToastNotifier : IInspectable
{
    void Show(IToastNotification notification);
}

[GeneratedComInterface]
[Guid("00000001-0000-0000-C000-000000000046")]
internal unsafe partial interface IClassFactory
{
    [PreserveSig]
    int CreateInstance(nint pUnkOuter, Guid* riid, nint* ppvObject);

    [PreserveSig]
    int LockServer(int fLock);
}

[GeneratedComInterface]
[Guid("53E31837-6600-4A81-9395-75CFFE746F94")]
internal partial interface INotificationActivationCallback
{
    void Activate(nint appUserModelId, nint invokedArgs, nint data, uint dataCount);
}
#endif
