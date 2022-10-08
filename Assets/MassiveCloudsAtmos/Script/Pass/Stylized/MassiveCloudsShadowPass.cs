using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class MassiveCloudsShadowPass : IMassiveCloudsPass<MassiveCloudsStylizedCloud>
    {
        private DistanceComparer distanceComparer;
        private List<int> sortedIndex = new List<int>();

        public void Update(MassiveCloudsStylizedCloud context)
        {
            // Sorted Index
            if (sortedIndex.Count != context.Profiles.Count)
            {
                sortedIndex.Clear();
                sortedIndex.AddRange(Enumerable.Range(0, context.Profiles.Count));
            }
        }
        
        public void BuildCommandBuffer(MassiveCloudsStylizedCloud context, MassiveCloudsPassContext ctx,
            FlippingRenderTextures renderTextures)
        {
            var targetCamera = ctx.targetCamera;
            var commandBuffer = ctx.cmd;

            if (distanceComparer == null) distanceComparer = new DistanceComparer(context);
            distanceComparer.SetTargetCamera(targetCamera);
            sortedIndex.Sort(distanceComparer);

            for (var i = 0; i < context.Profiles.Count; i++)
            {
                var index = sortedIndex[i];
                if (context.Profiles[index] == null) continue;
                var m = context.Mixers[index];
                commandBuffer.Blit(renderTextures.From, renderTextures.To, m.Material.ShadowMaterial);
                renderTextures.Flip();
            }
        }

        public void Clear()
        {
        }
    }
}