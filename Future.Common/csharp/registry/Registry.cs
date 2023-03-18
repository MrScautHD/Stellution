namespace Future.Common.csharp.registry; 

public abstract class Registry {

    /**
     * Register own Types
     */
    protected T Register<T, B>(string key, Dictionary<string, B> registryList, T type) where T : B {
        registryList.Add(key, type);
        
        return type;
    }
}