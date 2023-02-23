using Future.Common.csharp.scenes;

namespace Future.Common.csharp.events; 

public class SceneInitializeArgs {
    
    public readonly ModifiedScene Scene;

    public SceneInitializeArgs(ModifiedScene scene) => this.Scene = scene;
}