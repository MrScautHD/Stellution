using System;
using System.IO;
using Microsoft.Win32;

namespace Easel;

public static class SystemInfo
{
    public static string CpuInfo
    {
        get
        {
            if (OperatingSystem.IsWindows())
            {
                return Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0")
                           ?.GetValue("ProcessorNameString")?.ToString() ??
                       "Unknown (expected value was not found in the registry)";
            }

            if (OperatingSystem.IsLinux())
            {
                string[] lines = File.ReadAllLines("/proc/cpuinfo");
                string modelName = "Unknown (cpuinfo doesn't contain expected result..)";
                foreach (string line in lines)
                {
                    if (line.Trim().ToLower().StartsWith("model name"))
                    {
                        modelName = line.Split(':')[1].Trim();
                        break;
                    }
                }

                return modelName;
            }

            return "Unknown (unsupported OS).";
        }
    }

    public static string MemoryInfo
    {
        get
        {
            if (OperatingSystem.IsWindows())
                return "(not implemented for windows)";
            if (OperatingSystem.IsLinux())
            {
                string[] lines = File.ReadAllLines("/proc/meminfo");
                string total = "Unknown (meminfo doesn't contain expected result..)";
                string free = "Unknown (meminfo doesn't contain expected result..)";
                foreach (string line in lines)
                {
                    string tLine = line.Trim().ToLower();
                    if (tLine.StartsWith("memtotal"))
                        total = line.Split(':')[1].Trim();
                    else if (tLine.StartsWith("memavailable"))
                        free = line.Split(':')[1].Trim();
                }

                return "Total: " + total + ", Available: "+ free;
            }
            
            return "Unknown (unsupported OS).";
        }
    }

    public static int LogicalThreads => Environment.ProcessorCount;
}