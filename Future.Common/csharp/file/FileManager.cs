using System.Text.Json;

namespace Future.Common.csharp.file; 

public class FileManager {
    
    public string FileDirectory { get; private set; }
    public string FileName { get; private set; }
    
    public FileManager(string directory, string fileName) {
        this.FileDirectory = directory;
        this.FileName = fileName;
    }

    /**
     * Create File (Override it)
     */
    public void CreateFile() {
        if (!Directory.Exists(this.FileDirectory)) {
            Directory.CreateDirectory(this.FileDirectory);
        }

        File.Create(this.GetPath()).Close();
    }

    /**
     * Write File (.txt)
     */
    public void WriteTxt(string message) {
        using (StreamWriter writer = new StreamWriter(this.GetPath(), true)) {
            writer.WriteLine(message);
        }
    }
    
    /**
     * Read File (.txt)
     */
    public List<string> ReadTxt() {
        using (StreamReader reader = new StreamReader(this.GetPath())) {
            List<string> list = new();
            list.Add(reader.ReadToEnd());

            return list;
        }
    }
    
    /**
     * Write Json File (.json)
     */
    public void WriteJson<T>(T jsonType) {
        string text = JsonSerializer.Serialize(jsonType);
        
        File.WriteAllText(this.GetPath(), text);
    }

    /**
     * Read Json File (.json)
     */
    public T ReadJson<T>(T jsonType) {
        string text = File.ReadAllText(this.GetPath());
        T variableType = JsonSerializer.Deserialize<T>(text);
        
        return variableType;
    }

    protected string GetPath() {
        return Path.Combine(this.FileDirectory, this.FileName);
    }
}