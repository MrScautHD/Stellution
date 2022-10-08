using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mewlist.MassiveClouds
{
    [RequireComponent(typeof(Toggle))]
    public class MassiveCloudsProfileChanger : MonoBehaviour
    {
        [SerializeField] private MassiveCloudsStylizedCloudProfile stylizedCloudProfile = null;
        [SerializeField] private MassiveCloudsStylizedCloud massiveClouds = null;
        [SerializeField] private Text label = null;

        public void SetProfile(MassiveCloudsStylizedCloudProfile stylizedCloudProfile)
        {
            this.stylizedCloudProfile = stylizedCloudProfile;
            label.text = stylizedCloudProfile.name;
        }
		
        private Toggle Toggle
        {
            get { return GetComponent<Toggle>(); }
        }
	
        private void OnEnable()
        {
            Toggle.onValueChanged.AddListener(Switch);
        }
	
        private void OnDisable()
        {
            Toggle.onValueChanged.RemoveListener(Switch);
        }
	
        // Use this for initialization
        void Switch (bool isOn)
        {
            if (!isOn) return;

            if (massiveClouds != null)
            {
                massiveClouds.SetProfiles(new List<MassiveCloudsStylizedCloudProfile>() {stylizedCloudProfile});
            }
        }
    }
}