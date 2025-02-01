using D2RLAN.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;




class Injector
{
    private string gamePath;

    public Injector(string gamePath)
    {
        this.gamePath = gamePath;
    }

    [STAThread]
    public static void Inject(string[] args, string gamePath, string dllName)
    {
        if (args.Length != 1)
        {
            //MessageBox.Show("Provide the process name as an argument.");
            Environment.Exit(-1);
        }

        string processName = args[0];
        //MessageBox.Show($"[+] Injecting {dllName} into \"{processName}\"");

        var pids = GetPIDs(processName);
        foreach (var pid in pids)
        {
            //MessageBox.Show(exePath);
            string originalDllPath = System.IO.Path.Combine(gamePath, dllName);
            //MessageBox.Show($"[+] Injecting into {pid}");
            InjectDLL(pid, originalDllPath);
        }

        //Environment.Exit(0);
    }

    static List<int> GetPIDs(string processName)
    {
        var pids = new List<int>();

        foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName)))
        {
            pids.Add(process.Id);
            process.Dispose();
        }

        return pids;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool FreeLibrary(IntPtr hModule);

    static void InjectDLL(int pid, string path)
    {
        if (!File.Exists(path))
        {
            //MessageBox.Show("[!]Couldn't find DLL.");
            //Environment.Exit(-1);
        }

        IntPtr hProcess = OpenProcess(ProcessAccessFlags.All, false, pid);
        if (hProcess == IntPtr.Zero)
        {
            //MessageBox.Show("[!]Fail to open target process.");
            //Environment.Exit(-1);
        }

        IntPtr lpAddress = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)((path.Length + 1) * Marshal.SystemDefaultCharSize), AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
        if (lpAddress == IntPtr.Zero)
        {
            //MessageBox.Show("[!]Fail to allocate memory in target process.");
            //Environment.Exit(-1);
        }

        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(path);
        if (!WriteProcessMemory(hProcess, lpAddress, bytes, (uint)bytes.Length, out _))
        {
            //MessageBox.Show("[!]Fail to write in target process memory.");
            //Environment.Exit(-1);
        }

        IntPtr hKernel32 = GetModuleHandle("kernel32.dll");
        IntPtr lpStartAddress = GetProcAddress(hKernel32, "LoadLibraryW");
        if (lpStartAddress == IntPtr.Zero)
        {
            //MessageBox.Show("[!]Fail to get address of LoadLibraryW.");
            //Environment.Exit(-1);
        }

        IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, lpStartAddress, lpAddress, 0, IntPtr.Zero);
        if (hThread == IntPtr.Zero)
        {
            //MessageBox.Show("[!]Fail to create remote thread.");
            //Environment.Exit(-1);
        }

        WaitForSingleObject(hThread, 0xFFFFFFFF);
        CloseHandle(hThread);
        CloseHandle(hProcess);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool CloseHandle(IntPtr hObject);
}

[Flags]
public enum ProcessAccessFlags : uint
{
    All = 0x001F0FFF,
    Terminate = 0x00000001,
    CreateThread = 0x00000002,
    VirtualMemoryOperation = 0x00000008,
    VirtualMemoryRead = 0x00000010,
    VirtualMemoryWrite = 0x00000020,
    DuplicateHandle = 0x00000040,
    SetInformation = 0x00000200,
    QueryInformation = 0x00000400,
    Synchronize = 0x00100000
}

[Flags]
public enum AllocationType
{
    Commit = 0x1000,
    Reserve = 0x2000,
    Decommit = 0x4000,
    Release = 0x8000,
    Reset = 0x80000,
    TopDown = 0x100000,
    WriteWatch = 0x200000,
    Physical = 0x400000,
    LargePages = 0x20000000
}

[Flags]
public enum MemoryProtection
{
    NoAccess = 0x01,
    ReadOnly = 0x02,
    ReadWrite = 0x04,
    WriteCopy = 0x08,
    Execute = 0x10,
    ExecuteRead = 0x20,
    ExecuteReadWrite = 0x40,
    ExecuteWriteCopy = 0x80,
    GuardModifierflag = 0x100,
    NoCacheModifierflag = 0x200,
    WriteCombineModifierflag = 0x400
}
