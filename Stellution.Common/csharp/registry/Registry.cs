namespace Stellution.Common.csharp.registry; 

public class Registry {
    
    public static readonly List<IRegistry> RegistryTypes = new();
    
    /**
     * Use it to register "TYPES" that not get managed by the ContentManager!
     */
    protected T Register<T, B>(string name, Dictionary<string, B> registryList, T registerObject) where T : B {
        registryList.Add(name, registerObject);

        return registerObject;
    }
}