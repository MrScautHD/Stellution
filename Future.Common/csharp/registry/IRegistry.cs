namespace Future.Common.csharp.registry; 

public interface IRegistry {
    
    public static readonly List<IRegistry> RegistryTypes = new();

    public virtual void Initialize() {
        
    }
}