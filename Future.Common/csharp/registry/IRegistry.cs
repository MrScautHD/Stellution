using Easel.Content;

namespace Future.Common.csharp.registry; 

public interface IRegistry {
    
    public static readonly List<IRegistry> RegistryTypes = new();

    virtual void Register(ContentManager content) {
    }
}