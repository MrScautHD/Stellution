using Easel;

namespace Stellution.Common.csharp.registry; 

public class Registry {
    
    //protected static ContentManager Content => EaselGame.Instance.Content;
    
    public static readonly List<IRegistry> RegistryTypes = new();
}