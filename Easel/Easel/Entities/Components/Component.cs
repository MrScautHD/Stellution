using System;
using Easel.Audio;
using Easel.Content;
using Easel.Graphics;
using Easel.Interfaces;
using Easel.Scenes;

namespace Easel.Entities.Components;

/// <summary>
/// The base class for all entity components and scripts.
/// </summary>
public abstract class Component : InheritableEntity, IDisposable
{
    public bool Enabled;

    protected override EaselGame Game => EaselGame.Instance;
    
    protected override EaselGraphics Graphics => EaselGame.Instance.GraphicsInternal;
    
    protected override Scene ActiveScene => SceneManager.ActiveScene;
    
    protected override AudioDevice Audio => EaselGame.Instance.AudioInternal;

    protected override ContentManager Content => EaselGame.Instance.Content;

    protected internal Entity Entity { get; internal set; }

    protected Component()
    {
        Enabled = true;
    }

    /// <summary>
    /// The <see cref="Easel.Entities.Transform"/> of the current entity.
    /// </summary>
    protected Transform Transform => Entity.Transform;

    /// <summary>
    /// Called during entity initialization (usually when the entity is added to the scene), or, if the entity is already
    /// initialized, called when the component is added to the entity.
    /// </summary>
    protected internal virtual void Initialize() { }

    /// <summary>
    /// Called once per frame during update.
    /// </summary>
    protected internal virtual void Update() { }

    /// <summary>
    /// Called once per frame during draw.
    /// </summary>
    protected internal virtual void Draw() { }

    /// <summary>
    /// Called when an entity is removed from the scene, or when the scene changes.
    /// Use this to dispose of native resources.
    /// </summary>
    public virtual void Dispose() { }

    protected void AddComponent(Component component) => Entity.AddComponent(component);

    protected bool TryAddComponent(Component component) => Entity.TryAddComponent(component);

    /// <summary>
    /// Get the component with the given type on the current entity.
    /// </summary>
    /// <typeparam name="T">The type of component to get.</typeparam>
    /// <returns>The found component. If not found, returns null.</returns>
    protected T GetComponent<T>() where T : Component => Entity.GetComponent<T>();

    protected bool TryGetComponent<T>(out T component) where T : Component => Entity.TryGetComponent(out component);

    protected bool HasComponent<T>() where T : Component => Entity.HasComponent<T>();

    protected override void AddEntity(string name, Entity entity) => SceneManager.ActiveScene.AddEntity(name, entity);

    protected override void AddEntity(Entity entity) => SceneManager.ActiveScene.AddEntity(entity);

    protected override void RemoveEntity(string name) => SceneManager.ActiveScene.RemoveEntity(name);

    protected override void RemoveEntity(Entity entity) => SceneManager.ActiveScene.RemoveEntity(entity);

    protected override Entity GetEntity(string name) => SceneManager.ActiveScene.GetEntity(name);

    protected override T GetEntity<T>(string name) => SceneManager.ActiveScene.GetEntity<T>(name);

    protected override Entity[] GetEntitiesWithTag(string tag) => SceneManager.ActiveScene.GetEntitiesWithTag(tag);

    protected override Entity[] GetEntitiesWithComponent<T>() => SceneManager.ActiveScene.GetEntitiesWithComponent<T>();

    protected override Entity[] GetAllEntities() => SceneManager.ActiveScene.GetAllEntities();
}