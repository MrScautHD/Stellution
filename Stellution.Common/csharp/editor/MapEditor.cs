using Easel.Entities;
using Easel.Scenes;
using Stellution.Common.csharp.entity;

namespace Stellution.Common.csharp.editor; 

public class MapEditor {

    private Scene Scene => SceneManager.ActiveScene;

    public MapEditor() {
        
    }
    
    public Entity GetEntity(string entityName) {
        return this.Scene.GetEntity(entityName);
    }

    public List<ModifiedEntity> GetEntitiesByKey(string key) {
        List<ModifiedEntity> entities = new List<ModifiedEntity>();

        foreach (ModifiedEntity entity in this.Scene.GetAllEntities()) {
            if (entity.Key == key) {
                entities.Add(entity);
            }
        }

        return entities;
    }

    public Entity[] GetAllEntities() {
        return this.Scene.GetAllEntities();
    }

    public void AddEntity(Entity entity) {
        this.Scene.AddEntity(entity);
    }

    public void RemoveEntity(Entity entity) {
        this.Scene.RemoveEntity(entity);
    }
}