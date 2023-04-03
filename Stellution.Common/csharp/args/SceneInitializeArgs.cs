using Stellution.Common.csharp.scenes;

namespace Stellution.Common.csharp.args; 

public class SceneInitializeArgs {
    
    public readonly ModifiedScene Scene;

    public SceneInitializeArgs(ModifiedScene scene) => this.Scene = scene;
}