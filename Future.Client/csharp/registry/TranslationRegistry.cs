using Easel.Content;
using Future.Client.csharp.translation;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class TranslationRegistry : Registry, IRegistry {
    
    public static readonly string DefinitionName = "content/lang";
    
    public static Translation English { get; private set; }

    public void Initialize(ContentManager content) {
        English = new Translation(DefinitionName, "english");
    }
}