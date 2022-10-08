using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class AtmosGroup
    {
        [SerializeField] private List<AtmosProfile> profiles;

        public List<AtmosProfile> Profiles { get { return profiles; } }

        public AtmosGroup()
        {
            profiles = new List<AtmosProfile>();
        }

        public void Add(AtmosProfile profile)
        {
            profiles.Add(profile);
        }

        public Rect Bounds()
        {
            var ys = profiles.Select(x => x.Position.y);
            var enumerable = ys as float[] ?? ys.ToArray();
            return new Rect(0f, enumerable.Min() - 0.05f, 1f, enumerable.Max() - enumerable.Min() + 0.1f);
        }
    }
}