using UnityEngine;
using UnityEngine.UI;

namespace Mewlist.MassiveClouds
{
    [RequireComponent(typeof(Slider))]
    public class AtmosPadVariationSliderControl : MonoBehaviour
    {
        public AtmosPad atmosPad;

        public void Start()
        {
            GetComponent<Slider>().value = atmosPad.Pointer.y;
        }

        public void SetVariation(Slider slider)
        {
            atmosPad.SetVariation(slider.value);
        }
    }
}