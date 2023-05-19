using Easel.Core;
using Newtonsoft.Json.Linq;

namespace Stellution.Common.csharp.file; 

public abstract class AbstractConfig : FileManager {

    protected Dictionary<string, object> _dictionary = new();

    protected AbstractConfig(string directory, string name, string encryptKey = "") : base(directory, name + ".json", encryptKey) {
        this.CreateFile(false);
    }

    protected void WriteConfig() {
        if (this.IsJsonValid()) {
            foreach (var jsonObjectPair in this.ReadJsonAsNode().AsObject()) {
                if (!this._dictionary.ContainsKey(jsonObjectPair.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();
                    jsonObject.Remove(jsonObjectPair.Key);
                    
                    File.WriteAllText(this.GetPath(), this.EncryptString(jsonObject.ToString()));
                    Logger.Warn("Value: " + jsonObjectPair.Key + " get removed! In file " + this.FileName);
                }
            }
            
            foreach (var dictionary in this._dictionary) {
                if (!this.ReadJsonAsNode().AsObject().ContainsKey(dictionary.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();

                    jsonObject[dictionary.Key] = JToken.FromObject(dictionary.Value);
                    File.WriteAllText(this.GetPath(), this.EncryptString(jsonObject.ToString()));
                    Logger.Info("File " + this.FileName + " added: " + dictionary.Key + " = " + dictionary.Value);
                }
            }
        }
        else {
            this.ClearFile();
            this.WriteJson(this._dictionary);
            Logger.Warn("Re/Wrote " + this.FileName);
        }
    }
}