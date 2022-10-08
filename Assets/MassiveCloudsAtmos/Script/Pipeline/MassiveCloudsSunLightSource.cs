using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [RequireComponent(typeof(Light))]
    public class MassiveCloudsSunLightSource : MonoBehaviour
    {
        public Light Light
        {
            get
            {
                return GetComponent<Light>();
            }
        }
    }
}