namespace Future.Common.csharp.registry; 

public class Registry : IRegistry {

    protected T Register<T>(string name, Dictionary<string, T> registryList, T config) {
        registryList.Add(name, config);
        
        return config;
    }
}