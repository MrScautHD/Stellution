using Easel.Content;
using Stellution.Client.csharp.translation;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class TranslationRegistry : Registry, IRegistry {
    
    public static readonly string DefinitionName = "content/lang";
    
    public static Translation English { get; private set; }

    public void Initialize(ContentManager content) {
        English = new Translation(DefinitionName, "english");
    }
}