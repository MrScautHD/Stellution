using Easel.Content;

namespace Future.Common.csharp.registry; 

public class Registry {

    protected T Register<T, B>(string name, Dictionary<string, B> registryList, T type) where T : B {
        registryList.Add(name, type);
        
        return type;
    }
    
    protected T RegisterLoad<T>(string name, Dictionary<string, T> registryList, ContentManager content, string path) {
        T type = content.Load<T>(path);
        registryList.Add(name, type);

        return type;
    }
}