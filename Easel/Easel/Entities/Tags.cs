namespace Easel.Entities;

/// <summary>
/// Contains some commonly used tags for entities.
/// </summary>
public static class Tags
{
    /// <summary>
    /// Assign this tag to an entity to make it the main camera of the scene (unless there is already a main camera, then
    /// it is ignored.)
    /// </summary>
    public const string MainCamera = "MainCamera";
}