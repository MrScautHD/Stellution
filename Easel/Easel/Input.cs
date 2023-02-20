using System;
using System.Collections.Generic;
using System.Numerics;
using Easel.Math;
using Pie.Windowing;
using Silk.NET.GLFW;
using MouseButton = Pie.Windowing.MouseButton;

namespace Easel;

/// <summary>
/// Provides functions to query input states for the current frame.
/// </summary>
public static class Input
{
    private static HashSet<Key> _keysPressed;
    private static HashSet<Key> _newKeys;

    private static HashSet<MouseButton> _mouseButtonsPressed;
    private static HashSet<MouseButton> _newMouseButtons;

    static Input()
    {
        _keysPressed = new HashSet<Key>();
        _newKeys = new HashSet<Key>();

        _mouseButtonsPressed = new HashSet<MouseButton>();
        _newMouseButtons = new HashSet<MouseButton>();
    }

    /// <summary>
    /// Query if the given key is currently held down.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns><see langword="true"/>, if the key is currently held.</returns>
    public static bool KeyDown(Key key) => _keysPressed.Contains(key);

    public static bool AnyKeyDown(out Key pressedKey, params Key[] keys)
    {
        foreach (Key key in keys)
        {
            if (_keysPressed.Contains(key))
            {
                pressedKey = key;
                return true;
            }
        }

        pressedKey = Key.Unknown;
        return false;
    }

    public static bool AnyKeyDown(params Key[] keys) => AnyKeyDown(out _, keys);
    
    public static bool AnyKeyPressed(out Key pressedKey, params Key[] keys)
    {
        foreach (Key key in keys)
        {
            if (_newKeys.Contains(key))
            {
                pressedKey = key;
                return true;
            }
        }

        pressedKey = Key.Unknown;
        return false;
    }

    public static bool AnyKeyPressed(params Key[] keys) => AnyKeyPressed(out _, keys);

    /// <summary>
    /// Query if the given key was pressed this frame.
    /// </summary>
    /// <param name="key">The key to query.</param>
    /// <returns><see langword="true"/>, if the key was pressed this frame.</returns>
    public static bool KeyPressed(Key key) => _newKeys.Contains(key);

    /// <summary>
    /// Query if the given mouse button is currently held down.
    /// </summary>
    /// <param name="button">The mouse button to query.</param>
    /// <returns><see langword="true"/>, if the mouse button is currently held.</returns>
    public static bool MouseButtonDown(MouseButton button) => _mouseButtonsPressed.Contains(button);

    /// <summary>
    /// Query if the given mouse button was pressed this frame.
    /// </summary>
    /// <param name="button">The mouse button to query.</param>
    /// <returns><see langword="true"/>, if the mouse button was pressed this frame.</returns>
    public static bool MouseButtonPressed(MouseButton button) => _newMouseButtons.Contains(button);
    
    /// <summary>
    /// Get the current mouse position relative to the view (top left = 0, 0)
    /// </summary>
    public static Vector2<float> MousePosition { get; private set; }
    
    /// <summary>
    /// Returns the number of pixels the mouse has moved since the last frame.
    /// </summary>
    public static Vector2<float> DeltaMousePosition { get; private set; }

    private static MouseState _currentMouseState;
    private static bool _mouseStateChanged;

    /// <summary>
    /// Get or set the mouse state for the current game view.
    /// </summary>
    public static MouseState MouseState
    {
        get => _currentMouseState;
        set
        {
            _currentMouseState = value;
            _mouseStateChanged = true;
        }
    }
    
    public static Vector2<float> ScrollWheelDelta { get; private set; }

    internal static void Initialize(Window window)
    {
        window.KeyDown += WindowOnKeyDown;
        window.KeyUp += WindowOnKeyUp;
        window.MouseButtonDown += WindowOnMouseButtonDown;
        window.MouseButtonUp += WindowOnMouseButtonUp;
        window.Scroll += WindowOnScroll;

        InputState state = window.ProcessEvents();
        MousePosition = (Vector2<float>) state.MousePosition;
    }

    internal static void Update(Window window)
    {
        _newKeys.Clear();
        _newMouseButtons.Clear();
        
        ScrollWheelDelta = Vector2<float>.Zero;

        InputState state = window.ProcessEvents();
        DeltaMousePosition = (Vector2<float>) state.MousePosition - MousePosition;
        MousePosition = (Vector2<float>) state.MousePosition;

        if (_mouseStateChanged)
        {
            window.MouseState = _currentMouseState;
            _mouseStateChanged = false;
        }
    }

    private static void WindowOnKeyDown(Key key)
    {
        _keysPressed.Add(key);
        _newKeys.Add(key);
    }
    
    private static void WindowOnKeyUp(Key key)
    {
        _keysPressed.Remove(key);
        _newKeys.Remove(key);
    }
    
    private static void WindowOnMouseButtonDown(MouseButton button)
    {
        _mouseButtonsPressed.Add(button);
        _newMouseButtons.Add(button);
    }
    
    private static void WindowOnMouseButtonUp(MouseButton button)
    {
        _mouseButtonsPressed.Remove(button);
        _newMouseButtons.Remove(button);
    }
    
    private static void WindowOnScroll(System.Numerics.Vector2 scroll)
    {
        ScrollWheelDelta += (Vector2<float>) scroll;
    }
}