using System.Collections.Generic;
using Future.Client.csharp.translation;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientTranslationRegistry : Registry {
    
    public static readonly Dictionary<string, Translation> Translations = new();
    
    public static readonly Translation English = Register("english", Translations, new Translation("content", "english"));
}