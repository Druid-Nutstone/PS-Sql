using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

internal static class SniLoader
{
    [DllImport("kernel32", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    public static void LoadSni()
    {
        string dllName = "Microsoft.Data.SqlClient.SNI.dll";
        string path = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            dllName);

        if (File.Exists(path))
        {
            IntPtr result = LoadLibrary(path);
            if (result == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Exception($"Failed to load {dllName}. Error: {error}");
            }
        }
    }
}
