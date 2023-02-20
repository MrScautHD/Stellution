using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Easel.Graphics.Lighting;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Pie;

namespace Easel.Graphics.Renderers;

public sealed class ForwardRenderer : IRenderer
{
    private List<TransformedRenderable> _opaques;
    private List<Sprite> _opaqueSprites;

    private ProjViewModel _projViewModel;
    private GraphicsBuffer _projViewModelBuffer;

    private SceneInfo _sceneInfo;
    private GraphicsBuffer _sceneInfoBuffer;

    private DepthState _depthState;

    public ForwardRenderer(EaselGraphics graphics, Size<int> initialResolution)
    {
        _opaques = new List<TransformedRenderable>();
        _opaqueSprites = new List<Sprite>();
        _projViewModel = new ProjViewModel();

        MainTarget = new RenderTarget(initialResolution, autoDispose: false);
        
        graphics.SwapchainResized += GraphicsOnSwapchainResized;

        GraphicsDevice device = graphics.PieGraphics;

        _projViewModelBuffer = device.CreateBuffer(BufferType.UniformBuffer, _projViewModel, true);

        _sceneInfo = new SceneInfo();
        _sceneInfoBuffer = device.CreateBuffer(BufferType.UniformBuffer, _sceneInfo, true);

        _depthState = device.CreateDepthState(DepthStateDescription.LessEqual);
    }

    private void GraphicsOnSwapchainResized(Size<int> size)
    {
        MainTarget.Dispose();
        MainTarget = new RenderTarget(size);
    }
    
    public DirectionalLight? DirectionalLight { get; set; }
    public RenderTarget MainTarget { get; set; }

    public void Draw(in Renderable renderable, in Matrix4x4 world)
    {
        _opaques.Add(new TransformedRenderable(renderable, world));
    }

    public void DrawSprite(in Sprite sprite)
    {
        _opaqueSprites.Add(sprite);
    }

    public void NewFrame()
    {
        _opaques.Clear();
        _opaqueSprites.Clear();

        EaselGraphics graphics = EaselGraphics.Instance;
        graphics.SetRenderTarget(MainTarget);
    }

    public void DoneFrame()
    {
        EaselGraphics graphics = EaselGraphics.Instance;
        
        // TODO: EaselGraphics main RT.
        graphics.SetRenderTarget(null);
        
        graphics.Clear(Color.Black);
        graphics.SpriteRenderer.Begin();
        graphics.SpriteRenderer.Draw(MainTarget, Vector2<float>.Zero, null, Color.White, 0, Vector2<float>.Zero, Vector2<float>.One);
        graphics.SpriteRenderer.End();
    }

    public void Perform3DPass(CameraInfo camera)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        
        // First perform depth-only shadow pass
        
        
        
        // Then perform main color pass.
        
        if (camera.ClearColor.HasValue)
            EaselGraphics.Instance.Clear(camera.ClearColor.Value);
        
        _projViewModel.Projection = camera.Projection;
        _projViewModel.View = camera.View;

        _sceneInfo.Sun = DirectionalLight?.ShaderDirLight ?? new ShaderDirLight();
        
        device.SetPrimitiveType(PrimitiveType.TriangleList);
        device.SetDepthState(_depthState);
        
        // Draw front-to-back for opaques.
        // This is to save a bit of GPU time so it doesn't process fragments that are covered by objects in front.
        foreach (TransformedRenderable renderable in _opaques.OrderBy(renderable => Vector3.Distance(renderable.Transform.Translation, camera.Position)))
            DrawRenderable(device, camera, renderable);
        
        // Lastly draw the skybox.
        camera.Skybox?.Draw(camera.Projection, camera.View);
    }

    private void DrawRenderable(GraphicsDevice device, in CameraInfo camera, in TransformedRenderable renderable)
    {
        _projViewModel.Model = renderable.Transform;
        device.UpdateBuffer(_projViewModelBuffer, 0, _projViewModel);

        _sceneInfo.Material = renderable.Renderable.Material.ShaderMaterial;
        _sceneInfo.CameraPos = new Vector4(camera.Position, 1.0f);
        device.UpdateBuffer(_sceneInfoBuffer, 0, _sceneInfo);

        device.SetShader(renderable.Renderable.Material.EffectLayout.Effect.PieShader);
        device.SetUniformBuffer(0, _projViewModelBuffer);
        device.SetUniformBuffer(1, _sceneInfoBuffer);
        renderable.Renderable.Material.ApplyTextures(device);
        device.SetRasterizerState(renderable.Renderable.Material.RasterizerState.PieRasterizerState);

        device.SetVertexBuffer(0, renderable.Renderable.VertexBuffer,
            renderable.Renderable.Material.EffectLayout.Stride,
            renderable.Renderable.Material.EffectLayout.Layout);
        device.SetIndexBuffer(renderable.Renderable.IndexBuffer, IndexType.UInt);
        device.DrawIndexed(renderable.Renderable.NumIndices);
    }

    public void Perform2DPass(CameraInfo camera)
    {
        if (camera.ClearColor.HasValue)
            EaselGraphics.Instance.Clear(camera.ClearColor.Value);
        
        EaselGraphics graphics = EaselGraphics.Instance;
        _opaqueSprites.Sort((sprite, sprite1) => sprite.Position.Z.CompareTo(sprite1.Position.Z));
        
        graphics.SpriteRenderer.Begin(projection: camera.Projection, transform: camera.View);

        for (int i = 0; i < _opaqueSprites.Count; i++)
        {
            Sprite sprite = _opaqueSprites[i];
            graphics.SpriteRenderer.Draw(sprite.Texture, new Vector2<float>(sprite.Position.X, sprite.Position.Y),
                sprite.Source, sprite.Tint, sprite.Rotation, sprite.Origin, sprite.Scale, sprite.Flip);
        }
        
        graphics.SpriteRenderer.End();
    }

    public void Dispose()
    {
        MainTarget.Dispose();
        _projViewModelBuffer.Dispose();
    }
}