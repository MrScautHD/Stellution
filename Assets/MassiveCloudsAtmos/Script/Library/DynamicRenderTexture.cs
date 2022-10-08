using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Mewlist.MassiveClouds
{
    public class DynamicRenderTexture : IDisposable
    {
        public readonly RenderTextureFormat FormatAlpha;

        private Dictionary<int, RenderTexture> rtMap = new Dictionary<int, RenderTexture>();
        private Dictionary<int, RenderTextureDesc> rtDescMap = new Dictionary<int, RenderTextureDesc>();

        public RenderTexture GetRenderTexture(Camera camera)
        {
            if (!rtMap.ContainsKey(camera.GetInstanceID())) return null;
            return rtMap[camera.GetInstanceID()];
        }
        
        public DynamicRenderTexture(RenderTextureFormat formatAlpha)
        {
            FormatAlpha = formatAlpha;
        } 

        public RenderTexture CreateRenderTexture(RenderTextureDesc desc)
        {
            RenderTexture rt;
            if (XRSettings.enabled && XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes)
            {
                var rtDesc = new RenderTextureDescriptor(Mathf.RoundToInt(desc.Width * 2), desc.Height, FormatAlpha, 0)
                {
                    vrUsage = VRTextureUsage.TwoEyes,
                };
                rtDesc.autoGenerateMips = false;
                rt = new RenderTexture(rtDesc);
                rt.filterMode = FilterMode.Bilinear;
            }
            else
            {
                var rtDesc = new RenderTextureDescriptor(Mathf.RoundToInt(desc.Width), desc.Height, FormatAlpha, 0)
                {
                    useMipMap = false,
                };
                rt = new RenderTexture(rtDesc);
                rt.filterMode = FilterMode.Bilinear;
            }

            rt.name = "MassiveCloudsRT" + DateTime.Now.Millisecond;
            return rt;
        }

        public void Update(Camera targetCamera, RenderTextureDesc rtDesc)
        {
            if (!rtDescMap.ContainsKey(targetCamera.GetInstanceID()))
                rtDescMap[targetCamera.GetInstanceID()] = new RenderTextureDesc();

            var needCreate = !rtMap.ContainsKey(targetCamera.GetInstanceID())
                             || !rtMap[targetCamera.GetInstanceID()]
                             || !rtDescMap[targetCamera.GetInstanceID()].Equals(rtDesc);
            
            if (needCreate)
            {
                if (rtMap.ContainsKey(targetCamera.GetInstanceID()) && rtMap[targetCamera.GetInstanceID()])
                    rtMap[targetCamera.GetInstanceID()].Release();

                rtDescMap[targetCamera.GetInstanceID()] = rtDesc;
                rtMap[targetCamera.GetInstanceID()] = CreateRenderTexture(rtDesc);
            }
        }

        public void Dispose()
        {
            foreach (var x in rtMap)
            {
                if (x.Value)
                    x.Value.Release();
            }
            rtMap.Clear();
            rtDescMap.Clear();
        }
    }
}