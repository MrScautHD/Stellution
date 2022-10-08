using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class AtmosWeightedGroup
    {
        public readonly AtmosGroup atmosGroup;
        public readonly float Weight;

        public AtmosWeightedGroup(AtmosGroup atmosGroup, float weight)
        {
            this.atmosGroup = atmosGroup;
            Weight = weight;
        }

        public List<AtmosWeightedProfile> TargetProfiles(Vector2 pointer)
        {
            var results = new List<AtmosWeightedProfile>();
            var sorted = atmosGroup.Profiles.OrderBy(x => x.Position.x).ToArray();

            var l = sorted
                .Where(x => x.Position.x < pointer.x)
                .OrderBy(x => Mathf.Abs(x.Position.x - pointer.x))
                .FirstOrDefault();
            var lx = (l == null) ? 0 : l.Position.x;
            var r = sorted
                .Where(x => x.Position.x >= pointer.x)
                .OrderBy(x => Mathf.Abs(x.Position.x - pointer.x))
                .FirstOrDefault();
            var rx = (r == null) ? 0 : r.Position.x;
            if (l == null)
            {
                l = sorted.Last();
                if (l == r)
                {
                    results.Add(new AtmosWeightedProfile(r, 1f));
                    return results;
                }
                lx = l.Position.x - 1f;
            }
            else if (r == null)
            {
                r = sorted.First();
                if (l == r)
                {
                    results.Add(new AtmosWeightedProfile(l, 1f));
                    return results;
                }
                rx = r.Position.x + 1f;
            }

            var dl = Mathf.Abs(pointer.x - lx);
            var dr = Mathf.Abs(pointer.x - rx);
            var tl = dr / (dl + dr);
            var tr = dl / (dl + dr);
            results.Add(new AtmosWeightedProfile(l, tl));
            results.Add(new AtmosWeightedProfile(r, tr));
            return results;
        }
    }
}