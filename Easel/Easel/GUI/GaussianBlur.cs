using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class GaussianBlur
{
    public float Radius;
    public int Iterations;

    public GaussianBlur(float radius, int iterations)
    {
        Radius = radius;
        Iterations = iterations;
    }
}