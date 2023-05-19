using System;
using Easel;
using Easel.Content;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public abstract class ContentRegistry : Registry {
    
    public static ContentManager Content => EaselGame.Instance.Content;

    protected static Lazy<T> Load<T>(string definition, string path) {
        return new Lazy<T>(() => Content.Load<T>(definition, path));
    }
}