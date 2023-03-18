using System.Collections.Generic;
using Easel.Content;
using Future.Client.csharp.translation;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientTranslationRegistry : Registry, IRegistry {
    
    public static readonly Dictionary<string, Translation> Translations = new();

    public static Translation English { get; private set; }

    public void Register(ContentManager content) {
        English = Register("english", Translations, new Translation("content", "english"));
    }
}