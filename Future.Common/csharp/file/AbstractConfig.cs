using Newtonsoft.Json.Linq;

namespace Future.Common.csharp.file; 

public abstract class AbstractConfig : FileManager {

    protected Dictionary<string, object> _dictionary = new();
    
    protected AbstractConfig(string directory, string name) : base(directory, name) {
        this.CreateFile(false);
    }

    protected void WriteConfig() {
        if (this.IsJsonValid()) {
            foreach (var dictionary in this._dictionary) {
                if (!this.ReadJsonAsNode().AsObject().ContainsKey(dictionary.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();

                    jsonObject[dictionary.Key] = dictionary.Value.ToString();
                    File.WriteAllText(this.GetPath(), jsonObject.ToString());
                    Logger.Log.Print("File " + this.FileName + " added: " + dictionary.Key + " = " + dictionary.Value, ConsoleColor.Green);
                }
            }

            foreach (var jsonObjectPair in this.ReadJsonAsNode().AsObject()) {
                if (!this._dictionary.ContainsKey(jsonObjectPair.Key)) {
                    JObject jsonObject = this.ReadJsonAsObject();
                    jsonObject.Remove(jsonObjectPair.Key);
                    
                    File.WriteAllText(this.GetPath(), jsonObject.ToString());
                    Logger.Log.Print("Value: " + jsonObjectPair.Key + " get removed! in " + "file " + this.FileName, ConsoleColor.Red);
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