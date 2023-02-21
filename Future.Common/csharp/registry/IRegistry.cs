using Easel.Content;

namespace Future.Common.csharp.registry; 

public interface IRegistry {
    
    public static readonly List<IRegistry> RegistryTypes = new();

    virtual void Initialize(ContentManager content) {
        
    }

    virtual void FixedUpdate() {
        
    }

    virtual void Draw() {
        
    }
}