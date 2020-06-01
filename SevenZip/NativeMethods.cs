namespace SevenZip
{
    using System;
    using System.Runtime.InteropServices;

#if UNMANAGED
    internal static class NativeMethods
    {
        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int CreateObjectDelegate(
            [In] ref Guid classID,
            [In] ref Guid interfaceID,
            [MarshalAs(UnmanagedType.Interface)] out object outObject);

        #endregion

        public static IntPtr LoadLibrary(string fileName)
        {
#if NETFRAMEWORK
            if (Platform.OperatingSystem == OperatingSystemType.Windows)
            {
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#endif
                return LoadWindowsLibrary(fileName);
            }
            else
            {
                return LoadUnixLibrary(fileName, RTLD_NOW);
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr LoadWindowsLibrary([MarshalAs(UnmanagedType.LPStr)] string fileName);
        
        public const int RTLD_NOW = 0x002;
        
        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr LoadUnixLibrary(string path, int flags);

        public static bool FreeLibrary(IntPtr hModule)
        {
#if NETFRAMEWORK
            if (Platform.OperatingSystem == OperatingSystemType.Windows)
            {
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#endif
                return FreeLibraryWindows(hModule);
            }
            else
            {
                return (FreeLibraryUnix(hModule) == 0);
            }
        }
        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibraryWindows(IntPtr hModule);
        [DllImport("libdl", EntryPoint = "dlclose")]
        public static extern int FreeLibraryUnix(IntPtr hModule);

        public static IntPtr GetProcAddress(IntPtr hModule, string procName)
        {
#if NETFRAMEWORK
            if (Platform.OperatingSystem == OperatingSystemType.Windows)
            {
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#endif
                return GetProcAddressWindows(hModule, procName);
            }
            else
            {
                return GetProcAddressUnix(hModule, procName);
            }
        }
        
        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetProcAddressWindows(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string procName);
        [DllImport("libdl", EntryPoint = "dlsym")]
        public static extern IntPtr GetProcAddressUnix(IntPtr hModule, string procName);

        public static T SafeCast<T>(PropVariant var, T def)
        {
            object obj;
            try
            {
                obj = var.Object;
            }
            catch (Exception)
            {
                return def;
            }
            if (obj != null && obj is T)
            {
                return (T) obj;
            }            
            return def;
        }
    }
#endif
}