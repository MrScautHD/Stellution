using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsMixer : IDisposable
    {
        private MassiveCloudsMaterial material = new MassiveCloudsMaterial();
        private MassiveCloudsStylizedCloudProfile from;
        private MassiveCloudsStylizedCloudProfile target;

        private bool lerp = false;
        private float mix = 1;
        private float duration = 1;

        public MassiveCloudsMaterial Material
        {
            get { return material; }
        }

        public MassiveCloudsStylizedCloudProfile CurrentStylizedCloudProfile
        {
            get
            {
                if (lerp) return target;
                return mix < 0.5f ? from : target;
            }
        }

        public MassiveCloudsParameter CurrentParameter
        {
            get
            {
                if (lerp) return target.Lerp(@from, mix);
                return CurrentStylizedCloudProfile.Parameter;
            }
        }

        private float Density
        {
            get
            {
                if (lerp) return 1f;
                return Mathf.Pow(2f * Mathf.Abs(mix - 0.5f), 0.3f);
            }
        }

        public void ChangeTo(MassiveCloudsStylizedCloudProfile stylizedCloudProfile, bool lerp)
        {
            var firstTime = from == null && target == null;

            if (target != stylizedCloudProfile)
            {
                from = target;
                target = stylizedCloudProfile;
                mix = 0f;
                this.lerp = lerp;
            }
            else
            {
                return;
            }

            if (!Application.isPlaying || firstTime)
            {
                mix = 1f;
            }

            material.SetProfile(CurrentStylizedCloudProfile);
            material.SetFade(Density);
        }

        public void SetParameter(MassiveCloudsParameter parameter)
        {
            material.SetParameter(parameter);
        }

        public void SetDuration(float t)
        {
            duration = t;
        }
        
        public void Update()
        {
            if (mix >= 1f) return;
            mix = Mathf.Min(1f, mix + Time.deltaTime / duration);
            material.SetProfile(CurrentStylizedCloudProfile);
            if (CurrentStylizedCloudProfile != null)
                material.SetParameter(CurrentParameter);
            material.SetFade(Density);
        }
        
        public void SetLight(MassiveCloudsLight sun, MassiveCloudsLight moon, float scale)
        {
            material.SetLight(sun, moon, scale);
        }
        
        public void SetAmbientColor(MassiveCloudsAmbient ambient)
        {
            material.SetAmbientColor(ambient);
        }

        public void Dispose()
        {
            material.Dispose();
        }
    }
}