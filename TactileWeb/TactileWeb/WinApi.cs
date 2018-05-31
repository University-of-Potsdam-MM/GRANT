using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32;

namespace TactileWeb
{


    /// <summary>Windows API calls</summary>
    public static class WinApi
    {

        public enum SPDRP
        {
            SPDRP_DEVICEDESC                    = 0x00000000, // DeviceDesc (R/W)
            SPDRP_HARDWAREID                    = 0x00000001, // HardwareID (R/W)
            SPDRP_COMPATIBLEIDS                 = 0x00000002, // CompatibleIDs (R/W)
            SPDRP_UNUSED0                       = 0x00000003, // unused
            SPDRP_SERVICE                       = 0x00000004, // Service (R/W)
            SPDRP_UNUSED1                       = 0x00000005, // unused
            SPDRP_UNUSED2                       = 0x00000006, // unused
            SPDRP_CLASS                         = 0x00000007, // Class (R--tied to ClassGUID)
            SPDRP_CLASSGUID                     = 0x00000008, // ClassGUID (R/W)
            SPDRP_DRIVER                        = 0x00000009, // Driver (R/W)
            SPDRP_CONFIGFLAGS                   = 0x0000000A, // ConfigFlags (R/W)
            SPDRP_MFG                           = 0x0000000B, // Mfg (R/W)
            SPDRP_FRIENDLYNAME                  = 0x0000000C, // FriendlyName (R/W)
            SPDRP_LOCATION_INFORMATION          = 0x0000000D, // LocationInformation (R/W)
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME   = 0x0000000E, // PhysicalDeviceObjectName (R)
            SPDRP_CAPABILITIES                  = 0x0000000F, // Capabilities (R)
            SPDRP_UI_NUMBER                     = 0x00000010, // UiNumber (R)
            SPDRP_UPPERFILTERS                  = 0x00000011, // UpperFilters (R/W)
            SPDRP_LOWERFILTERS                  = 0x00000012, // LowerFilters (R/W)
            SPDRP_BUSTYPEGUID                   = 0x00000013, // BusTypeGUID (R)
            SPDRP_LEGACYBUSTYPE                 = 0x00000014, // LegacyBusType (R)
            SPDRP_BUSNUMBER                     = 0x00000015, // BusNumber (R)
            SPDRP_ENUMERATOR_NAME               = 0x00000016, // Enumerator Name (R)
            SPDRP_SECURITY                      = 0x00000017, // Security (R/W, binary form)
            SPDRP_SECURITY_SDS                  = 0x00000018, // Security (W, SDS form)
            SPDRP_DEVTYPE                       = 0x00000019, // Device Type (R/W)
            SPDRP_EXCLUSIVE                     = 0x0000001A, // Device is exclusive-access (R/W)
            SPDRP_CHARACTERISTICS               = 0x0000001B, // Device Characteristics (R/W)
            SPDRP_ADDRESS                       = 0x0000001C, // Device Address (R)
            SPDRP_UI_NUMBER_DESC_FORMAT         = 0X0000001D, // UiNumberDescFormat (R/W)
            SPDRP_DEVICE_POWER_DATA             = 0x0000001E, // Device Power Data (R)
            SPDRP_REMOVAL_POLICY                = 0x0000001F, // Removal Policy (R)
            SPDRP_REMOVAL_POLICY_HW_DEFAULT     = 0x00000020, // Hardware Removal Policy (R)
            SPDRP_REMOVAL_POLICY_OVERRIDE       = 0x00000021, // Removal Policy Override (RW)
            SPDRP_INSTALL_STATE                 = 0x00000022, // Device Install State (R)
            
            /// <summary>(Windows Server 2003 and later) The function retrieves a REG_MULTI_SZ string that represents the location of the device in the device tree.</summary>
            SPDRP_LOCATION_PATHS                = 0x00000023, // Device Location Paths (R)
            SPDRP_BASE_CONTAINERID              = 0x00000024  // Base ContainerID (R)
        }




        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public  int     cbSize;
            public  Guid    ClassGuid;
            public  int     DevInst;
            public  UIntPtr Reserved;
        }

        [DllImport("setupapi.dll")]     public static extern bool   SetupDiEnumDeviceInfo            (IntPtr DeviceInfoSet, int MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);
        [DllImport("setupapi.dll")]     public static extern bool   SetupDiGetDeviceRegistryProperty (IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, SPDRP Property, out int PropertyRegDataType, IntPtr PropertyBuffer, int PropertyBufferSize, out int RequiredSize);





        public static IntPtr INVALID_HANDLE_VALUE   = new IntPtr(-1);

        public const int  DIGCF_PRESENT             = 0x00000002;
        public const int  DIGCF_DEVICEINTERFACE     = 0x00000010;

        public const uint GENERIC_READ              = 0x80000000;
        public const uint GENERIC_WRITE             = 0x40000000;
        public const uint OPEN_EXISTING             = 0x00000003;

        public const uint FILE_SHARE_READ           = 0x00000001;
        public const uint FILE_SHARE_WRITE          = 0x00000002;

        public const uint FILE_FLAG_NO_BUFFERING    = 0x20000000;
        public const uint FILE_FLAG_OVERLAPPED      = 0x40000000;

        public const uint INFINITE                  = 0xFFFFFFFF;
        public const uint WAIT_ABANDONED            = 0x00000080;
        public const uint WAIT_OBJECT_0             = 0x00000000;
        public const uint WAIT_TIMEOUT              = 0x00000102;





        /// <summary>0x80942032</summary>
        public const uint IOCTL_USBIO_CLASS_OR_VENDOR_IN_REQUEST   = 0x80942032; // _USBIO_IOCTL_CODE(12,METHOD_OUT_DIRECT);
        
        /// <summary>0x80942035</summary>
        public const uint IOCTL_USBIO_CLASS_OR_VENDOR_OUT_REQUEST  = 0x80942035; // _USBIO_IOCTL_CODE(13,METHOD_IN_DIRECT);

        /// <summary>0x80942024</summary>
        public const uint IOCTL_USBIO_SET_CONFIGURATION            = 0x80942024; // _USBIO_IOCTL_CODE( 9,METHOD_BUFFERED)

        /// <summary>0x80942050</summary>
        public const uint IOCTL_USBIO_GET_CONFIGURATION_INFO       = 0x80942050; // _USBIO_IOCTL_CODE(20,METHOD_BUFFERED)

        /// <summary>0x80942078</summary>
        public const uint IOCTL_USBIO_BIND_PIPE                    = 0x80942078; // _USBIO_IOCTL_CODE(30,METHOD_BUFFERED)

        /// <summary>0x8094207c</summary>
        public const uint IOCTL_USBIO_UNBIND_PIPE                  = 0x8094207c; // _USBIO_IOCTL_CODE(31,METHOD_BUFFERED)


        [DllImport("user32.dll")]                               public static extern int    GetCaretBlinkTime();
        [DllImport("user32.dll")]                               public static extern bool   SetCaretBlinkTime(uint uMSeconds);


        [DllImport("user32.dll")]                               public static extern bool   SystemParametersInfo(int uiAction, int uiParam, ref int pvParam, int fWinIni);

        /// <summary>Delay in ms until repeat the key</summary>
        public static int GetKeyboardDelay()
        {
            int value = 0;

            if ( SystemParametersInfo(0x0016, 0, ref value, 0 ) == true )
            {
                // SPI_GETKEYBOARDDELAY (0..3)
                // 0 = 250ms, 1= 500ms, 2 = 750ms, 3 = 1000ms
                // 1 (500ms) ist Standard
                return value * 250;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>Repeats per seconds</summary>
        public static int GetKeyboardSpeed()
        {
            int value = 0;

            if ( SystemParametersInfo(0x000A, 0, ref value, 0 ) == true )
            {
                // SPI_GETKEYBOARDSPEED (0..31)
                // 0 = 2.5 repetitions per second...31 = 30 repetitions per second
                // TODO noch hochrechnen
                return value + 1;
            }
            else
            {
                return 0;
            }
        }

        public static readonly IntPtr HWND_BOTTOM       = new IntPtr( 1);
        public static readonly IntPtr HWND_NOTOPMOST    = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP          = new IntPtr( 0);
        public static readonly IntPtr HWND_TOPMOST      = new IntPtr(-1);

        public const int GWL_EXSTYLE      = -20;

        public const int WS_EX_TOOLWINDOW = 0x00000080;

        public const int SWP_NOACTIVATE   = 0x0010;

        [DllImport("user32.dll")]                               public static extern IntPtr GetDesktopWindow    ();
        [DllImport("user32.dll")]                               public static extern bool   SetForegroundWindow (IntPtr hWnd);
        [DllImport("user32.dll")]                               public static extern bool   BringWindowToTop    (IntPtr hWnd);
        [DllImport("user32.dll")]                               public static extern bool   SetWindowPos        (IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int W, int H, uint uFlags);
        [DllImport("user32.dll")]                               public static extern bool   ShowWindow          (IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]                               public static extern bool   ShowWindowAsync     (IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]                               public static extern int    GetWindowLong       (IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]                               public static extern IntPtr SetWindowLong       (IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]                               public static extern IntPtr GetDC               (IntPtr ptr);
        [DllImport("user32.dll")]                               public static extern int    GetSystemMetrics    (int abc);
        [DllImport("user32.dll")]                               public static extern IntPtr ReleaseDC           (IntPtr hWnd, IntPtr hDc);


        //[DllImport("user32.dll")]                               public static extern bool   LockWindowUpdate    (IntPtr hWndLock);

        public const int WM_SETREDRAW = 11;
        [DllImport("user32.dll")]                               public static extern int    SendMessage         (IntPtr hWnd, int Msg, bool wParam, int lParam);



        [DllImport("kernel32.dll")]                             public static extern int    GetProcessId    (IntPtr Process);


        [DllImport("kernel32.dll", CharSet=CharSet.Unicode)]    public static extern IntPtr CreateFileW     (string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile); 
        [DllImport("kernel32.dll")]                             public static extern bool   CloseHandle     (IntPtr hObject); 

        [DllImport("kernel32.dll")]                             public static extern bool   SystemTimeToTzSpecificLocalTime     (IntPtr lpTimeZone, ref SYSTEMTIME lpUniversalTime, ref SYSTEMTIME lpLocalTime); 

        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, byte[]                          lpInBuffer, int nInBufferSize,     byte[]                     lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped); 
        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, byte                            lpInBuffer, int nInBufferSize,     byte                       lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped); 

        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, ref USBIO_CLASS_OR_VENDOR_REQUEST lpInBuffer, int nInBufferSize,     byte[]                       lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped); 
        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, ref USBIO_SET_CONFIGURATION       lpInBuffer, int nInBufferSize,     IntPtr                       lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped);         
        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, IntPtr                            lpInBuffer, int nInBufferSize,     ref USBIO_CONFIGURATION_INFO lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped); 
        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, ref USBIO_BIND_PIPE               lpInBuffer, int nInBufferSize,     IntPtr                       lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped); 
        [DllImport("kernel32.dll")]                             public static extern bool   DeviceIoControl (IntPtr hDevice, uint dwIoControlCode, IntPtr                            lpInBuffer, int nInBufferSize,     IntPtr                       lpOutBuffer, int nOutBufferSize,     ref int lpBytesReturned, IntPtr lpOverlapped); 
        
        [DllImport("kernel32.dll")]                             public static extern bool   ReadFile        (IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr                                   lpOverlapped);
        
        
        [DllImport("Kernel32.dll")]                             public static extern void   RtlMoveMemory   (ref long dest, ref byte src, int size);
        
        [DllImport("kernel32.dll")]                             public static extern bool   WriteFile       (IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr                                   lpOverlapped);
        [DllImport("kernel32.dll")]                             public static extern bool   WriteFile       (IntPtr hFile, byte   lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr                                   lpOverlapped);
        [DllImport("kernel32.dll")]                             public static extern bool   WriteFile       (IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr                                   lpOverlapped);
        [DllImport("kernel32.dll")]                             public static extern bool   WriteFile       (IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, ref System.Threading.NativeOverlapped    lpOverlapped);



        // Overlapped
        [DllImport("kernel32.dll", SetLastError=true)]          public static extern bool   ReadFile            (IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr                                   lpOverlapped);
        [DllImport("kernel32.dll", SetLastError=true)]          public static extern bool   ReadFile            (IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, ref System.Threading.NativeOverlapped    lpOverlapped);
        [DllImport("kernel32.dll", SetLastError=true)]          public static extern bool   ReadFile            (IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, ref System.Threading.NativeOverlapped    lpOverlapped);






        [DllImport("kernel32.dll")]                             public static extern IntPtr CreateEvent         (IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("kernel32.dll", SetLastError=true)]          public static extern uint   WaitForSingleObject (IntPtr hHandle, uint dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError=true)]          public static extern bool   GetOverlappedResult (IntPtr hFile, IntPtr                                 lpOverlapped, out int lpNumberOfBytesTransferred, bool bWait);
        [DllImport("kernel32.dll", SetLastError=true)]          public static extern bool   GetOverlappedResult (IntPtr hFile, ref System.Threading.NativeOverlapped  lpOverlapped, out int lpNumberOfBytesTransferred, bool bWait);
        [DllImport("kernel32.dll")]                             public static extern bool   CancelIo            (IntPtr hFile);





        [DllImport("hid.dll")]                                  public static extern void   HidD_GetHidGuid                (out Guid HidGuid);
        [DllImport("hid.dll")]                                  public static extern bool   HidD_FlushQueue                (IntPtr HidDeviceObject);
        
        
        
        [DllImport("setupapi.dll")]                             public static extern IntPtr SetupDiGetClassDevs            (IntPtr   ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);
        [DllImport("setupapi.dll")]                             public static extern IntPtr SetupDiGetClassDevs            (ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);
        [DllImport("setupapi.dll")]                             public static extern bool   SetupDiDestroyDeviceInfoList   (IntPtr DeviceInfoSet);
        
        [DllImport("setupapi.dll")]                             public static extern bool   SetupDiEnumDeviceInterfaces    (IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref PSP_DEVICE_INTERFACE_DATA DeviceInterfaceData);
        
        [DllImport("setupapi.dll")]                             public static extern bool   SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref PSP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr                                 DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, out int RequiredSize, IntPtr DeviceInfoData);
        [DllImport("setupapi.dll")]                             public static extern bool   SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref PSP_DEVICE_INTERFACE_DATA DeviceInterfaceData, ref PSP_DEVICE_INTERFACE_DETAIL_DATA   DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, out int RequiredSize, IntPtr DeviceInfoData);






        // !!! Use UIntPtr else there is a overflow problem with x86 machines !!!
        // http://pinvoke.net/default.aspx/advapi32.RegOpenKeyEx


        public static UIntPtr HKEY_CURRENT_USER         = (UIntPtr)(0x80000001u);
        public static UIntPtr HKEY_LOCAL_MACHINE        = (UIntPtr)(0x80000002u);

        public const int      KEY_ENUMERATE_SUB_KEYS    = 0x0008;
        public const int      KEY_READ                  = 0x20019;
        public const int      RRF_RT_ANY                = 0x0000ffff;
        
        public const int      ERROR_SUCCESS             = 0;
        

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]     public static extern int    RegOpenKeyEx  (UIntPtr hKey, string lpSubKey, int ulOptions, int samDesired, out UIntPtr phkResult);
        [DllImport("advapi32.dll")]                             public static extern int    RegCloseKey   (UIntPtr hKey);
        [DllImport("Advapi32.dll", CharSet = CharSet.Auto)]     public static extern int    RegGetValueW  (UIntPtr hKey, string lpSubKey, string lpValue, int dwFlags, out int pdwType, IntPtr pvData, ref uint pcbData);
        
        [DllImport("advapi32.dll")]                             public static extern int    RegEnumKeyExW (UIntPtr hKey, int dwIndex, StringBuilder lpName, ref int lpcbName, IntPtr lpReserved, IntPtr lpClass, IntPtr lpcbClass, out long lpftLastWriteTime);
        [DllImport("advapi32.dll")]                             public static extern int    RegEnumKeyExW (UIntPtr hKey, int dwIndex, IntPtr        lpName, ref int lpcbName, IntPtr lpReserved, IntPtr lpClass, IntPtr lpcbClass, out long lpftLastWriteTime);
        [DllImport("advapi32.dll")]                             public static extern int    RegEnumKeyExW (UIntPtr hKey, int dwIndex, IntPtr        lpName, ref int lpcbName, IntPtr lpReserved, IntPtr lpClass, IntPtr lpcbClass, IntPtr   lpftLastWriteTime);


        [DllImport("advapi32.dll")]                             public static extern int    RegEnumValueW (UIntPtr hKey, uint dwIndex, StringBuilder lpValueName, ref uint lpcValueName, IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);
        [DllImport("advapi32.dll")]                             public static extern int    RegEnumValueW (UIntPtr hKey, uint dwIndex, IntPtr        lpValueName, ref uint lpcValueName, IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);


        [DllImport("advapi32.dll")]                             public static extern int    RegGetValue   (UIntPtr hKey, string lpSubKey, string lpValue, int dwFlags, out int pdwType, IntPtr pvData, ref int pcbData);






        [DllImport("Irprops.cpl")]  public static extern IntPtr     BluetoothFindFirstRadio             (ref BLUETOOTH_FIND_RADIO_PARAMS pbtfrp, out IntPtr phRadio);
        [DllImport("Irprops.cpl")]  public static extern bool       BluetoothFindNextRadio              (IntPtr hFind, ref IntPtr phRadio);
        [DllImport("Irprops.cpl")]  public static extern bool       BluetoothFindRadioClose             (IntPtr hFind);  
        [DllImport("irprops.cpl")]  public static extern int        BluetoothGetRadioInfo               (IntPtr hRadio, ref BLUETOOTH_RADIO_INFO pRadioInfo);

        [DllImport("Irprops.cpl")]  public static extern bool       BluetoothIsDiscoverable             (IntPtr hRadio);  
        [DllImport("Irprops.cpl")]  public static extern bool       BluetoothIsConnectable              (IntPtr hRadio);  
        
        [DllImport("Irprops.cpl")]  public static extern bool       BluetoothEnableDiscovery            (IntPtr hRadio, bool fEnabled);          
        [DllImport("Irprops.cpl")]  public static extern bool       BluetoothEnableIncomingConnections  (IntPtr hRadio, bool fEnabled);          






        /// <summary>The BluetoothFindFirstDevice function begins the enumeration Bluetooth devices.</summary>
        [DllImport("irprops.cpl")]  public static extern IntPtr     BluetoothFindFirstDevice             (ref BLUETOOTH_DEVICE_SEARCH_PARAMS pbtsp, ref BLUETOOTH_DEVICE_INFO pbtdi);
        
        /// <summary>The BluetoothFindNextDevice function finds the next remote Bluetooth device.</summary>
        [DllImport("irprops.cpl")]  public static extern bool       BluetoothFindNextDevice             (IntPtr hFind, ref BLUETOOTH_DEVICE_INFO pbtdi);

        /// <summary>The BluetoothFindDeviceClose function closes an enumeration handle associated with a device query.</summary>
        [DllImport("irprops.cpl")]  public static extern bool       BluetoothFindDeviceClose            (IntPtr hFind);

        [DllImport("irprops.cpl")]  public static extern int        BluetoothGetDeviceInfo              (IntPtr hRadio, ref BLUETOOTH_DEVICE_INFO pbtdi);




        /// <summary>The BluetoothDisplayDeviceProperties function starts Control Panel device information property sheet.</summary>
        [DllImport("irprops.cpl")]  public static extern bool        BluetoothDisplayDeviceProperties   (IntPtr hwndParent, ref BLUETOOTH_DEVICE_INFO pbtdi);

        /// <summary>The BluetoothRemoveDevice function removes authentication between a Bluetooth device and the computer and clears cached service information for the device.</summary>
        [DllImport("irprops.cpl")]  public static extern int        BluetoothRemoveDevice               (ref long pAddress);
    




        [DllImport("bthprops.cpl")]  public static extern int  BluetoothRegisterForAuthenticationEx (ref BLUETOOTH_DEVICE_INFO pbtdiln, out IntPtr phRegHandleOut, PFN_AUTHENTICATION_CALLBACK_EX pfnCallbackIn, IntPtr pvParam);
        [DllImport("bthprops.cpl")]  public static extern bool BluetoothUnregisterAuthentication     (IntPtr hRegHandle);

        [DllImport("bthprops.cpl")]  public static extern int  BluetoothAuthenticateDeviceEx      (IntPtr hwndParentIn, IntPtr hRadioIn, ref BLUETOOTH_DEVICE_INFO pbtdiInout, IntPtr pbtOobData, BLUETOOTH_AUTHENTICATION_REQUIREMENTS authenticationRequirement);
        [DllImport("bthprops.cpl")]  public static extern int  BluetoothSendAuthenticationResponseEx(IntPtr hRadioIn, ref BLUETOOTH_AUTHENTICATE_RESPONSE_PIN_INFO          pauthResponse);
        [DllImport("bthprops.cpl")]  public static extern int  BluetoothSendAuthenticationResponseEx(IntPtr hRadioIn, ref BLUETOOTH_AUTHENTICATE_NUMERIC_COMPARISON_INFO    pauthResponse);






























    

        /// <summary>Wandelt einen SYSTEMTYPE in ein DateTime um. Sind die Werte nicht gesetzt so wird DateTime(0) zurückgegeben</summary>
        public static DateTime ConvertSystemTimeToDateTime (SYSTEMTIME utc)
        {
            if (utc.wYear == 0)  return new DateTime();

            SYSTEMTIME st = new SYSTEMTIME();

            bool ret = SystemTimeToTzSpecificLocalTime(IntPtr.Zero, ref utc, ref st);


            return new DateTime(st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);
        }




        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_CLASS_OR_VENDOR_REQUEST
        {
            public int      Flags;
            public int      Type;
            public int      Recipient;
        
            public byte     RequestType;
            public byte     Request;
            public short    Value;
            public short    Index;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_INTERFACE_SETTING
        {
            public short        InterfaceIndex;
            public short        AlternateSettingIndex;
            public int          MaximumTransferSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_SET_CONFIGURATION
        {
            public short                        ConfigurationIndex;
            public short                        NbOfInterfaces;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
            public USBIO_INTERFACE_SETTING[]    InterfaceList;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_INTERFACE_CONFIGURATION_INFO
        {
            public byte InterfaceNumber;
            public byte AlternateSetting;
            public byte Class;
            public byte SubClass;
            public byte Protocol;
            public byte NumberOfPipes;
            public byte reserved1;  // reserved field, set to zero
            public byte reserved2;  // reserved field, set to zero
        }

        public enum USBIO_PIPE_TYPE
        {
            PipeTypeControl             = 0,
            PipeTypeIsochronous         = 1,
            PipeTypeBulk                = 2,
            PipeTypeInterrupt           = 3,
            PipeType_Force32bitEnum     = 2000000000
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_PIPE_CONFIGURATION_INFO
        {
            USBIO_PIPE_TYPE     PipeType;               // type
            public int          MaximumTransferSize;    // maximum Read/Write buffer size 
            public short        MaximumPacketSize;      // FIFO size of the endpoint  
            public byte         EndpointAddress;        // including direction bit (bit 7)
            public byte         Interval;               // in ms, for interrupt pipe
            public byte         InterfaceNumber;        // interface that the EP belongs to
            public byte         reserved1;              // reserved field, set to zero
            public byte         reserved2;              // reserved field, set to zero
            public byte         reserved3;              // reserved field, set to zero
        }




        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_CONFIGURATION_INFO
        {
            public int                                  NbOfInterfaces;
            public int                                  NbOfPipes;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
            public USBIO_INTERFACE_CONFIGURATION_INFO[] InterfaceInfo;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]            
            public USBIO_PIPE_CONFIGURATION_INFO[]      PipeInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USBIO_BIND_PIPE
        {
	        public byte     EndpointAddress;
        }


        //[StructLayout(LayoutKind.Sequential)]
        //public struct ISO_SETUP_DATA
        //{
        //    public int                                  ulIsoReadPacketSize;
        //    public int                                  ulIsoWritePacketSize;
        //}













        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_DEVICE_SEARCH_PARAMS
        {
            /// <summary>The size, in bytes, of the structure.</summary>
            public int      dwSize;
            
            /// <summary>A value that specifies that the search should return authenticated Bluetooth devices.</summary>
            public bool     fReturnAuthenticated;

            /// <summary>A value that specifies that the search should return remembered Bluetooth devices.</summary>
            public bool     fReturnRemembered;

            /// <summary>A value that specifies that the search should return unknown Bluetooth devices.</summary>
            public bool     fReturnUnknown;

            /// <summary>A value that specifies that the search should return connected Bluetooth devices.</summary>
            public bool     fReturnConnected;

            /// <summary>A value that specifies that a new inquiry should be issued.</summary>
            public bool     fIssueInquiry;

            /// <summary>A value that indicates the time out for the inquiry, expressed in increments of 1.28 seconds. For example, an inquiry of 12.8 seconds has a cTimeoutMultiplier value of 10. The maximum value for this member is 48. When a value greater than 48 is used, the calling function immediately fails and returns E_INVALIDARG.</summary>
            public byte     cTimeoutMultiplier;

            /// <summary>A handle for the radio on which to perform the inquiry. Set to NULL to perform the inquiry on all local Bluetooth radios.</summary>
            public IntPtr   hRadio;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME 
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct BLUETOOTH_DEVICE_INFO
        {
            /// <summary>Size of the BLUETOOTH_DEVICE_INFO structure, in bytes.</summary>
            public int              dwSize;
            
            /// <summary>Address of the device.</summary>            
            public long             Address;
            
            /// <summary>Class of the device.</summary>
            public int              ulClassofDevice;
            
            /// <summary>Specifies whether the device is connected.</summary>
            public bool             fConnected;
            
            /// <summary>Specifies whether the device is a remembered device. Not all remembered devices are authenticated.</summary>            
            public bool             fRemembered;
            
            /// <summary>Specifies whether the device is authenticated, paired, or bonded. All authenticated devices are remembered.</summary>            
            public bool             fAuthenticated;
            
            /// <summary>Last time the device was seen, in the form of a SYSTEMTIME structure.</summary>            
            public SYSTEMTIME       stLastSeen;
            
            /// <summary>Last time the device was used, in the form of a SYSTEMTIME structure.</summary>            
            public SYSTEMTIME       stLastUsed;

            /// <summary>Name of the device.</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 248)]   public string szName;
        }




        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_FIND_RADIO_PARAMS
        {
	        public int dwSize;
        }


        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
        public struct BLUETOOTH_RADIO_INFO
        {
            /// <summary>Size, in bytes, of the structure.</summary>
            public int      dwSize;
            
            /// <summary>Address of the local Bluetooth radio.</summary>            
            public long     address;
            
            /// <summary>Name of the local Bluetooth radio.</summary>            
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 248 )] public string   szName;
            
            /// <summary>Device class for the local Bluetooth radio.</summary>
            public int      ulClassOfDevice;
            
            /// <summary>This member contains data specific to individual Bluetooth device manufacturers.</summary>            
            public short    lmpSubversion;
            
            /// <summary>Manufacturer of the Bluetooth radio, expressed as a BTH_MFG_Xxx value. For more information about the Bluetooth assigned numbers document and a current list of values, see the Bluetooth specification at www.bluetooth.com.</summary>            
            public short    manufacturer;
        }





        [StructLayout(LayoutKind.Sequential)]
        public struct PSP_DEVICE_INTERFACE_DATA
        {
            public  int     cbSize;
            public  Guid    interfaceClassGuid;
            public  int     flags;
            public  UIntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PSP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public  uint     cbSize;
            
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string DevicePath;        
        }







        public enum BLUETOOTH_AUTHENTICATION_METHOD
        {
            BLUETOOTH_AUTHENTICATION_METHOD_LEGACY                  = 0x01,
            BLUETOOTH_AUTHENTICATION_METHOD_OOB                     = 0x02,
            BLUETOOTH_AUTHENTICATION_METHOD_NUMERIC_COMPARISON      = 0x03,
            BLUETOOTH_AUTHENTICATION_METHOD_PASSKEY_NOTIFICATION    = 0x04,
            BLUETOOTH_AUTHENTICATION_METHOD_PASSKEY                 = 0x05
        }
 
        public enum BLUETOOTH_IO_CAPABILITY
        {
            BLUETOOTH_IO_CAPABILITY_DISPLAYONLY      = 0x00,
            BLUETOOTH_IO_CAPABILITY_DISPLAYYESNO     = 0x01,
            BLUETOOTH_IO_CAPABILITY_KEYBOARDONLY     = 0x02,
            BLUETOOTH_IO_CAPABILITY_NOINPUTNOOUTPUT  = 0x03,
            BLUETOOTH_IO_CAPABILITY_UNDEFINED        = 0xFF
        }

        public enum BLUETOOTH_AUTHENTICATION_REQUIREMENTS 
        {
            MITMProtectionNotRequired                 = 0x00,
            MITMProtectionRequired                    = 0x01,
            MITMProtectionNotRequiredBonding          = 0x02,
            MITMProtectionRequiredBonding             = 0x03,
            MITMProtectionNotRequiredGeneralBonding   = 0x04,
            MITMProtectionRequiredGeneralBonding      = 0x05,
            MITMProtectionNotDefined                  = 0xff 
        } 









        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_AUTHENTICATION_CALLBACK_PARAMS
        {
            public BLUETOOTH_DEVICE_INFO                    deviceInfo;
            public BLUETOOTH_AUTHENTICATION_METHOD          authenticationMethod;
            public BLUETOOTH_IO_CAPABILITY                  ioCapability;
            public BLUETOOTH_AUTHENTICATION_REQUIREMENTS    authenticationRequirements;
            public uint                                     Numeric_Value;
            public uint                                     Passkey;
        }

        public delegate bool PFN_AUTHENTICATION_CALLBACK_EX(IntPtr pvParam, ref BLUETOOTH_AUTHENTICATION_CALLBACK_PARAMS bdi);


        [StructLayout(LayoutKind.Sequential, Size = 20)]
        public struct BLUETOOTH_PIN_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] // BTH_MAX_PIN_SIZE
            public byte[] pin;
            public byte   pinLength;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_AUTHENTICATE_RESPONSE_PIN_INFO
        {
            public long                             bthAddressRemote;
            public BLUETOOTH_AUTHENTICATION_METHOD  authMethod;
            public BLUETOOTH_PIN_INFO               pinInfo;
            public byte                             negativeResponse;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLUETOOTH_AUTHENTICATE_NUMERIC_COMPARISON_INFO
        {
            public long                             bthAddressRemote;
            public BLUETOOTH_AUTHENTICATION_METHOD  authMethod;
            public uint                             NumericValue;
            public byte                             negativeResponse;
        }



     //union {
     //   BLUETOOTH_PIN_INFO                pinInfo;
     //   BLUETOOTH_OOB_DATA                oobInfo;
     //   BLUETOOTH_NUMERIC_COMPARISON_INFO numericCompInfo;
     //   BLUETOOTH_PASSKEY_INFO            passkeyInfo;
     // };




        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLEXECUTEINFO
        {
                                                public int    cbSize;
                                                public uint   fMask;
                                                public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]   public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]   public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]   public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]   public string lpDirectory;
                                                public int    nShow;
                                                public IntPtr hInstApp;
                                                public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]   public string lpClass;
                                                public IntPtr hkeyClass;
                                                public uint   dwHotKey;
                                                public IntPtr hIcon;
                                                public IntPtr hProcess;
        }

        [DllImport("Shell32.dll")]                          public static extern bool   IsUserAnAdmin  ();
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]  public static extern bool   ShellExecuteEx (ref SHELLEXECUTEINFO pExecInfo);





    }
}


// http://code.google.com/p/smtcare/source/browse/trunk/RobotHealth_InTheHand/BTLib/Net.Bluetooth/?r=53
