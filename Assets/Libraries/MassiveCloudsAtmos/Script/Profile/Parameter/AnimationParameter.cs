using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class AnimationParameter
    {
        public Vector3 ScrollVelocity;
        [Range(-1f, 1f)] public float Phase;

        private Vector3 scrollPos;

        public void Update()
        {
            scrollPos += ScrollVelocity * (Time.deltaTime * 1000f) / 3600f;
        }

        public void ApplyTo(Material mat)
        {
            mat.SetFloat("_Phase", Phase);
            mat.SetVector("_ScrollVelocity", Vector3.zero);
            mat.SetVector("_ScrollOffset", scrollPos);
        }
    }
}