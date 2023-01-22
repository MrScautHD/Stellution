using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Future.Common.csharp.file; 

public class FileManager {
    
    public string FileDirectory { get; private set; }
    public string FileName { get; private set; }

    public FileManager(string directory, string name) {
        this.FileDirectory = directory;
        this.FileName = name;
    }

    /**
     * Create file (Override it)
     */
    protected void CreateFile(bool overrideExisting) {
        if (!Directory.Exists(this.FileDirectory)) {
            Directory.CreateDirectory(this.FileDirectory);
        }

        if (overrideExisting || !File.Exists(this.GetPath())) {
            File.Create(this.GetPath()).Close();
        }
    }

    /**
     * Write a line in the file
     */
    public void WriteLine(object? message) {
        using (StreamWriter writer = new StreamWriter(this.GetPath(), true)) {
            writer.WriteLine(message);
        }
    }

    /**
     * Read file and return a list with all strings
     */
    public string[] ReadAllLines<T>() {
        return File.ReadAllLines(this.GetPath());
    }
    
    /**
     * Checks is file empty
     */
    public bool IsFileEmpty() {
        return File.ReadAllText(this.GetPath()).Length == 0;
    }

    /**
     * Write a value in the file
     */
    public void WriteJson<T>(T obj) {
        using (StreamWriter writer = new StreamWriter(this.GetPath(), true)) {
            writer.WriteLine(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj)));
        }
    }
    
    /**
     * Read JSON file and return a list with all values as JsonNode
     */
    public JsonNode ReadJsonAsNode() {
        return JsonArray.Parse(File.ReadAllText(this.GetPath()));
    }
    
    /**
     * Read JSON file and return a list with all values as JObject
     */
    public JObject ReadJsonAsObject() {
        return JObject.Parse(File.ReadAllText(this.GetPath()));
    }

    /**
     * Checks is json file valid
     */
    public bool IsJsonValid() {
        try {
            this.ReadJsonAsNode();
            return true;
        } catch (Exception) {
            return false;
        }
    }

    /**
     * Clear file
     */
    public void ClearFile() {
        File.WriteAllText(this.GetPath(), string.Empty);
    }

    /**
     * Get file path
     */
    public string GetPath() {
        return Path.Combine(this.FileDirectory, this.FileName);
    }
}