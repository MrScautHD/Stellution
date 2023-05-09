using Easel.Scenes;

namespace Stellution.Common.csharp.scenes; 

public abstract class ModifiedScene : Scene {

    public delegate void OnSceneInitializing(ModifiedScene scene, string name);
    public static event OnSceneInitializing? Initializing;

    protected ModifiedScene(string name, int initialCapacity = 128) : base(name, initialCapacity) {
        
    }

    protected override void Initialize() {
        base.Initialize();

        Initializing?.Invoke(this, this.Name);
    }
}