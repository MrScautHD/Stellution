using Easel;
using Easel.Content;
using Easel.Content.Builder;

namespace Future.Common.csharp.registry; 

public abstract class Registry {

    protected static T Register<T, B>(string key, Dictionary<string, B> registryList, T type) where T : B {
        registryList.Add(key, type);
        
        return type;
    }
    
    protected static T Load<T>(ContentBuilder builder, IContentType contentType) {
        ContentDefinition definition = builder.Add(contentType).Build(DuplicateHandling.Ignore);
        ContentManager content = EaselGame.Instance.Content;

        content.AddContent(definition); // MAKE IT OVERRIDE ABLE
        
        T type = content.Load<T>(contentType.FriendlyName);
        
        return type;
    }
}