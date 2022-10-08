using System.Collections.Generic;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class DistanceComparer : IComparer<int>
    {
        private readonly MassiveCloudsStylizedCloud massiveClouds;
        private Camera targetCamera;

        public DistanceComparer(MassiveCloudsStylizedCloud massiveClouds)
        {
            this.massiveClouds = massiveClouds;
        }

        public void SetTargetCamera(Camera camera)
        {
            targetCamera = camera;
        }

        public int Compare(int lhs, int rhs)
        {
            var profiles = massiveClouds.Profiles;
            var parameters = massiveClouds.Parameters;
            var l = profiles[lhs] == null ? float.MaxValue : Distance(parameters[lhs]);
            var r = profiles[rhs] == null ? float.MaxValue : Distance(parameters[rhs]);
            if (l == r) return 0;
            return l < r ? 1 : -1;
        }

        private float Distance(MassiveCloudsParameter parameter)
        {
            var cameraPos = targetCamera.transform.position;
            if (parameter.Horizontal)
            {
                if (parameter.RelativeHeight)
                    return parameter.FromHeight;
                else
                    return Mathf.Min(
                        Mathf.Abs(cameraPos.y - parameter.FromHeight),
                        Mathf.Abs(cameraPos.y - parameter.ToHeight));
            }
            else
            {
                return parameter.FromDistance;
            }
        }
    }
}