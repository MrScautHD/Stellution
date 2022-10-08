using UnityEngine;
using UnityEngine.UI;

namespace Mewlist.MassiveClouds
{
    [RequireComponent(typeof(Slider))]
    public class AtmosPadTimeSliderControl : MonoBehaviour
    {
        public AtmosPad atmosPad;

        public void Start()
        {
            GetComponent<Slider>().value = atmosPad.Hour;
        }
        
        public void SetHour(Slider slider)
        {
            atmosPad.SetHour(slider.value);
        }
    }
}