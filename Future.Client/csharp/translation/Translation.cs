using System.Collections.Generic;
using Future.Client.csharp.registry;
using Future.Common.csharp.file;

namespace Future.Client.csharp.translation; 

public class Translation : FileManager {

    /**
     * Set this to your LANG.
     */
    public static Translation Lang = TranslationRegistry.English;
    
    private Dictionary<string, string> _dictionary = new();

    public Translation(string definitionName, string name) : base(definitionName, name + ".json") {
        this.ReadFile();
    }
    
    public string Get(string key) {
        foreach (var dictionary in this._dictionary) {
            if (dictionary.Key.Equals(key)) {
                return dictionary.Value;
            }
        }

        return key;
    }

    private void ReadFile() {
        foreach (var jsonObjectPair in this.ReadJsonAsNode().AsObject()) {
            this._dictionary.Add(jsonObjectPair.Key, jsonObjectPair.Value.ToString());
        }
    }
}