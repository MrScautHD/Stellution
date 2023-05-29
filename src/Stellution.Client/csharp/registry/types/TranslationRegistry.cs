using Stellution.Client.csharp.translation;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry.types; 

public class TranslationRegistry : Registry {
    
    public static readonly string DefinitionName = "content/lang";
    
    public static Translation? English { get; private set; }

    public override void Initialize() {
        English = new Translation(DefinitionName, "english");
    }
}