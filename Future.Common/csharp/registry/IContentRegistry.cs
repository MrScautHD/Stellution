using Easel.Content;

namespace Future.Common.csharp.registry; 

public interface IContentRegistry {
    
    public static readonly List<IContentRegistry> ContentTypes = new();
    
    virtual void Load(ContentManager content) {
    }
}