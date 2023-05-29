using Easel.Entities;
using Easel.Scenes;

namespace Stellution.Common.csharp.editor; 

public class MapEditor {

    public Scene Scene => SceneManager.ActiveScene;

    public MapEditor() {
        
    }

    public void AddEntity(Entity entity) {
        this.Scene.AddEntity(entity);
    }

    public void RemoveEntity(Entity entity) {
        this.Scene.RemoveEntity(entity);
    }
}