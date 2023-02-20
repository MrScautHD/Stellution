using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Graphics;
using Easel.Math;

namespace Easel.GUI;

public static class UI
{
    private static Dictionary<string, UIElement> _elements;

    public static UITheme Theme;

    public static Tooltip CurrentTooltip;

    internal static Font DefaultFont;

    static UI()
    {
        _elements = new Dictionary<string, UIElement>();

        DefaultFont = new Font(Utils.LoadEmbeddedResource(Assembly.GetExecutingAssembly(), "Easel.Roboto-Regular.ttf"));
        Theme = new UITheme()
        {
            Font = DefaultFont
        };
    }

    public static void Add(string id, UIElement element)
    {
        _elements.Add(id, element);
    }

    public static void Remove(string id)
    {
        _elements.Remove(id, out UIElement element);
    }

    public static T Get<T>(string id) where T : UIElement
    {
        return (T) _elements[id];
    }

    public static void Clear()
    {
        _elements.Clear();
    }

    internal static void Update(Rectangle<int> viewport)
    {
        CurrentTooltip = null;
        
        bool mouseCaptured = false;
        foreach ((_, UIElement element) in _elements.Reverse())
            element.Update(ref mouseCaptured, viewport);
    }

    internal static void Draw(EaselGraphics graphics)
    {
        graphics.SpriteRenderer.Begin();
        
        foreach ((_, UIElement element) in _elements)
            element.Draw(graphics.SpriteRenderer);
        
        CurrentTooltip?.Draw(graphics.SpriteRenderer);
        
        graphics.SpriteRenderer.End();
    }
}