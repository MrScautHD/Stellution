using Newtonsoft.Json.Linq;

namespace Future.Common.csharp.file; 

public class Config : FileManager {

    private Dictionary<string, object> _dictionary = new();
    
    public Config() : base("config", "config.json") {
        
        // Create File if not already
        this.CreateFile(false);
        
        // HERE TO A NEW CLASS:
        
        // Set Dictionary
        this._dictionary.Add("CustomerName", "test");
        this._dictionary.Add("CustomerEmail", "IDIOTS.com");
        this._dictionary.Add("Check", true);
        this._dictionary.Add("TotalSales", 10);
        
        // Write Json
        if (this.IsJsonValid()) {
            foreach (var dictionary in this._dictionary) {
                if (!this.ReadJsonAsNode().AsObject().ContainsKey(dictionary.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();

                    jsonObject[dictionary.Key] = dictionary.Value.ToString();
                    File.WriteAllText(this.GetPath(), jsonObject.ToString());
                }
            }
        }
        else {
            this.ClearFile();
            this.WriteJson(this._dictionary);
        }
    }
}