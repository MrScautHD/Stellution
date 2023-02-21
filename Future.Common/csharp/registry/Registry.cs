using Easel.Content;

namespace Future.Common.csharp.registry; 

public class Registry {

    protected T Register<T, B>(string key, Dictionary<string, B> registryList, T type) where T : B {
        registryList.Add(key, type);
        
        return type;
    }
    
    protected T RegisterLoad<T>(string key, Dictionary<string, T> registryList, ContentManager content, string path) {
        T type = content.Load<T>(path);
        registryList.Add(key, type);

        return type;
    }
}