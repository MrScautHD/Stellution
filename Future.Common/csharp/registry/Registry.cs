namespace Future.Common.csharp.registry; 

public class Registry : IRegistry {

    protected T Register<T, B>(string name, Dictionary<string, B> registryList, T config) where T : B {
        registryList.Add(name, config);
        
        return config;
    }
}