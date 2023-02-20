using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Easel.Core;

public static class Logger
{
    public static event OnLogAdded LogAdded;

    public static bool ShowCallerClass;

    public static bool ShowNamespace;

    public static bool ShowCallerMethod;

    static Logger()
    {
#if DEBUG
        ShowCallerClass = true;
        ShowCallerMethod = true;
        ShowNamespace = true;
#endif
    }

    public static void Log(LogType type, string message)
    {
        string caller = GetCaller(3);
        LogAdded?.Invoke(type, caller, message);
        if (type == LogType.Fatal)
            throw new EaselException(message);
    }
    
    public static void Debug(string message)
    {
        Log(LogType.Debug, message);
    }
    
    public static void Info(string message)
    {
        Log(LogType.Info, message);
    }

    public static void Warn(string message)
    {
        Log(LogType.Warn, message);
    }
    
    public static void Error(string message)
    {
        Log(LogType.Error, message);
    }

    public static void Fatal(string message)
    {
        Log(LogType.Fatal, message);
    }

    private static string GetCaller(int frames)
    {
        if (!(ShowCallerClass || ShowCallerMethod))
            return "";
        
        MethodBase info = new StackFrame(frames).GetMethod();
        string caller = "";
        if (ShowCallerClass)
            caller += ShowNamespace ? info.DeclaringType.FullName : info.DeclaringType.Name;
        if (ShowCallerMethod)
            caller += "::" + info.Name;
        return caller;
    }

    public delegate void OnLogAdded(LogType type, string caller, string message);

    private static string GetLogMessage(LogType type, string caller, string message)
    {
        if (caller.Length > 0)
            caller += " ";
        
        return "[" + caller + type.ToString().ToUpper() + "] " + message;
    }

    public enum LogType
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public static void UseConsoleLogs()
    {
        LogAdded += ConsoleLog;
    }

    private static void ConsoleLog(LogType type, string caller, string message)
    {
        string msg = GetLogMessage(type, caller, message);
        
        switch (type)
        {
            case LogType.Debug:
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(msg);
                break;
            case LogType.Info:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogType.Warn:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogType.Fatal:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(msg);
                Console.ForegroundColor = ConsoleColor.White;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    #region Log file

    public static string LogFilePath { get; private set; }

    private static StreamWriter _stream;
    
    public static void InitializeLogFile(string path)
    {
        LogFilePath = path;
        _stream = new StreamWriter(path, true);
        _stream.AutoFlush = true;
        
        LogAdded += LogFile;
    }

    private static void LogFile(LogType type, string caller, string message)
    {
        _stream.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + GetLogMessage(type, caller, message));
    }

    #endregion
}