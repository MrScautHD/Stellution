using System;
using Easel;
using Easel.Content;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public abstract class ContentRegistry : Registry {
    
    public static ContentManager Content => EaselGame.Instance.Content;

    protected static Lazy<T> Load<T>(string definitionName, string path) {
        Lazy<T> lazy = new Lazy<T>(() => {
            T content = Content.Load<T>(definitionName, path);
            
            return content;
        });

        return lazy;
    }
}