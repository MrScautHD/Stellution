using System;
using System.Collections.Generic;
using Easel.Audio;
using Easel.Content;
using Easel.Core;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Interfaces;
using Easel.Scenes;

namespace Easel.Entities;

/// <summary>
/// An entity, sometimes known as a game object, is the base object in the Entity-Component (EC) system. (Note, this is
/// <b>NOT</b> an ECS, which is very different). These entities can both have components added to them, such as
/// <see cref="ModelRenderer"/>, and can be inherited from, such as <see cref="Camera"/>. Most of the time, you will be
/// adding components to the entity instead of inheriting, but the choice is yours.
/// </summary>
public class Entity : InheritableEntity, IDisposable
{
    protected override EaselGame Game => EaselGame.Instance;

    protected override EaselGraphics Graphics => EaselGame.Instance.GraphicsInternal;

    protected override Scene ActiveScene => SceneManager.ActiveScene;

    protected override AudioDevice Audio => EaselGame.Instance.AudioInternal;

    protected override ContentManager Content => EaselGame.Instance.Content;

    /// <summary>
    /// The parent of this entity, if any.
    /// </summary>
    public Entity Parent;

    /// <summary>
    /// The name of this entity as it is currently stored in the scene.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The tag, if any, of this entity.
    /// </summary>
    public string Tag;

    /// <summary>
    /// If <see langword="false"/>, the entity will not be updated nor drawn, however will still exist in the scene.
    /// </summary>
    public bool Enabled;

    /// <summary>
    /// This entity's <see cref="Easel.Entities.Transform"/>. Unlike other engines, a transform is required for each
    /// entity, and is not a component.
    /// </summary>
    public Transform Transform;

    private Component[] _components;

    private Dictionary<Type, int> _componentPointers;

    private int _componentCount;

    private bool _hasInitialized;

    /// <summary>
    /// Create a new <see cref="Entity"/> with the default <see cref="Entities.Transform"/>.
    /// </summary>
    /// <param name="initialCapacity">The starting capacity of the <see cref="Component"/> array. This array doubles
    /// in size if you exceed its size.</param>
    public Entity(int initialCapacity = 16) : this(new Transform(), initialCapacity) { }

    /// <summary>
    /// Create a new <see cref="Entity"/>.
    /// </summary>
    /// <param name="transform">The starting <see cref="Entities.Transform"/> of this entity.</param>
    /// <param name="initialCapacity">The starting capacity of the <see cref="Component"/> array. This array doubles
    /// in size if you exceed its size.</param>
    public Entity(Transform transform, int initialCapacity = 16)
    {
        Transform = transform;
        Enabled = true;
        _components = new Component[initialCapacity];
        _componentPointers = new Dictionary<Type, int>(initialCapacity);
    }

    protected internal virtual void Initialize()
    {
        for (int i = 0; i < _componentCount; i++)
            _components[i].Initialize();

        _hasInitialized = true;
    }

    protected internal virtual void Update()
    {
        for (int i = 0; i < _componentCount; i++)
        {
            if (!_components[i].Enabled)
                continue;
            _components[i].Update();
        }
    }

    protected internal virtual void Draw()
    {
        for (int i = 0; i < _componentCount; i++)
        {
            if (!_components[i].Enabled)
                continue;
            _components[i].Draw();
        }
    }

    /// <summary>
    /// Dispose of this entity and the components inside it.
    /// </summary>
    public virtual void Dispose()
    {
        Logger.Debug($"Disposing {_componentCount} components...");
        for (int i = 0; i < _componentCount; i++)
            _components[i].Dispose();
        Logger.Debug("Entity disposed.");
    }

    /// <summary>
    /// Add a new component to this entity.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <exception cref="EaselException">Thrown if the entity already contains this component's type. Entities
    /// cannot have more than one component with the same type.</exception>
    /// <remarks>If the entity has not been added to the scene, the component will not be initialized until it is
    /// added to the scene. Otherwise, the component is initialized when it is added.</remarks>
    public void AddComponent(Component component)
    {
        if (!TryAddComponent(component))
            throw new EaselException("Entities cannot have more than one type of each component.");
    }

    public bool TryAddComponent(Component component)
    {
        Type type = component.GetType();
        if (_componentPointers.ContainsKey(type))
            return false;
        
        component.Entity = this;
        if (_hasInitialized)
            component.Initialize();
        
        int count = _componentCount++;
        if (count >= _components.Length)
            Array.Resize(ref _components, _components.Length << 1);
        _components[count] = component;
        _componentPointers.Add(type, count);

        return true;
    }

    /// <summary>
    /// Get the component with the given type on this entity.
    /// </summary>
    /// <typeparam name="T">The type of component to get.</typeparam>
    /// <returns>The found component. If not found, returns null.</returns>
    public T GetComponent<T>() where T : Component
    {
        if (!_componentPointers.TryGetValue(typeof(T), out int ptr))
            return null;

        return (T) _components[ptr];
    }

    public bool TryGetComponent<T>(out T component) where T : Component
    {
        return (component = GetComponent<T>()) != null;
    }

    public bool HasComponent<T>() where T : Component
    {
        return GetComponent<T>() != null;
    }

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