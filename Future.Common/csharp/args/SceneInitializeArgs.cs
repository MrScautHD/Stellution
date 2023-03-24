using Future.Common.csharp.scenes;

namespace Future.Common.csharp.args; 

public class SceneInitializeArgs {
    
    public readonly ModifiedScene Scene;

    public SceneInitializeArgs(ModifiedScene scene) => this.Scene = scene;
}