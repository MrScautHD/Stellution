using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    public interface IMassiveCloudsPass<T>
    {
        void Update(T context);

        void BuildCommandBuffer(T context, MassiveCloudsPassContext ctx, FlippingRenderTextures renderTextures);
        void Clear();
    }
}