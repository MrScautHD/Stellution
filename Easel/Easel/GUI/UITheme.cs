using System;
using System.IO;
using System.Runtime.InteropServices;
using Easel.Graphics;
using Easel.Math;

namespace Easel.GUI;

/// <summary>
/// Represents the default UI theme for this application. This can be overridden for any UI element.
/// </summary>
public struct UITheme
{
    /// <summary>
    /// The rectangle border width for elements that use rectangles.
    /// </summary>
    public int BorderWidth;

    /// <summary>
    /// The radius, in pixels, of the rectangle's border for elements that use rectangles.
    /// </summary>
    public float BorderRadius;

    /// <summary>
    /// The background color of the rectangle. TODO more docs these docs are the most useless docs i've ever seen docs
    /// </summary>
    public Color BackgroundColor;

    public Color HoverColor;

    public Color ClickedColor;

    public Color ActiveColor;

    public Color BorderColor;

    public float TransitionTime;

    public DropShadow? DropShadow;

    public Font Font;

    public Color FontColor;

    public GaussianBlur Blur;

    public UITheme()
    {
        BorderWidth = 1;
        BorderRadius = 10;
        BackgroundColor = Color.GhostWhite;
        HoverColor = Color.DarkGray;
        ClickedColor = Color.LightGray;
        ActiveColor = Color.DimGray;
        BorderColor = Color.Black;
        TransitionTime = 0;
        DropShadow = null;
        Font = null;
        FontColor = Color.Black;
        Blur = null;
    }
}