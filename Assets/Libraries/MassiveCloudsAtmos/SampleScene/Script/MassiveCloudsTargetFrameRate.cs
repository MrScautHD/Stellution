using UnityEngine;

namespace Mewlist.MassiveClouds.Sample
{
    public class MassiveCloudsTargetFrameRate : MonoBehaviour
    {
        public int target = 300;

        private void Start ()
        {
            Application.targetFrameRate = target;
        }
    }
}
