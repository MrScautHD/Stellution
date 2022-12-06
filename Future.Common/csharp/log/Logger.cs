namespace Future.Common.csharp.log; 

public class Logger {
    
    private readonly string _directory = "logs";
    private readonly string _fileName = "log.txt";

    public Logger() {
        if (!Directory.Exists(this._directory)) {
            Directory.CreateDirectory(this._directory);
        }
        
        File.Create(this.GetName()).Close();
    }
    
    public void Print(string message) {
        using (StreamWriter line = new StreamWriter(this.GetName(), true)) {
            line.WriteLine(message);
        }

        Console.WriteLine(message);
    }

    private string GetName() {
        return Path.Combine(this._directory, this._fileName);
    }
}