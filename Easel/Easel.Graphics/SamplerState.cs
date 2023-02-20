using System;
using System.Collections.Generic;
using Easel.Core;
using Pie;

namespace Easel.Graphics;

public class SamplerState : IDisposable
{
    private static Dictionary<SamplerStateDescription, SamplerState> _cachedStates;

    static SamplerState()
    {
        _cachedStates = new Dictionary<SamplerStateDescription, SamplerState>();
    }

    public Pie.SamplerState PieSamplerState;

    private SamplerState(in SamplerStateDescription description)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        PieSamplerState = device.CreateSamplerState(description);
    }

    public static SamplerState FromDescription(in SamplerStateDescription description)
    {
        if (!_cachedStates.TryGetValue(description, out SamplerState state))
        {
            Logger.Debug("Creating new sampler state.");
            state = new SamplerState(description);
            _cachedStates.Add(description, state);
        }

        return state;
    }

    public static SamplerState PointClamp => FromDescription(SamplerStateDescription.PointClamp);

    public static SamplerState PointRepeat => FromDescription(SamplerStateDescription.PointRepeat);
    
    public static SamplerState LinearClamp => FromDescription(SamplerStateDescription.LinearClamp);

    public static SamplerState LinearRepeat => FromDescription(SamplerStateDescription.LinearRepeat);
    
    public static SamplerState AnisotropicClamp => FromDescription(SamplerStateDescription.AnisotropicClamp);

    public static SamplerState AnisotropicRepeat => FromDescription(SamplerStateDescription.AnisotropicRepeat);

    public void Dispose()
    {
        _cachedStates.Remove(PieSamplerState.Description);
        PieSamplerState.Dispose();
    }
}