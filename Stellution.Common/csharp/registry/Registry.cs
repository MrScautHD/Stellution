namespace Stellution.Common.csharp.registry; 

public abstract class Registry {
    
    public static readonly List<Registry> RegistryTypes = new();
    
    public abstract void Initialize();
    
    /**
     * Use it to register "Types" that not get managed by the "ContentManager" or if they need to be listed!
     */
    protected T Register<T, B>(string name, Dictionary<string, B> registryList, T registerObject) where T : B {
        registryList.Add(name, registerObject);

        return registerObject;
    }
}