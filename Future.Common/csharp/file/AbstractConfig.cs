using Newtonsoft.Json.Linq;

namespace Future.Common.csharp.file; 

public abstract class AbstractConfig : FileManager {

    protected Dictionary<string, object> _dictionary = new();

    protected AbstractConfig(string directory, string name, string encryptKey = "") : base(directory, name, encryptKey) {
        this.CreateFile(false);
    }

    protected void WriteConfig() {
        if (this.IsJsonValid()) {
            foreach (var jsonObjectPair in this.ReadJsonAsNode().AsObject()) {
                if (!this._dictionary.ContainsKey(jsonObjectPair.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();
                    jsonObject.Remove(jsonObjectPair.Key);
                    
                    File.WriteAllText(this.GetPath(), this.EncryptString(jsonObject.ToString()));
                    Logger.Log.Print("Value: " + jsonObjectPair.Key + " get removed! In file " + this.FileName, ConsoleColor.Red);
                }
            }
            
            foreach (var dictionary in this._dictionary) {
                if (!this.ReadJsonAsNode().AsObject().ContainsKey(dictionary.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();

                    jsonObject[dictionary.Key] = JToken.FromObject(dictionary.Value);
                    File.WriteAllText(this.GetPath(), this.EncryptString(jsonObject.ToString()));
                    Logger.Log.Print("File " + this.FileName + " added: " + dictionary.Key + " = " + dictionary.Value, ConsoleColor.Green);
                }
            }
        }
        else {
            this.ClearFile();
            this.WriteJson(this._dictionary);
            Logger.Log.Print("Re/Wrote " + this.FileName, ConsoleColor.Yellow);
        }
    }
}