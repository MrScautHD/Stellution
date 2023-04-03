using Easel.Scenes;
using Stellution.Common.csharp.args;

namespace Stellution.Common.csharp.scenes; 

public abstract class ModifiedScene : Scene {
    
    public static event EventHandler<SceneInitializeArgs>? Initializing;

    protected ModifiedScene(string name, int initialCapacity = 128) : base(name, initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();
        
        Initializing?.Invoke(null, new SceneInitializeArgs(this));
    }
}