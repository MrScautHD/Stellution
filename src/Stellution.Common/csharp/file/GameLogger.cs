using Easel.Core;

namespace Stellution.Common.csharp.file; 

public class GameLogger {
    
    public static void Initialize(string directory, string name) {
        string path = Path.Combine(directory, name + "-" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".txt");
        
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        File.Create(path).Close();
        Logger.InitializeLogFile(path);
        Logger.UseConsoleLogs();
    }
}