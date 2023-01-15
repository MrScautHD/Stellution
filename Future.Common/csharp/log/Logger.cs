using Future.Common.csharp.file;

namespace Future.Common.csharp.log; 

public class Logger : FileManager {
    
    public Logger(string directory, string fileName) : base(directory, fileName) {
        this.CreateFile();
    }

    public void Print(string message, ConsoleColor color = ConsoleColor.White) {
        Console.ForegroundColor = color;
        this.WriteTxt(message);
        Console.WriteLine(message);
        Console.ResetColor();
    }
}