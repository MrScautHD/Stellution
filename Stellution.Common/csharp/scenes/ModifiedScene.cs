using Easel;
using Easel.Scenes;
using Stellution.Common.csharp.args;

namespace Stellution.Common.csharp.scenes; 

public abstract class ModifiedScene : Scene {

    private readonly bool _physic;
    public static event EventHandler<SceneInitializeArgs>? Initializing;

    protected ModifiedScene(string name, bool physic = false, int initialCapacity = 128) : base(name, initialCapacity) {
        this._physic = physic;
    }

    protected override void Initialize() {
        base.Initialize();
        
        if (this._physic) {
            //Physics.Initialize(new PhysicsInitializeSettings());
        }

        Initializing?.Invoke(null, new SceneInitializeArgs(this));
    }

    protected override void Update() {
        base.Update();
        
        if (this._physic) {
            //Physics.Timestep(Time.DeltaTime);
        }
    }
}