using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace QuickRoute.GPSDeviceReaders.GarminUSBReader
{
    public class APIs
    {
        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetupDiGetClassDevs(           // 1st form using a ClassGUID
           ref Guid ClassGuid,
           int Enumerator,
           IntPtr hwndParent,
           int Flags
        );

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean SetupDiEnumDeviceInterfaces(
           IntPtr hDevInfo,
           //ref DevinfoData devInfo,
           uint devInfo,
           ref Guid interfaceClassGuid,
           UInt32 memberIndex,
           ref DeviceInterfaceData deviceInterfaceData
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr lpDeviceInfoSet,
            ref DeviceInterfaceData oInterfaceData,
            IntPtr lpDeviceInterfaceDetailData,         //To get the nRequiredSize
            uint nDeviceInterfaceDetailDataSize,
            ref uint nRequiredSize,
            ref DevinfoData deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr lpDeviceInfoSet,
            ref DeviceInterfaceData oInterfaceData,
            ref DeviceInterfaceDetailData oDetailData,  //We have the size -> send struct
            uint nDeviceInterfaceDetailDataSize,
            ref uint nRequiredSize,
            ref DevinfoData deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        public static extern int CM_Get_Device_ID(
           IntPtr hDeviceInstance,
           IntPtr Buffer,
           int BufferLen,
           int ulFlags
        );

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
           string fileName,
           [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess,
           [MarshalAs(UnmanagedType.U4)] FileShare fileShare,
           IntPtr securityAttributes,
           [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
           int flags,
           IntPtr template
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool WriteFile(
            IntPtr hHandle,
            IntPtr lpBuffer,
            int nNumberOfBytesToWrite,
            out int lpNumberOfBytesWritten,
            IntPtr lpOverlapped
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToRead,
            ref int lpNumberOfBytesRead,
            IntPtr lpOverlapped
        );


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            [MarshalAs(UnmanagedType.U4)] int hDevice,
            [MarshalAs(UnmanagedType.U4)] uint dwIoControlCode,
            IntPtr lpInBuffer,
            [MarshalAs(UnmanagedType.U4)]uint nInBufferSize,
            IntPtr lpOutBuffer,
            [MarshalAs(UnmanagedType.U4)]uint nOutBufferSize,
            [MarshalAs(UnmanagedType.U4)]out int lpBytesReturned,
            [MarshalAs(UnmanagedType.U4)]uint lpOverlapped);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
            public void Init()
            {
                this.Size = Marshal.SizeOf(typeof(DeviceInterfaceData));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Overlapped
        {
            public int Internal;
            public int InternalHigh;
            public int Offset;
            public int OffsetHigh;
            public System.IntPtr hEvent;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DevinfoData
        {
            public int Size;
            public Guid ClassGuid;
            public IntPtr DevInst;
            public int Reserved;
        }

        /// <summary>
        /// Access to the path for a device
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [Flags]
        public enum DeviceFlags : int
        {
            DigCFDefault = 1,
            DigCFPresent = 0x02,         // return only devices that are currently present
            DigCFAllClasses = 4,         // gets all classes, ignores the guid...
            DigCFProfile = 8,            // gets only classes that are part of the current hardware profile
            DigCDDeviceInterface = 0x10, // Return devices that expose interfaces of the interface class that are specified by ClassGuid.
        }
        public enum CRErrorCodes
        {
            CR_SUCCESS = 0,
            CR_DEFAULT,
            CR_OUT_OF_MEMORY,
            CR_INVALID_POINTER,
            CR_INVALID_FLAG,
            CR_INVALID_DEVNODE,
            CR_INVALID_RES_DES,
            CR_INVALID_LOG_CONF,
            CR_INVALID_ARBITRATOR,
            CR_INVALID_NODELIST,
            CR_DEVNODE_HAS_REQS,
            CR_INVALID_RESOURCEID,
            CR_DLVXD_NOT_FOUND,             // WIN 95 ONLY
            CR_NO_SUCH_DEVNODE,
            CR_NO_MORE_LOG_CONF,
            CR_NO_MORE_RES_DES,
            CR_ALREADY_SUCH_DEVNODE,
            CR_INVALID_RANGE_LIST,
            CR_INVALID_RANGE,
            CR_FAILURE,
            CR_NO_SUCH_LOGICAL_DEV,
            CR_CREATE_BLOCKED,
            CR_NOT_SYSTEM_VM,            // WIN 95 ONLY
            CR_REMOVE_VETOED,
            CR_APM_VETOED,
            CR_INVALID_LOAD_TYPE,
            CR_BUFFER_SMALL,
            CR_NO_ARBITRATOR,
            CR_NO_REGISTRY_HANDLE,
            CR_REGISTRY_ERROR,
            CR_INVALID_DEVICE_ID,
            CR_INVALID_DATA,
            CR_INVALID_API,
            CR_DEVLOADER_NOT_READY,
            CR_NEED_RESTART,
            CR_NO_MORE_HW_PROFILES,
            CR_DEVICE_NOT_THERE,
            CR_NO_SUCH_VALUE,
            CR_WRONG_TYPE,
            CR_INVALID_PRIORITY,
            CR_NOT_DISABLEABLE,
            CR_FREE_RESOURCES,
            CR_QUERY_VETOED,
            CR_CANT_SHARE_IRQ,
            CR_NO_DEPENDENT,
            CR_SAME_RESOURCES,
            CR_NO_SUCH_REGISTRY_KEY,
            CR_INVALID_MACHINENAME,      // NT ONLY
            CR_REMOTE_COMM_FAILURE,      // NT ONLY
            CR_MACHINE_UNAVAILABLE,      // NT ONLY
            CR_NO_CM_SERVICES,           // NT ONLY
            CR_ACCESS_DENIED,            // NT ONLY
            CR_CALL_NOT_IMPLEMENTED,
            CR_INVALID_PROPERTY,
            CR_DEVICE_INTERFACE_ACTIVE,
            CR_NO_SUCH_DEVICE_INTERFACE,
            CR_INVALID_REFERENCE_STRING,
            CR_INVALID_CONFLICT_LIST,
            CR_INVALID_INDEX,
            CR_INVALID_STRUCTURE_SIZE,
            NUM_CR_RESULTS,
        }

        public const int CM_REGISTRY_HARDWARE = 0;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int ERROR_INVALID_DATA = 13;
        public const int ERROR_INVALID_PARAMETER = 87;
        public const int ERROR_INVALID_HANDLE = 6;
        public const int ERROR_NO_MORE_ITEMS = 259;
        public const int KEY_QUERY_VALUE = 1;
        public const int RegDisposition_OpenExisting = 1;
        public const int INVALID_HANDLE_VALUE = -1;
        public const int MAXIMUM_USB_STRING_LENGTH = 255;
        public const int ASYNC_DATA_SIZE = 64;
    }
}
