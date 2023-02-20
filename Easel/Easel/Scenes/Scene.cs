using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Audio;
using Easel.Content;
using Easel.Core;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Graphics.Lighting;
using Easel.Math;
using Pie;
using DirectionalLight = Easel.Entities.Components.DirectionalLight;

namespace Easel.Scenes;

/// <summary>
/// Scenes allow multiple different gameplay "levels" without needing a large state machine. For example, you could have
/// a main menu scene, a gameplay scene, etc.
/// Scenes are not required for an easel game, however, if developing a game, you will most likely, and should, use them.
/// </summary>
public abstract class Scene : IDisposable
{
    // This is the array which gets looped through on update and draw - arrays are FAR faster to loop through compared
    // to dictionaries, so we want to use the array for speed reasons.
    private Entity[] _entities;

    // This dictionary doesn't get looped through - it's purpose is to provide fast lookup to an entity in the array.
    // Instead of searching through the entire array when entity stuff is called, the dictionary just contains an array
    // index to the entity which gets returned instead.
    private Dictionary<string, int> _entityPointers;

    /// <summary>
    /// Contains a list of disposable objects that will be automatically disposed when the scene is disposed. Engine
    /// objects, such as textures, will be automatically added to this list on creation (unless you tell them not to).
    /// Pie objects, such as graphics buffers, however, will not, so it is recommended that you add them to this list
    /// so you don't need to remember to delete them later.
    /// </summary>
    /// <remarks>It's recommended you don't clear this list unless you remember to manually clean objects later. You
    /// also don't need to add entities to this list, they are automatically disposed separately.</remarks>
    public List<IDisposable> GarbageCollections;

    private int _entityCount;

    private int _unnamedEntityId;
    
    /// <summary>
    /// The current <see cref="EaselGame"/> instance.
    /// </summary>
    protected EaselGame Game => EaselGame.Instance;

    /// <summary>
    /// The current <see cref="EaselGraphics"/> instance.
    /// </summary>
    protected EaselGraphics Graphics => EaselGame.Instance.GraphicsInternal;

    /// <summary>
    /// The current <see cref="AudioDevice"/> instance.
    /// </summary>
    protected AudioDevice Audio => EaselGame.Instance.AudioInternal;

    protected ContentManager Content => EaselGame.Instance.Content;

    /// <summary>
    /// Create a new scene.
    /// </summary>
    /// <param name="initialCapacity">The starting entity array size. This array doubles in size when its size is exceeded.</param>
    protected Scene(int initialCapacity = 128)
    {
        _entities = new Entity[initialCapacity];
        _entityPointers = new Dictionary<string, int>(initialCapacity);
        GarbageCollections = new List<IDisposable>();
    }

    /// <summary>
    /// Called when this scene is initialized.
    /// </summary>
    protected internal virtual void Initialize()
    {
        if (!EaselGame.Instance.IsServer)
        {
            Size<int> size = (Size<int>) EaselGame.Instance.Window.Size;
            Camera camera = new Camera(EaselMath.ToRadians(70), size.Width / (float) size.Height);
            camera.Tag = Tags.MainCamera;
            AddEntity("Main Camera", camera);
        }

        Entity directionalLight = new Entity();
        directionalLight.AddComponent(new DirectionalLight(new Vector2<float>(EaselMath.ToRadians(0), EaselMath.ToRadians(75)),
            Color.White));
        AddEntity("Sun", directionalLight);
    }

    /// <summary>
    /// Called once per frame during update. Where the base function is called will determine when entities in the scene
    /// are updated.
    /// </summary>
    protected internal virtual void Update()
    {
        for (int i = 0; i < _entityCount; i++)
        {
            ref Entity entity = ref _entities[i];
            if (entity == null || !entity.Enabled)
                continue;
            entity.Update();
        }
    }

    /// <summary>
    /// Called once per frame during draw. Where the base function is called will determine when entities in the scene
    /// are drawn.
    /// </summary>
    protected internal virtual void Draw()
    {
        Entity[] cameras = GetEntitiesWithTag(Tags.MainCamera);
        Graphics.Renderer.NewFrame();

        for (int i = 0; i < _entityCount; i++)
        {
            ref Entity entity = ref _entities[i];
            if (!entity.Enabled)
                continue;
            entity.Draw();
        }

        Size<int> framebufferSize = Graphics.Renderer.MainTarget.Size;

        DirectionalLight sun = null;
        Entity[] lights = GetEntitiesWithComponent<DirectionalLight>();
        if (lights.Length > 0)
            sun = lights[0].GetComponent<DirectionalLight>();
        
        Graphics.Renderer.DirectionalLight = sun?.InternalLight;

        int j = 0;
        foreach (Camera camera in cameras)
        {
            Graphics.PieGraphics.Clear(ClearFlags.Depth | ClearFlags.Stencil);
            
            // Convert the camera's normalized viewport into a viewport pie can understand.
            Rectangle<int> viewport = new Rectangle<int>();
            viewport.X = (int) (framebufferSize.Width * camera.Viewport.X);
            viewport.Y = (int) (framebufferSize.Height * camera.Viewport.Y);
            viewport.Width = (int) (framebufferSize.Width * camera.Viewport.Z) - viewport.X;
            viewport.Height = (int) (framebufferSize.Height * camera.Viewport.W) - viewport.Y;

            Graphics.Viewport = viewport;
            if ((camera.CameraType & CameraType.Camera3D) == CameraType.Camera3D) 
                Graphics.Renderer.Perform3DPass(j == 0 ? camera.CameraInfo : camera.CameraInfo with { ClearColor = null });
            if ((camera.CameraType & CameraType.Camera2D) == CameraType.Camera2D) 
                Graphics.Renderer.Perform2DPass(j == 0 ? camera.CameraInfo : camera.CameraInfo with { ClearColor = null });

            j++;
        }
        
        Graphics.Renderer.DoneFrame();
    }

    /// <summary>
    /// Dispose of all entities in the scene, as well as any outstanding garbage collections.
    /// </summary>
    public virtual void Dispose()
    {
        Logger.Debug("Disposing entities...");
        for (int i = 0; i < _entityCount; i++)
            _entities[i].Dispose();
        
        Logger.Debug("Collecting garbage...");
        foreach (IDisposable disposable in GarbageCollections)
            disposable.Dispose();
        
        Logger.Debug("Scene disposed.");
    }

    /// <summary>
    /// Add an entity to the scene.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <param name="entity">The entity to add.</param>
    public void AddEntity(string name, Entity entity)
    {
        entity.Name = name;
        entity.Initialize();
        
        int count = _entityCount++;
        if (count >= _entities.Length)
            Array.Resize(ref _entities, _entities.Length << 1);
        _entities[count] = entity;
        _entityPointers.Add(name, count);
    }

    /// <summary>
    /// Add an entity to the scene. Use this overload if you don't plan on referencing the entity, or you are
    /// keeping a reference to the entity directly in your code. It will be automatically assigned a name.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void AddEntity(Entity entity)
    {
        entity.Name = _unnamedEntityId++.ToString();
        AddEntity(entity.Name, entity);
    }

    /// <summary>
    /// Remove an entity from the scene.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    public void RemoveEntity(string name)
    {
        int location = _entityPointers[name];
        _entities[location].Dispose();
        _entities[location] = null;
        //GC.Collect();
        _entityCount--;
        _entityPointers.Remove(name);
        if (location == _entityCount)
            return;

        _entityPointers.Remove(_entities[_entityCount].Name);
        _entities[location] = _entities[_entityCount];
        _entities[_entityCount] = null;
        _entityPointers.Add(_entities[location].Name, location);
    }

    /// <summary>
    /// Remove an entity from the scene.
    /// </summary>
    /// <param name="entity">The entity reference.</param>
    public void RemoveEntity(Entity entity) => RemoveEntity(entity.Name);

    /// <summary>
    /// Get the entity reference with the given name.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <returns>The entity with the given name. If not found, returns null.</returns>
    public Entity GetEntity(string name)
    {
        if (!_entityPointers.TryGetValue(name, out int ptr))
            return null;

        return _entities[ptr];
    }

    /// <summary>
    /// Get the entity with the given name.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <typeparam name="T">The entity type, for example <see cref="Camera"/>.</typeparam>
    /// <returns>The entity with the given name. If not found, returns null.</returns>
    public T GetEntity<T>(string name) where T : Entity => (T) GetEntity(name);

    /// <summary>
    /// Get all entities in the scene with the given tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>All entities with the given tag.</returns>
    public Entity[] GetEntitiesWithTag(string tag)
    {
        List<Entity> entities = new List<Entity>();
        for (int i = 0; i < _entityCount; i++)
        {
            ref Entity entity = ref _entities[i];
            if (entity.Tag == tag)
                entities.Add(entity);
        }

        return entities.ToArray();
    }

    public Entity[] GetEntitiesWithComponent<T>() where T : Component
    {
        List<Entity> entities = new List<Entity>();
        for (int i = 0; i < _entityCount; i++)
        {
            if (_entities[i].HasComponent<T>())
                entities.Add(_entities[i]);
        }

        return entities.ToArray();
    }

    /// <summary>
    /// Get all entities in the current scene.
    /// </summary>
    /// <returns>The entity array.</returns>
    public Entity[] GetAllEntities()
    {
        return _entities[.._entityCount];
    }
}