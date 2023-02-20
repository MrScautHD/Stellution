using System;
using Easel.Core;
using Easel.GUI;

namespace Easel.Scenes;

public static class SceneManager
{
    private static Scene _activeScene;
    private static Scene _switchScene;

    public static Scene ActiveScene => _activeScene;

    internal static void InitializeScene(Scene scene)
    {
        _activeScene = scene;
    }

    internal static void Initialize()
    {
        Logger.Debug("Initializing SceneManager...");
        if (_activeScene == null)
            Logger.Info("Scene was null, will not use a scene by default.");
        else
            _activeScene.Initialize();
        _switchScene = null;
    }

    internal static void Update()
    {
        if (_switchScene != null)
        {
            _activeScene?.Dispose();
            _activeScene = null;
            GC.Collect();
            UI.Clear();
            _activeScene = _switchScene;
            _activeScene.Initialize();
            _switchScene = null;
        }
        
        _activeScene?.Update();
    }

    internal static void Draw()
    {
        if (_activeScene != null)
        {
            _activeScene.Draw();
        }
    }

    /// <summary>
    /// Set the scene that will be transitioned to.
    /// </summary>
    /// <param name="scene">The scene to use.</param>
    /// <remarks>Transitioning occurs at the start of the update cycle.</remarks>
    public static void SetScene(Scene scene)
    {
        _switchScene = scene;
    }
}