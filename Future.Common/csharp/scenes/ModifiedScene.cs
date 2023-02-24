using Easel.Scenes;
using Future.Common.csharp.events;

namespace Future.Common.csharp.scenes; 

public abstract class ModifiedScene : Scene {
    
    public static event EventHandler<SceneInitializeArgs>? Initializing;

    public ModifiedScene(string name = "", int initialCapacity = 128) : base(initialCapacity) {
        this.Name = name != String.Empty ? name : this.SceneName();
    }
    
    public abstract string SceneName();
    
    protected override void Initialize() {
        base.Initialize();
        
        Initializing?.Invoke(null, new SceneInitializeArgs(this));
    }
}