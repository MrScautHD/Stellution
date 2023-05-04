using System;
using Easel;
using Easel.Content;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public abstract class ClientRegistry : Registry {
    
    public static ContentManager Content => EaselGame.Instance.Content;

    public static Lazy<T> Load<T>(string definitionName, string path) {
        Lazy<T> lazy = new Lazy<T>(() => {
            T texture = Content.Load<T>(definitionName, path);
            
            return texture;
        });

        return lazy;
    }
}