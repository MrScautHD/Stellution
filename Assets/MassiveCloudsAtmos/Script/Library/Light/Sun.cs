using System;
using System.Linq;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class Sun : MassiveCloudsLight
    {
        private Light reference = null;
        public Light Reference { get { return reference; } }

        public bool HasReference { get { return reference; } }

        public void Detect()
        {
            var lights = GameObject.FindObjectsOfType<Light>()
                .Where(x => x.type == LightType.Directional)
                .OrderByDescending(x => x.intensity).ToList();
            if (lights.Any()) reference = lights.First();
        }

        public void SetReference(Light light)
        {
            reference = light;
        }
    }
}