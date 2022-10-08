using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class Moon : MassiveCloudsLight
    {
        private Transform reference = null;
        public Transform Reference { get { return reference; } set { reference = value; } }

        public void SetReference(Transform moonTransform)
        {
            reference = moonTransform;
        }

        public void SetIntensity(float v)
        {
            intensity = v;
        }

        public void SetColor(Color v)
        {
            color = v;
        }
    }
}