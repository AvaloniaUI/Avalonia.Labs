#pragma warning disable 108
// ReSharper disable RedundantUsingDirective
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedType.Local
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantCast
// ReSharper disable IdentifierTypo
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantUnsafeContext
// ReSharper disable RedundantBaseQualifier
// ReSharper disable EmptyStatement
// ReSharper disable RedundantAttributeParentheses
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MicroCom.Runtime;

namespace Avalonia.Labs.Notifications.Windows.WinRT
{
    internal enum TrustLevel
    {
        BaseTrust,
        PartialTrust,
        FullTrust
    }

    internal enum UserNotificationChangedKind
    {
        Added = 0,
        Removed = 1
    }

    internal enum NotificationSetting
    {
        Enabled = 0,
        DisabledForApplication = 1,
        DisabledForUser = 2,
        DisabledByGroupPolicy = 3,
        DisabledByManifest = 4
    }

    internal enum NotificationUpdateResult
    {
        Succeeded = 0,
        Failed = 1,
        NotificationNotFound = 2
    }

    internal unsafe partial interface IScheduledToastNotification : IInspectable
    {
        IXmlDocument Content { get; }

        void* DeliveryTime { get; }

        void* SnoozeInterval { get; }

        uint MaximumSnoozeCount { get; }

        void SetId(IntPtr value);
        IntPtr Id { get; }
    }

    internal unsafe partial interface IXmlDocument : IInspectable
    {
    }

    internal unsafe partial interface IToastNotificationManagerStatics : IInspectable
    {
        IToastNotifier CreateToastNotifier();
        IToastNotifier CreateToastNotifierWithId(IntPtr applicationId);
    }

    internal unsafe partial interface IToastNotification2 : IInspectable
    {
        void SetTag(IntPtr value);
        IntPtr Tag { get; }

        void SetGroup(IntPtr value);
        IntPtr Group { get; }

        void SetSuppressPopup(int value);
        int SuppressPopup { get; }
    }

    internal unsafe partial interface IToastNotificationFactory : IInspectable
    {
        IToastNotification CreateToastNotification(IXmlDocument content);
    }

    internal unsafe partial interface IXmlDocumentIO : IInspectable
    {
        void LoadXml(IntPtr xml);
    }

    internal unsafe partial interface IToastNotification : IInspectable
    {
        IXmlDocument Content { get; }

        void SetExpirationTime(void* value);
        void* ExpirationTime { get; }

        int AddDismissed(void* handler);
        void RemoveDismissed(int cookie);
        int AddActivated(void* handler);
        void RemoveActivated(int cookie);
        int AddFailed(void* handler);
        void RemoveFailed(int token);
    }

    internal unsafe partial interface INotificationData : IInspectable
    {
        void* Values { get; }

        uint SequenceNumber { get; }

        void SetSequenceNumber(uint value);
    }

    internal unsafe partial interface INotificationVisual : IInspectable
    {
        IntPtr Language { get; }

        void SetLanguage(IntPtr value);
        void* Bindings { get; }

        void* GetBinding(IntPtr templateName);
    }

    internal unsafe partial interface INotification : IInspectable
    {
        void* ExpirationTime { get; }

        void SetExpirationTime(void* value);
        INotificationVisual Visual { get; }

        void SetVisual(INotificationVisual value);
    }

    internal unsafe partial interface IToastNotificationHistory : IInspectable
    {
        void RemoveGroup(IntPtr group);
        void RemoveGroupWithId(IntPtr group, IntPtr applicationId);
        void RemoveGroupedTagWithId(IntPtr tag, IntPtr group, IntPtr applicationId);
        void RemoveGroupedTag(IntPtr tag, IntPtr group);
        void Remove(IntPtr tag);
        void Clear();
        void ClearWithId(IntPtr applicationId);
    }

    internal unsafe partial interface IInspectable : global::MicroCom.Runtime.IUnknown
    {
        void GetIids(ulong* iidCount, Guid** iids);
        IntPtr RuntimeClassName { get; }

        TrustLevel TrustLevel { get; }
    }

    internal unsafe partial interface IToastNotificationManagerForUser : IInspectable
    {
        IToastNotifier CreateToastNotifier();
        IToastNotifier CreateToastNotifierWithId(IntPtr applicationId);
        IToastNotificationHistory History { get; }

        void* User { get; }
    }

    internal unsafe partial interface IToastNotificationManagerStatics2 : IInspectable
    {
        IToastNotificationHistory History { get; }
    }

    internal unsafe partial interface IToastNotifier : IInspectable
    {
        void Show(IToastNotification notification);
        void Hide(IToastNotification notification);
        NotificationSetting Setting();
        void AddToSchedule(IScheduledToastNotification scheduledToast);
        void RemoveFromSchedule(IScheduledToastNotification scheduledToast);
        void* ScheduledToastNotifications { get; }
    }

    internal unsafe partial interface IToastNotifier2 : IInspectable
    {
        NotificationUpdateResult UpdateWithTagAndGroup(INotificationData data, IntPtr tag, IntPtr group);
        NotificationUpdateResult UpdateWithTag(INotificationData data, IntPtr tag);
    }

    internal unsafe partial interface IUserNotification : IInspectable
    {
        INotification Notification { get; }

        void* AppInfo { get; }

        uint Id { get; }

        void* CreationTime { get; }
    }

    internal unsafe partial interface IUserNotificationChangedEventArgs : IInspectable
    {
        UserNotificationChangedKind ChangeKind { get; }

        uint UserNotificationId { get; }
    }
}

namespace Avalonia.Labs.Notifications.Windows.WinRT.Impl
{
    internal unsafe partial class __MicroComIScheduledToastNotificationProxy : __MicroComIInspectableProxy, IScheduledToastNotification
    {
        public IXmlDocument Content
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetContent failed", __result);
                return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IXmlDocument>(__marshal_value, true);
            }
        }

        public void* DeliveryTime
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetDeliveryTime failed", __result);
                return value;
            }
        }

        public void* SnoozeInterval
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSnoozeInterval failed", __result);
                return value;
            }
        }

        public uint MaximumSnoozeCount
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetMaximumSnoozeCount failed", __result);
                return value;
            }
        }

        public void SetId(IntPtr value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 4])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetId failed", __result);
        }

        public IntPtr Id
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetId failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IScheduledToastNotification), new Guid("79F577F8-0DE7-48CD-9740-9B370490C838"), (p, owns) => new __MicroComIScheduledToastNotificationProxy(p, owns));
        }

        protected __MicroComIScheduledToastNotificationProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComIScheduledToastNotificationVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetContentDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetContent(void* @this, void** value)
        {
            IScheduledToastNotification __target = null;
            try
            {
                {
                    __target = (IScheduledToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Content;
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetDeliveryTimeDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetDeliveryTime(void* @this, void** value)
        {
            IScheduledToastNotification __target = null;
            try
            {
                {
                    __target = (IScheduledToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.DeliveryTime;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSnoozeIntervalDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSnoozeInterval(void* @this, void** value)
        {
            IScheduledToastNotification __target = null;
            try
            {
                {
                    __target = (IScheduledToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.SnoozeInterval;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetMaximumSnoozeCountDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetMaximumSnoozeCount(void* @this, uint* value)
        {
            IScheduledToastNotification __target = null;
            try
            {
                {
                    __target = (IScheduledToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.MaximumSnoozeCount;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetIdDelegate(void* @this, IntPtr value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetId(void* @this, IntPtr value)
        {
            IScheduledToastNotification __target = null;
            try
            {
                {
                    __target = (IScheduledToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetId(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIdDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetId(void* @this, IntPtr* value)
        {
            IScheduledToastNotification __target = null;
            try
            {
                {
                    __target = (IScheduledToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Id;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIScheduledToastNotificationVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetContent); 
#else
            base.AddMethod((GetContentDelegate)GetContent); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetDeliveryTime); 
#else
            base.AddMethod((GetDeliveryTimeDelegate)GetDeliveryTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetSnoozeInterval); 
#else
            base.AddMethod((GetSnoozeIntervalDelegate)GetSnoozeInterval); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetMaximumSnoozeCount); 
#else
            base.AddMethod((GetMaximumSnoozeCountDelegate)GetMaximumSnoozeCount); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetId); 
#else
            base.AddMethod((SetIdDelegate)SetId); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetId); 
#else
            base.AddMethod((GetIdDelegate)GetId); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IScheduledToastNotification), new __MicroComIScheduledToastNotificationVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIXmlDocumentProxy : __MicroComIInspectableProxy, IXmlDocument
    {
        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IXmlDocument), new Guid("F7F3A506-1E87-42D6-BCFB-B8C809FA5494"), (p, owns) => new __MicroComIXmlDocumentProxy(p, owns));
        }

        protected __MicroComIXmlDocumentProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 0;
    }

    unsafe class __MicroComIXmlDocumentVTable : __MicroComIInspectableVTable
    {
        protected __MicroComIXmlDocumentVTable()
        {
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IXmlDocument), new __MicroComIXmlDocumentVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotificationManagerStaticsProxy : __MicroComIInspectableProxy, IToastNotificationManagerStatics
    {
        public IToastNotifier CreateToastNotifier()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateToastNotifier failed", __result);
            return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotifier>(__marshal_result, true);
        }

        public IToastNotifier CreateToastNotifierWithId(IntPtr applicationId)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, applicationId, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateToastNotifierWithId failed", __result);
            return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotifier>(__marshal_result, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotificationManagerStatics), new Guid("50AC103F-D235-4598-BBEF-98FE4D1A3AD4"), (p, owns) => new __MicroComIToastNotificationManagerStaticsProxy(p, owns));
        }

        protected __MicroComIToastNotificationManagerStaticsProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIToastNotificationManagerStaticsVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateToastNotifierDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateToastNotifier(void* @this, void** result)
        {
            IToastNotificationManagerStatics __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerStatics)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateToastNotifier();
                        *result = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateToastNotifierWithIdDelegate(void* @this, IntPtr applicationId, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateToastNotifierWithId(void* @this, IntPtr applicationId, void** result)
        {
            IToastNotificationManagerStatics __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerStatics)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateToastNotifierWithId(applicationId);
                        *result = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotificationManagerStaticsVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateToastNotifier); 
#else
            base.AddMethod((CreateToastNotifierDelegate)CreateToastNotifier); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&CreateToastNotifierWithId); 
#else
            base.AddMethod((CreateToastNotifierWithIdDelegate)CreateToastNotifierWithId); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotificationManagerStatics), new __MicroComIToastNotificationManagerStaticsVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotification2Proxy : __MicroComIInspectableProxy, IToastNotification2
    {
        public void SetTag(IntPtr value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 0])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetTag failed", __result);
        }

        public IntPtr Tag
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTag failed", __result);
                return value;
            }
        }

        public void SetGroup(IntPtr value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 2])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetGroup failed", __result);
        }

        public IntPtr Group
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetGroup failed", __result);
                return value;
            }
        }

        public void SetSuppressPopup(int value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 4])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSuppressPopup failed", __result);
        }

        public int SuppressPopup
        {
            get
            {
                int __result;
                int value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSuppressPopup failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotification2), new Guid("9DFB9FD1-143A-490E-90BF-B9FBA7132DE7"), (p, owns) => new __MicroComIToastNotification2Proxy(p, owns));
        }

        protected __MicroComIToastNotification2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComIToastNotification2VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetTagDelegate(void* @this, IntPtr value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetTag(void* @this, IntPtr value)
        {
            IToastNotification2 __target = null;
            try
            {
                {
                    __target = (IToastNotification2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetTag(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTagDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTag(void* @this, IntPtr* value)
        {
            IToastNotification2 __target = null;
            try
            {
                {
                    __target = (IToastNotification2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Tag;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetGroupDelegate(void* @this, IntPtr value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetGroup(void* @this, IntPtr value)
        {
            IToastNotification2 __target = null;
            try
            {
                {
                    __target = (IToastNotification2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetGroup(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetGroupDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetGroup(void* @this, IntPtr* value)
        {
            IToastNotification2 __target = null;
            try
            {
                {
                    __target = (IToastNotification2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Group;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSuppressPopupDelegate(void* @this, int value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSuppressPopup(void* @this, int value)
        {
            IToastNotification2 __target = null;
            try
            {
                {
                    __target = (IToastNotification2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSuppressPopup(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSuppressPopupDelegate(void* @this, int* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSuppressPopup(void* @this, int* value)
        {
            IToastNotification2 __target = null;
            try
            {
                {
                    __target = (IToastNotification2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.SuppressPopup;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotification2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetTag); 
#else
            base.AddMethod((SetTagDelegate)SetTag); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetTag); 
#else
            base.AddMethod((GetTagDelegate)GetTag); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetGroup); 
#else
            base.AddMethod((SetGroupDelegate)SetGroup); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetGroup); 
#else
            base.AddMethod((GetGroupDelegate)GetGroup); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&SetSuppressPopup); 
#else
            base.AddMethod((SetSuppressPopupDelegate)SetSuppressPopup); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int*, int>)&GetSuppressPopup); 
#else
            base.AddMethod((GetSuppressPopupDelegate)GetSuppressPopup); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotification2), new __MicroComIToastNotification2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotificationFactoryProxy : __MicroComIInspectableProxy, IToastNotificationFactory
    {
        public IToastNotification CreateToastNotification(IXmlDocument content)
        {
            int __result;
            void* __marshal_value = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(content), &__marshal_value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateToastNotification failed", __result);
            return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotification>(__marshal_value, true);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotificationFactory), new Guid("04124B20-82C6-4229-B109-FD9ED4662B53"), (p, owns) => new __MicroComIToastNotificationFactoryProxy(p, owns));
        }

        protected __MicroComIToastNotificationFactoryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIToastNotificationFactoryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateToastNotificationDelegate(void* @this, void* content, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateToastNotification(void* @this, void* content, void** value)
        {
            IToastNotificationFactory __target = null;
            try
            {
                {
                    __target = (IToastNotificationFactory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateToastNotification(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IXmlDocument>(content, false));
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotificationFactoryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, void**, int>)&CreateToastNotification); 
#else
            base.AddMethod((CreateToastNotificationDelegate)CreateToastNotification); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotificationFactory), new __MicroComIToastNotificationFactoryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIXmlDocumentIOProxy : __MicroComIInspectableProxy, IXmlDocumentIO
    {
        public void LoadXml(IntPtr xml)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 0])(PPV, xml);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("LoadXml failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IXmlDocumentIO), new Guid("6CD0E74E-EE65-4489-9EBF-CA43E87BA637"), (p, owns) => new __MicroComIXmlDocumentIOProxy(p, owns));
        }

        protected __MicroComIXmlDocumentIOProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIXmlDocumentIOVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int LoadXmlDelegate(void* @this, IntPtr xml);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int LoadXml(void* @this, IntPtr xml)
        {
            IXmlDocumentIO __target = null;
            try
            {
                {
                    __target = (IXmlDocumentIO)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.LoadXml(xml);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIXmlDocumentIOVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&LoadXml); 
#else
            base.AddMethod((LoadXmlDelegate)LoadXml); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IXmlDocumentIO), new __MicroComIXmlDocumentIOVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotificationProxy : __MicroComIInspectableProxy, IToastNotification
    {
        public IXmlDocument Content
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetContent failed", __result);
                return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IXmlDocument>(__marshal_value, true);
            }
        }

        public void SetExpirationTime(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetExpirationTime failed", __result);
        }

        public void* ExpirationTime
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetExpirationTime failed", __result);
                return value;
            }
        }

        public int AddDismissed(void* handler)
        {
            int __result;
            int cookie = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, handler, &cookie);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddDismissed failed", __result);
            return cookie;
        }

        public void RemoveDismissed(int cookie)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 4])(PPV, cookie);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveDismissed failed", __result);
        }

        public int AddActivated(void* handler)
        {
            int __result;
            int cookie = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, handler, &cookie);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddActivated failed", __result);
            return cookie;
        }

        public void RemoveActivated(int cookie)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 6])(PPV, cookie);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveActivated failed", __result);
        }

        public int AddFailed(void* handler)
        {
            int __result;
            int token = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 7])(PPV, handler, &token);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddFailed failed", __result);
            return token;
        }

        public void RemoveFailed(int token)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int, int>)(*PPV)[base.VTableSize + 8])(PPV, token);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveFailed failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotification), new Guid("997E2675-059E-4E60-8B06-1760917C8B80"), (p, owns) => new __MicroComIToastNotificationProxy(p, owns));
        }

        protected __MicroComIToastNotificationProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 9;
    }

    unsafe class __MicroComIToastNotificationVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetContentDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetContent(void* @this, void** value)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Content;
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetExpirationTimeDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetExpirationTime(void* @this, void* value)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetExpirationTime(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetExpirationTimeDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetExpirationTime(void* @this, void** value)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ExpirationTime;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddDismissedDelegate(void* @this, void* handler, int* cookie);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddDismissed(void* @this, void* handler, int* cookie)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AddDismissed(handler);
                        *cookie = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveDismissedDelegate(void* @this, int cookie);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveDismissed(void* @this, int cookie)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveDismissed(cookie);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddActivatedDelegate(void* @this, void* handler, int* cookie);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddActivated(void* @this, void* handler, int* cookie)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AddActivated(handler);
                        *cookie = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveActivatedDelegate(void* @this, int cookie);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveActivated(void* @this, int cookie)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveActivated(cookie);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddFailedDelegate(void* @this, void* handler, int* token);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddFailed(void* @this, void* handler, int* token)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AddFailed(handler);
                        *token = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveFailedDelegate(void* @this, int token);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveFailed(void* @this, int token)
        {
            IToastNotification __target = null;
            try
            {
                {
                    __target = (IToastNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveFailed(token);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotificationVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetContent); 
#else
            base.AddMethod((GetContentDelegate)GetContent); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetExpirationTime); 
#else
            base.AddMethod((SetExpirationTimeDelegate)SetExpirationTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetExpirationTime); 
#else
            base.AddMethod((GetExpirationTimeDelegate)GetExpirationTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int*, int>)&AddDismissed); 
#else
            base.AddMethod((AddDismissedDelegate)AddDismissed); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&RemoveDismissed); 
#else
            base.AddMethod((RemoveDismissedDelegate)RemoveDismissed); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int*, int>)&AddActivated); 
#else
            base.AddMethod((AddActivatedDelegate)AddActivated); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&RemoveActivated); 
#else
            base.AddMethod((RemoveActivatedDelegate)RemoveActivated); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int*, int>)&AddFailed); 
#else
            base.AddMethod((AddFailedDelegate)AddFailed); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int, int>)&RemoveFailed); 
#else
            base.AddMethod((RemoveFailedDelegate)RemoveFailed); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotification), new __MicroComIToastNotificationVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComINotificationDataProxy : __MicroComIInspectableProxy, INotificationData
    {
        public void* Values
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetValues failed", __result);
                return value;
            }
        }

        public uint SequenceNumber
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetSequenceNumber failed", __result);
                return value;
            }
        }

        public void SetSequenceNumber(uint value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, uint, int>)(*PPV)[base.VTableSize + 2])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetSequenceNumber failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(INotificationData), new Guid("9FFD2312-9D6A-4AAF-B6AC-FF17F0C1F280"), (p, owns) => new __MicroComINotificationDataProxy(p, owns));
        }

        protected __MicroComINotificationDataProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComINotificationDataVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetValuesDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetValues(void* @this, void** value)
        {
            INotificationData __target = null;
            try
            {
                {
                    __target = (INotificationData)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Values;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetSequenceNumberDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetSequenceNumber(void* @this, uint* value)
        {
            INotificationData __target = null;
            try
            {
                {
                    __target = (INotificationData)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.SequenceNumber;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetSequenceNumberDelegate(void* @this, uint value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetSequenceNumber(void* @this, uint value)
        {
            INotificationData __target = null;
            try
            {
                {
                    __target = (INotificationData)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetSequenceNumber(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComINotificationDataVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetValues); 
#else
            base.AddMethod((GetValuesDelegate)GetValues); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetSequenceNumber); 
#else
            base.AddMethod((GetSequenceNumberDelegate)GetSequenceNumber); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint, int>)&SetSequenceNumber); 
#else
            base.AddMethod((SetSequenceNumberDelegate)SetSequenceNumber); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(INotificationData), new __MicroComINotificationDataVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComINotificationVisualProxy : __MicroComIInspectableProxy, INotificationVisual
    {
        public IntPtr Language
        {
            get
            {
                int __result;
                IntPtr value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetLanguage failed", __result);
                return value;
            }
        }

        public void SetLanguage(IntPtr value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetLanguage failed", __result);
        }

        public void* Bindings
        {
            get
            {
                int __result;
                void* result = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &result);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetBindings failed", __result);
                return result;
            }
        }

        public void* GetBinding(IntPtr templateName)
        {
            int __result;
            void* result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, templateName, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetBinding failed", __result);
            return result;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(INotificationVisual), new Guid("68835B8E-AA56-4E11-86D3-5F9A6957BC5B"), (p, owns) => new __MicroComINotificationVisualProxy(p, owns));
        }

        protected __MicroComINotificationVisualProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 4;
    }

    unsafe class __MicroComINotificationVisualVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetLanguageDelegate(void* @this, IntPtr* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetLanguage(void* @this, IntPtr* value)
        {
            INotificationVisual __target = null;
            try
            {
                {
                    __target = (INotificationVisual)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Language;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetLanguageDelegate(void* @this, IntPtr value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetLanguage(void* @this, IntPtr value)
        {
            INotificationVisual __target = null;
            try
            {
                {
                    __target = (INotificationVisual)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetLanguage(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBindingsDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBindings(void* @this, void** result)
        {
            INotificationVisual __target = null;
            try
            {
                {
                    __target = (INotificationVisual)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Bindings;
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetBindingDelegate(void* @this, IntPtr templateName, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetBinding(void* @this, IntPtr templateName, void** result)
        {
            INotificationVisual __target = null;
            try
            {
                {
                    __target = (INotificationVisual)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.GetBinding(templateName);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComINotificationVisualVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetLanguage); 
#else
            base.AddMethod((GetLanguageDelegate)GetLanguage); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&SetLanguage); 
#else
            base.AddMethod((SetLanguageDelegate)SetLanguage); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetBindings); 
#else
            base.AddMethod((GetBindingsDelegate)GetBindings); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&GetBinding); 
#else
            base.AddMethod((GetBindingDelegate)GetBinding); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(INotificationVisual), new __MicroComINotificationVisualVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComINotificationProxy : __MicroComIInspectableProxy, INotification
    {
        public void* ExpirationTime
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetExpirationTime failed", __result);
                return value;
            }
        }

        public void SetExpirationTime(void* value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetExpirationTime failed", __result);
        }

        public INotificationVisual Visual
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetVisual failed", __result);
                return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<INotificationVisual>(__marshal_value, true);
            }
        }

        public void SetVisual(INotificationVisual value)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(value));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("SetVisual failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(INotification), new Guid("108037FE-EB76-4F82-97BC-DA07530A2E20"), (p, owns) => new __MicroComINotificationProxy(p, owns));
        }

        protected __MicroComINotificationProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 4;
    }

    unsafe class __MicroComINotificationVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetExpirationTimeDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetExpirationTime(void* @this, void** value)
        {
            INotification __target = null;
            try
            {
                {
                    __target = (INotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ExpirationTime;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetExpirationTimeDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetExpirationTime(void* @this, void* value)
        {
            INotification __target = null;
            try
            {
                {
                    __target = (INotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetExpirationTime(value);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetVisualDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetVisual(void* @this, void** value)
        {
            INotification __target = null;
            try
            {
                {
                    __target = (INotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Visual;
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SetVisualDelegate(void* @this, void* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int SetVisual(void* @this, void* value)
        {
            INotification __target = null;
            try
            {
                {
                    __target = (INotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.SetVisual(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<INotificationVisual>(value, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComINotificationVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetExpirationTime); 
#else
            base.AddMethod((GetExpirationTimeDelegate)GetExpirationTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetExpirationTime); 
#else
            base.AddMethod((SetExpirationTimeDelegate)SetExpirationTime); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetVisual); 
#else
            base.AddMethod((GetVisualDelegate)GetVisual); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&SetVisual); 
#else
            base.AddMethod((SetVisualDelegate)SetVisual); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(INotification), new __MicroComINotificationVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotificationHistoryProxy : __MicroComIInspectableProxy, IToastNotificationHistory
    {
        public void RemoveGroup(IntPtr group)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 0])(PPV, group);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveGroup failed", __result);
        }

        public void RemoveGroupWithId(IntPtr group, IntPtr applicationId)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)(*PPV)[base.VTableSize + 1])(PPV, group, applicationId);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveGroupWithId failed", __result);
        }

        public void RemoveGroupedTagWithId(IntPtr tag, IntPtr group, IntPtr applicationId)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, int>)(*PPV)[base.VTableSize + 2])(PPV, tag, group, applicationId);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveGroupedTagWithId failed", __result);
        }

        public void RemoveGroupedTag(IntPtr tag, IntPtr group)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)(*PPV)[base.VTableSize + 3])(PPV, tag, group);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveGroupedTag failed", __result);
        }

        public void Remove(IntPtr tag)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 4])(PPV, tag);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Remove failed", __result);
        }

        public void Clear()
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, int>)(*PPV)[base.VTableSize + 5])(PPV);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Clear failed", __result);
        }

        public void ClearWithId(IntPtr applicationId)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)(*PPV)[base.VTableSize + 6])(PPV, applicationId);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("ClearWithId failed", __result);
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotificationHistory), new Guid("5CADDC63-01D3-4C97-986F-0533483FEE14"), (p, owns) => new __MicroComIToastNotificationHistoryProxy(p, owns));
        }

        protected __MicroComIToastNotificationHistoryProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 7;
    }

    unsafe class __MicroComIToastNotificationHistoryVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveGroupDelegate(void* @this, IntPtr group);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveGroup(void* @this, IntPtr group)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveGroup(group);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveGroupWithIdDelegate(void* @this, IntPtr group, IntPtr applicationId);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveGroupWithId(void* @this, IntPtr group, IntPtr applicationId)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveGroupWithId(group, applicationId);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveGroupedTagWithIdDelegate(void* @this, IntPtr tag, IntPtr group, IntPtr applicationId);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveGroupedTagWithId(void* @this, IntPtr tag, IntPtr group, IntPtr applicationId)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveGroupedTagWithId(tag, group, applicationId);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveGroupedTagDelegate(void* @this, IntPtr tag, IntPtr group);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveGroupedTag(void* @this, IntPtr tag, IntPtr group)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveGroupedTag(tag, group);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveDelegate(void* @this, IntPtr tag);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Remove(void* @this, IntPtr tag)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Remove(tag);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ClearDelegate(void* @this);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Clear(void* @this)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Clear();
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ClearWithIdDelegate(void* @this, IntPtr applicationId);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int ClearWithId(void* @this, IntPtr applicationId)
        {
            IToastNotificationHistory __target = null;
            try
            {
                {
                    __target = (IToastNotificationHistory)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.ClearWithId(applicationId);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotificationHistoryVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&RemoveGroup); 
#else
            base.AddMethod((RemoveGroupDelegate)RemoveGroup); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)&RemoveGroupWithId); 
#else
            base.AddMethod((RemoveGroupWithIdDelegate)RemoveGroupWithId); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, int>)&RemoveGroupedTagWithId); 
#else
            base.AddMethod((RemoveGroupedTagWithIdDelegate)RemoveGroupedTagWithId); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, int>)&RemoveGroupedTag); 
#else
            base.AddMethod((RemoveGroupedTagDelegate)RemoveGroupedTag); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&Remove); 
#else
            base.AddMethod((RemoveDelegate)Remove); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, int>)&Clear); 
#else
            base.AddMethod((ClearDelegate)Clear); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, int>)&ClearWithId); 
#else
            base.AddMethod((ClearWithIdDelegate)ClearWithId); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotificationHistory), new __MicroComIToastNotificationHistoryVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIInspectableProxy : global::MicroCom.Runtime.MicroComProxyBase, IInspectable
    {
        public void GetIids(ulong* iidCount, Guid** iids)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, iidCount, iids);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("GetIids failed", __result);
        }

        public IntPtr RuntimeClassName
        {
            get
            {
                int __result;
                IntPtr className = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &className);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetRuntimeClassName failed", __result);
                return className;
            }
        }

        public TrustLevel TrustLevel
        {
            get
            {
                int __result;
                TrustLevel trustLevel = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &trustLevel);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetTrustLevel failed", __result);
                return trustLevel;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IInspectable), new Guid("AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90"), (p, owns) => new __MicroComIInspectableProxy(p, owns));
        }

        protected __MicroComIInspectableProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 3;
    }

    unsafe class __MicroComIInspectableVTable : global::MicroCom.Runtime.MicroComVtblBase
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIidsDelegate(void* @this, ulong* iidCount, Guid** iids);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetIids(void* @this, ulong* iidCount, Guid** iids)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.GetIids(iidCount, iids);
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetRuntimeClassNameDelegate(void* @this, IntPtr* className);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetRuntimeClassName(void* @this, IntPtr* className)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.RuntimeClassName;
                        *className = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetTrustLevelDelegate(void* @this, TrustLevel* trustLevel);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetTrustLevel(void* @this, TrustLevel* trustLevel)
        {
            IInspectable __target = null;
            try
            {
                {
                    __target = (IInspectable)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.TrustLevel;
                        *trustLevel = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIInspectableVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, ulong*, Guid**, int>)&GetIids); 
#else
            base.AddMethod((GetIidsDelegate)GetIids); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr*, int>)&GetRuntimeClassName); 
#else
            base.AddMethod((GetRuntimeClassNameDelegate)GetRuntimeClassName); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, TrustLevel*, int>)&GetTrustLevel); 
#else
            base.AddMethod((GetTrustLevelDelegate)GetTrustLevel); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IInspectable), new __MicroComIInspectableVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotificationManagerForUserProxy : __MicroComIInspectableProxy, IToastNotificationManagerForUser
    {
        public IToastNotifier CreateToastNotifier()
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateToastNotifier failed", __result);
            return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotifier>(__marshal_result, true);
        }

        public IToastNotifier CreateToastNotifierWithId(IntPtr applicationId)
        {
            int __result;
            void* __marshal_result = null;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, applicationId, &__marshal_result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("CreateToastNotifierWithId failed", __result);
            return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotifier>(__marshal_result, true);
        }

        public IToastNotificationHistory History
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHistory failed", __result);
                return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotificationHistory>(__marshal_value, true);
            }
        }

        public void* User
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetUser failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotificationManagerForUser), new Guid("79AB57F6-43FE-487B-8A7F-99567200AE94"), (p, owns) => new __MicroComIToastNotificationManagerForUserProxy(p, owns));
        }

        protected __MicroComIToastNotificationManagerForUserProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 4;
    }

    unsafe class __MicroComIToastNotificationManagerForUserVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateToastNotifierDelegate(void* @this, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateToastNotifier(void* @this, void** result)
        {
            IToastNotificationManagerForUser __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerForUser)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateToastNotifier();
                        *result = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int CreateToastNotifierWithIdDelegate(void* @this, IntPtr applicationId, void** result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int CreateToastNotifierWithId(void* @this, IntPtr applicationId, void** result)
        {
            IToastNotificationManagerForUser __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerForUser)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreateToastNotifierWithId(applicationId);
                        *result = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHistoryDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHistory(void* @this, void** value)
        {
            IToastNotificationManagerForUser __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerForUser)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.History;
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUserDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUser(void* @this, void** value)
        {
            IToastNotificationManagerForUser __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerForUser)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.User;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotificationManagerForUserVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&CreateToastNotifier); 
#else
            base.AddMethod((CreateToastNotifierDelegate)CreateToastNotifier); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, IntPtr, void**, int>)&CreateToastNotifierWithId); 
#else
            base.AddMethod((CreateToastNotifierWithIdDelegate)CreateToastNotifierWithId); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetHistory); 
#else
            base.AddMethod((GetHistoryDelegate)GetHistory); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetUser); 
#else
            base.AddMethod((GetUserDelegate)GetUser); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotificationManagerForUser), new __MicroComIToastNotificationManagerForUserVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotificationManagerStatics2Proxy : __MicroComIInspectableProxy, IToastNotificationManagerStatics2
    {
        public IToastNotificationHistory History
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetHistory failed", __result);
                return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotificationHistory>(__marshal_value, true);
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotificationManagerStatics2), new Guid("7AB93C52-0E48-4750-BA9D-1A4113981847"), (p, owns) => new __MicroComIToastNotificationManagerStatics2Proxy(p, owns));
        }

        protected __MicroComIToastNotificationManagerStatics2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 1;
    }

    unsafe class __MicroComIToastNotificationManagerStatics2VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetHistoryDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetHistory(void* @this, void** value)
        {
            IToastNotificationManagerStatics2 __target = null;
            try
            {
                {
                    __target = (IToastNotificationManagerStatics2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.History;
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotificationManagerStatics2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetHistory); 
#else
            base.AddMethod((GetHistoryDelegate)GetHistory); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotificationManagerStatics2), new __MicroComIToastNotificationManagerStatics2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotifierProxy : __MicroComIInspectableProxy, IToastNotifier
    {
        public void Show(IToastNotification notification)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(notification));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Show failed", __result);
        }

        public void Hide(IToastNotification notification)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(notification));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Hide failed", __result);
        }

        public NotificationSetting Setting()
        {
            int __result;
            NotificationSetting value = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("Setting failed", __result);
            return value;
        }

        public void AddToSchedule(IScheduledToastNotification scheduledToast)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(scheduledToast));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("AddToSchedule failed", __result);
        }

        public void RemoveFromSchedule(IScheduledToastNotification scheduledToast)
        {
            int __result;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 4])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(scheduledToast));
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("RemoveFromSchedule failed", __result);
        }

        public void* ScheduledToastNotifications
        {
            get
            {
                int __result;
                void* scheduledToasts = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 5])(PPV, &scheduledToasts);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetScheduledToastNotifications failed", __result);
                return scheduledToasts;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotifier), new Guid("75927B93-03F3-41EC-91D3-6E5BAC1B38E7"), (p, owns) => new __MicroComIToastNotifierProxy(p, owns));
        }

        protected __MicroComIToastNotifierProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 6;
    }

    unsafe class __MicroComIToastNotifierVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int ShowDelegate(void* @this, void* notification);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Show(void* @this, void* notification)
        {
            IToastNotifier __target = null;
            try
            {
                {
                    __target = (IToastNotifier)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Show(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotification>(notification, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int HideDelegate(void* @this, void* notification);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Hide(void* @this, void* notification)
        {
            IToastNotifier __target = null;
            try
            {
                {
                    __target = (IToastNotifier)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.Hide(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IToastNotification>(notification, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int SettingDelegate(void* @this, NotificationSetting* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int Setting(void* @this, NotificationSetting* value)
        {
            IToastNotifier __target = null;
            try
            {
                {
                    __target = (IToastNotifier)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Setting();
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int AddToScheduleDelegate(void* @this, void* scheduledToast);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int AddToSchedule(void* @this, void* scheduledToast)
        {
            IToastNotifier __target = null;
            try
            {
                {
                    __target = (IToastNotifier)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.AddToSchedule(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IScheduledToastNotification>(scheduledToast, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int RemoveFromScheduleDelegate(void* @this, void* scheduledToast);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int RemoveFromSchedule(void* @this, void* scheduledToast)
        {
            IToastNotifier __target = null;
            try
            {
                {
                    __target = (IToastNotifier)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    __target.RemoveFromSchedule(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<IScheduledToastNotification>(scheduledToast, false));
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetScheduledToastNotificationsDelegate(void* @this, void** scheduledToasts);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetScheduledToastNotifications(void* @this, void** scheduledToasts)
        {
            IToastNotifier __target = null;
            try
            {
                {
                    __target = (IToastNotifier)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ScheduledToastNotifications;
                        *scheduledToasts = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotifierVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&Show); 
#else
            base.AddMethod((ShowDelegate)Show); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&Hide); 
#else
            base.AddMethod((HideDelegate)Hide); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, NotificationSetting*, int>)&Setting); 
#else
            base.AddMethod((SettingDelegate)Setting); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&AddToSchedule); 
#else
            base.AddMethod((AddToScheduleDelegate)AddToSchedule); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, int>)&RemoveFromSchedule); 
#else
            base.AddMethod((RemoveFromScheduleDelegate)RemoveFromSchedule); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetScheduledToastNotifications); 
#else
            base.AddMethod((GetScheduledToastNotificationsDelegate)GetScheduledToastNotifications); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotifier), new __MicroComIToastNotifierVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIToastNotifier2Proxy : __MicroComIInspectableProxy, IToastNotifier2
    {
        public NotificationUpdateResult UpdateWithTagAndGroup(INotificationData data, IntPtr tag, IntPtr group)
        {
            int __result;
            NotificationUpdateResult result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, IntPtr, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(data), tag, group, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("UpdateWithTagAndGroup failed", __result);
            return result;
        }

        public NotificationUpdateResult UpdateWithTag(INotificationData data, IntPtr tag)
        {
            int __result;
            NotificationUpdateResult result = default;
            __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(data), tag, &result);
            if (__result != 0)
                throw new System.Runtime.InteropServices.COMException("UpdateWithTag failed", __result);
            return result;
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IToastNotifier2), new Guid("354389C6-7C01-4BD5-9C20-604340CD2B74"), (p, owns) => new __MicroComIToastNotifier2Proxy(p, owns));
        }

        protected __MicroComIToastNotifier2Proxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIToastNotifier2VTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int UpdateWithTagAndGroupDelegate(void* @this, void* data, IntPtr tag, IntPtr group, NotificationUpdateResult* result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int UpdateWithTagAndGroup(void* @this, void* data, IntPtr tag, IntPtr group, NotificationUpdateResult* result)
        {
            IToastNotifier2 __target = null;
            try
            {
                {
                    __target = (IToastNotifier2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UpdateWithTagAndGroup(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<INotificationData>(data, false), tag, group);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int UpdateWithTagDelegate(void* @this, void* data, IntPtr tag, NotificationUpdateResult* result);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int UpdateWithTag(void* @this, void* data, IntPtr tag, NotificationUpdateResult* result)
        {
            IToastNotifier2 __target = null;
            try
            {
                {
                    __target = (IToastNotifier2)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UpdateWithTag(global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<INotificationData>(data, false), tag);
                        *result = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIToastNotifier2VTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, IntPtr, NotificationUpdateResult*, int>)&UpdateWithTagAndGroup); 
#else
            base.AddMethod((UpdateWithTagAndGroupDelegate)UpdateWithTagAndGroup); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, NotificationUpdateResult*, int>)&UpdateWithTag); 
#else
            base.AddMethod((UpdateWithTagDelegate)UpdateWithTag); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IToastNotifier2), new __MicroComIToastNotifier2VTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIUserNotificationProxy : __MicroComIInspectableProxy, IUserNotification
    {
        public INotification Notification
        {
            get
            {
                int __result;
                void* __marshal_value = null;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &__marshal_value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetNotification failed", __result);
                return global::MicroCom.Runtime.MicroComRuntime.CreateProxyOrNullFor<INotification>(__marshal_value, true);
            }
        }

        public void* AppInfo
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetAppInfo failed", __result);
                return value;
            }
        }

        public uint Id
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 2])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetId failed", __result);
                return value;
            }
        }

        public void* CreationTime
        {
            get
            {
                int __result;
                void* value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 3])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetCreationTime failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IUserNotification), new Guid("ADF7E52F-4E53-42D5-9C33-EB5EA515B23E"), (p, owns) => new __MicroComIUserNotificationProxy(p, owns));
        }

        protected __MicroComIUserNotificationProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 4;
    }

    unsafe class __MicroComIUserNotificationVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetNotificationDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetNotification(void* @this, void** value)
        {
            IUserNotification __target = null;
            try
            {
                {
                    __target = (IUserNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Notification;
                        *value = global::MicroCom.Runtime.MicroComRuntime.GetNativePointer(__result, true);
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetAppInfoDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetAppInfo(void* @this, void** value)
        {
            IUserNotification __target = null;
            try
            {
                {
                    __target = (IUserNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.AppInfo;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetIdDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetId(void* @this, uint* value)
        {
            IUserNotification __target = null;
            try
            {
                {
                    __target = (IUserNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.Id;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetCreationTimeDelegate(void* @this, void** value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetCreationTime(void* @this, void** value)
        {
            IUserNotification __target = null;
            try
            {
                {
                    __target = (IUserNotification)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.CreationTime;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIUserNotificationVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetNotification); 
#else
            base.AddMethod((GetNotificationDelegate)GetNotification); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetAppInfo); 
#else
            base.AddMethod((GetAppInfoDelegate)GetAppInfo); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetId); 
#else
            base.AddMethod((GetIdDelegate)GetId); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, void**, int>)&GetCreationTime); 
#else
            base.AddMethod((GetCreationTimeDelegate)GetCreationTime); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IUserNotification), new __MicroComIUserNotificationVTable().CreateVTable());
    }

    internal unsafe partial class __MicroComIUserNotificationChangedEventArgsProxy : __MicroComIInspectableProxy, IUserNotificationChangedEventArgs
    {
        public UserNotificationChangedKind ChangeKind
        {
            get
            {
                int __result;
                UserNotificationChangedKind value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 0])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetChangeKind failed", __result);
                return value;
            }
        }

        public uint UserNotificationId
        {
            get
            {
                int __result;
                uint value = default;
                __result = (int)((delegate* unmanaged[Stdcall]<void*, void*, int>)(*PPV)[base.VTableSize + 1])(PPV, &value);
                if (__result != 0)
                    throw new System.Runtime.InteropServices.COMException("GetUserNotificationId failed", __result);
                return value;
            }
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit()
        {
            global::MicroCom.Runtime.MicroComRuntime.Register(typeof(IUserNotificationChangedEventArgs), new Guid("B6BD6839-79CF-4B25-82C0-0CE1EEF81F8C"), (p, owns) => new __MicroComIUserNotificationChangedEventArgsProxy(p, owns));
        }

        protected __MicroComIUserNotificationChangedEventArgsProxy(IntPtr nativePointer, bool ownsHandle) : base(nativePointer, ownsHandle)
        {
        }

        protected override int VTableSize => base.VTableSize + 2;
    }

    unsafe class __MicroComIUserNotificationChangedEventArgsVTable : __MicroComIInspectableVTable
    {
        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetChangeKindDelegate(void* @this, UserNotificationChangedKind* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetChangeKind(void* @this, UserNotificationChangedKind* value)
        {
            IUserNotificationChangedEventArgs __target = null;
            try
            {
                {
                    __target = (IUserNotificationChangedEventArgs)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.ChangeKind;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
        delegate int GetUserNotificationIdDelegate(void* @this, uint* value);
#if NET5_0_OR_GREATER
        [System.Runtime.InteropServices.UnmanagedCallersOnly(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })] 
#endif
        static int GetUserNotificationId(void* @this, uint* value)
        {
            IUserNotificationChangedEventArgs __target = null;
            try
            {
                {
                    __target = (IUserNotificationChangedEventArgs)global::MicroCom.Runtime.MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
                    {
                        var __result = __target.UserNotificationId;
                        *value = __result;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException __com_exception__)
            {
                return __com_exception__.ErrorCode;
            }
            catch (System.Exception __exception__)
            {
                global::MicroCom.Runtime.MicroComRuntime.UnhandledException(__target, __exception__);
                return unchecked((int)0x80004005u);
            }

            return 0;
        }

        protected __MicroComIUserNotificationChangedEventArgsVTable()
        {
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, UserNotificationChangedKind*, int>)&GetChangeKind); 
#else
            base.AddMethod((GetChangeKindDelegate)GetChangeKind); 
#endif
#if NET5_0_OR_GREATER
            base.AddMethod((delegate* unmanaged[Stdcall]<void*, uint*, int>)&GetUserNotificationId); 
#else
            base.AddMethod((GetUserNotificationIdDelegate)GetUserNotificationId); 
#endif
        }

        [System.Runtime.CompilerServices.ModuleInitializer()]
        internal static void __MicroComModuleInit() => global::MicroCom.Runtime.MicroComRuntime.RegisterVTable(typeof(IUserNotificationChangedEventArgs), new __MicroComIUserNotificationChangedEventArgsVTable().CreateVTable());
    }
}