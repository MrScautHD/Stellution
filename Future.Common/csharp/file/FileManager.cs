using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace Future.Common.csharp.file; 

public class FileManager {
    
    public string FileDirectory { get; private set; }
    public string FileName { get; private set; }

    public FileManager(string directory, string name) {
        this.FileDirectory = directory;
        this.FileName = name;
    }

    /**
     * Create File (Override it)
     */
    protected void CreateFile() {
        if (!Directory.Exists(this.FileDirectory)) {
            Directory.CreateDirectory(this.FileDirectory);
        }

        File.Create(this.GetPath()).Close();
    }

    /**
     * Write a line in the File
     */
    public void WriteLine(object? message) {
        using (StreamWriter writer = new StreamWriter(this.GetPath(), true)) {
            writer.WriteLine(message);
        }
    }

    /**
     * Read File and return a list with all strings
     */
    public string[] ReadAllLines<T>() {
        return File.ReadAllLines(this.GetPath());
    }
    
    /**
     * Write a object in the File
     */
    public void WriteJson<T>(T obj) {
        using (StreamWriter writer = new StreamWriter(this.GetPath(), true)) {
            writer.WriteLine(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj)));
        }
    }
    
    /**
     * Read JSON File and return a list with all objects
     */
    public JsonNode ReadJson() {
        return JsonArray.Parse(File.ReadAllText(this.GetPath()));
    }

    /**
     * Get File path
     */
    public string GetPath() {
        return Path.Combine(this.FileDirectory, this.FileName);
    }
}