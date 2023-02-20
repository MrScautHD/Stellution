using Easel.Audio;
using Easel.Content;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Graphics;
using Easel.Scenes;

namespace Easel.Interfaces;

// Not actually an interface but the only way to have PROTECTED METHODS ARGH
/// <summary>
/// Provides helpful functions for entities and components that can be inherited from.
/// </summary>
public abstract class InheritableEntity
{
    /// <summary>
    /// The current <see cref="EaselGame"/> instance.
    /// </summary>
    protected abstract EaselGame Game { get; }
    
    /// <summary>
    /// The current <see cref="EaselGraphics"/> instance.
    /// </summary>
    protected abstract EaselGraphics Graphics { get; }
    
    /// <summary>
    /// The currently active <see cref="Scene"/>.
    /// </summary>
    protected abstract Scene ActiveScene { get; }
    
    /// <summary>
    /// The current <see cref="AudioDevice"/> instance.
    /// </summary>
    protected abstract AudioDevice Audio { get; }
    
    protected abstract ContentManager Content { get; }

    /// <summary>
    /// Add an entity to the current scene.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <param name="entity">The entity to add.</param>
    protected abstract void AddEntity(string name, Entity entity);

    /// <summary>
    /// Add an entity to the current scene. Use this overload if you don't plan on referencing the entity, or you are
    /// keeping a reference to the entity directly in your code. It will be automatically assigned a name.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    protected abstract void AddEntity(Entity entity);

    /// <summary>
    /// Remove an entity from the scene.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    protected abstract void RemoveEntity(string name);

    /// <summary>
    /// Remove an entity from the scene.
    /// </summary>
    /// <param name="entity">The entity reference.</param>
    protected abstract void RemoveEntity(Entity entity);

    /// <summary>
    /// Get the entity reference with the given name.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <returns>The entity with the given name. If not found, returns null.</returns>
    protected abstract Entity GetEntity(string name);

    /// <summary>
    /// Get the entity with the given name.
    /// </summary>
    /// <param name="name">The name of the entity.</param>
    /// <typeparam name="T">The entity type, for example <see cref="Camera"/>.</typeparam>
    /// <returns>The entity with the given name. If not found, returns null.</returns>
    protected abstract T GetEntity<T>(string name) where T : Entity;

    /// <summary>
    /// Get all entities in the current scene with the given tag.
    /// </summary>
    /// <param name="tag">The tag to search for.</param>
    /// <returns>All entities with the given tag.</returns>
    protected abstract Entity[] GetEntitiesWithTag(string tag);

    protected abstract Entity[] GetEntitiesWithComponent<T>() where T : Component;

    /// <summary>
    /// Get all entities in the current scene.
    /// </summary>
    /// <returns>The entity array.</returns>
    protected abstract Entity[] GetAllEntities();
}