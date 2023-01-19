using Future.Common.csharp.file;

namespace Future.Common.csharp.log; 

public class Logger : FileManager {
    
    public Logger(string directory, string name) : base(directory, name) {
        this.CreateFile();
    }

    /**
     * Print in the console and in the .log file
     */
    public void Print(object? message, ConsoleColor color = ConsoleColor.White) {
        Console.ForegroundColor = color;
        this.WriteLine(message);
        Console.WriteLine(message);
        Console.ResetColor();
    }
}