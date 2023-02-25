using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Future.Common.csharp.file; 

public class FileManager {
    
    public string FileDirectory { get; }
    public string FileName { get; }
    
    private bool _encrypt;
    private string _encryptKey;

    public FileManager(string directory, string name, string encryptKey = "") {
        this.FileDirectory = directory;
        this.FileName = name;
        this._encrypt = encryptKey != string.Empty;
        this._encryptKey = encryptKey;
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
    public void WriteLine(object? obj) {
        using (StreamWriter writer = new StreamWriter(this.GetPath(), true)) {
            writer.WriteLine(this.EncryptString(obj.ToString()));
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
        this.WriteLine(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj)));
    }

    /**
     * Read JSON file and return a list with all values as JsonNode
     */
    public JsonNode ReadJsonAsNode() {
        return JsonArray.Parse(this.DecryptString(File.ReadAllText(this.GetPath())));
    }
    
    /**
     * Read JSON file and return a list with all values as JObject
     */
    public JObject ReadJsonAsObject() {
        return JObject.Parse(this.DecryptString(File.ReadAllText(this.GetPath())));
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
     * Encrypt string
     */
    public string EncryptString(string text) {
        if (!this._encrypt) {
            return text;
        }
        
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(this._encryptKey);
        aes.IV = new byte[16];

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream)) {
            streamWriter.Write(text);
        }
        
        return Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.InsertLineBreaks);
    }
    
    /**
     * Decrypt string
     */
    public string DecryptString(string text) {
        if (!this._encrypt) {
            return text;
        }
        
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(this._encryptKey);
        aes.IV = new byte[16];
                
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(Convert.FromBase64String(text));
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        
        return streamReader.ReadToEnd();
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