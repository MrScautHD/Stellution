using System.Collections.Generic;
using Easel.Math;

namespace Easel.Audio;

public static class AudioEffect
{
    private static List<FadeEffect> _fades;

    static AudioEffect()
    {
        _fades = new List<FadeEffect>();
    }

    public static void Fade(ISoundInstance instance, double startVolume, double endVolume, float time)
    {
        _fades.Add(new FadeEffect(instance, time, startVolume, endVolume));
    }

    internal static void Update()
    {
        for (int i = 0; i < _fades.Count; i++)
        {
            float lerpValue = (Time.TotalSeconds - _fades[i].StartTime) / (_fades[i].EndTime - _fades[i].StartTime);

            _fades[i].Instance.Volume = EaselMath.Lerp(_fades[i].InitialVolume, _fades[i].EndVolume, lerpValue);
            if (lerpValue >= 1f)
            {
                _fades.Remove(_fades[i]);
                i--;
            }
        }
    }

    private struct FadeEffect
    {
        public ISoundInstance Instance;
        public double InitialVolume;
        public double EndVolume;
        public float StartTime;
        public float EndTime;

        public FadeEffect(ISoundInstance instance, float time, double initialVolume, double endVolume)
        {
            Instance = instance;
            InitialVolume = initialVolume;
            EndVolume = endVolume;
            
            StartTime = Time.TotalSeconds;
            EndTime = StartTime + time;
        }
    }
}