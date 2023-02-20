using System;
using System.Collections.Generic;
using Easel.Core;
using Pie;

namespace Easel.Graphics;

public class RasterizerState : IDisposable
{
    private static Dictionary<RasterizerStateDescription, RasterizerState> _cachedStates;

    static RasterizerState()
    {
        _cachedStates = new Dictionary<RasterizerStateDescription, RasterizerState>();
    }

    public Pie.RasterizerState PieRasterizerState;

    private RasterizerState(in RasterizerStateDescription description)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        PieRasterizerState = device.CreateRasterizerState(description);
    }

    public static RasterizerState FromDescription(in RasterizerStateDescription description)
    {
        if (!_cachedStates.TryGetValue(description, out RasterizerState state))
        {
            Logger.Debug("Creating new rasterizer state.");
            state = new RasterizerState(description);
            _cachedStates.Add(description, state);
        }

        return state;
    }

    public static RasterizerState CullNone => FromDescription(RasterizerStateDescription.CullNone);
    
    public static RasterizerState CullClockwise => FromDescription(RasterizerStateDescription.CullClockwise);
    
    public static RasterizerState CullCounterClockwise => FromDescription(RasterizerStateDescription.CullCounterClockwise);

    public void Dispose()
    {
        _cachedStates.Remove(PieRasterizerState.Description);
        PieRasterizerState.Dispose();
    }
}