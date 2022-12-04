namespace Future.Common.csharp.registry; 

public class RegistryObject<T> {

    public string Key { get; private set; }
    
    public T Value { get; private set; }

    public static RegistryObject<T> Create(RegistryTypes type, string key, T value) {
        RegistryObject<T> registryObject = new RegistryObject<T>();
        registryObject.Value = value;
        
        Registry.AddRegistry(type, key, value);

        return registryObject;
    }
}