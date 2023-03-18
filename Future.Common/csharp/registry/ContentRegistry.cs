using Easel;
using Easel.Content;
using Easel.Content.Builder;

namespace Future.Common.csharp.registry; 

public abstract class ContentRegistry {
    
    /**
     * Register ContentTypes
     */
    protected T Register<T>(ContentBuilder builder, T contentType) where T : IContentType {
        builder.Add(contentType);

        return contentType;
    }

    /**
     * Get Content with it
     */
    public static T Get<T>(IContentType contentType) {
        ContentManager content = EaselGame.Instance.Content;
        
        return content.Load<T>(contentType.FriendlyName);
    }
}