using System;
using System.Numerics;
using System.Text;
using Easel;
using Easel.Core;
using Easel.Entities;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Pie;
using Pie.Windowing;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class DebugOverlay : Overlay {
    
    private readonly uint _fontSize;
    private readonly Position _position;
    
    public static string CpuInfo = SystemInfo.CpuInfo;
    public static string MemoryInfo = SystemInfo.MemoryInfo;
    public static string OsInfo = Environment.OSVersion.VersionString;

    public DebugOverlay() {
        this._fontSize = 18;
        this._position = new Position(new Vector2T<int>(5, 5));
    }

    public override void Draw() {
        StringBuilder builder = new StringBuilder();
        
        this.AddText(builder, "Stellution " + StellutionClient.Version, "aqua");
        this.AddText(builder, "Easel " + EaselGame.Version, "aqua");
        this.AddText(builder, "Pie " + PieMetrics.Version, "aqua");
        this.AddText(builder);
        this.AddText(builder, "System information:", "blue");
        this.AddText(builder, "    CPU: " + CpuInfo, "aqua");
        this.AddText(builder, "    Memory: " + MemoryInfo, "aqua");
        this.AddText(builder, "    Logical threads: " + SystemInfo.LogicalThreads, "aqua");
        this.AddText(builder, "    OS: " + OsInfo, "aqua");
        this.AddText(builder, "    Graphic API: " + this.Graphics.PieGraphics.Api.ToFriendlyString(), "aqua");
        this.AddText(builder, "    Log: " + Logger.LogFilePath, "aqua");
        this.AddText(builder);
        this.AddText(builder, "Render information:", "darkred");
        this.AddText(builder, "    FPS: " + Metrics.FPS + " (dt: " + MathF.Round(Time.DeltaTime * 1000, 1) + "ms)", "red");
        this.AddText(builder, "    Total Frames: " + Metrics.TotalFrames, "red");
        this.AddText(builder, "    Total VBuffers: " + PieMetrics.VertexBufferCount, "red");
        this.AddText(builder, "    Total IBuffers: " + PieMetrics.IndexBufferCount, "red");
        this.AddText(builder, "    Total CBuffers: " + PieMetrics.UniformBufferCount, "red");
        this.AddText(builder, "    Draws: " + PieMetrics.DrawCalls, "red");
        this.AddText(builder, "    Tris: " + PieMetrics.TriCount, "red");
        this.AddText(builder, "    Window Size: " + StellutionClient.Instance.Window.Size, "red");
        this.AddText(builder);
        this.AddText(builder, "World information:", "darkgreen");
        this.AddText(builder, "    XYZ: " + (int) Camera.Main.Transform.Position.X + " / " + (int) Camera.Main.Transform.Position.Y + " / " + (int) Camera.Main.Transform.Position.Z, "green");
        this.AddText(builder, "    Active Scene: " + SceneManager.ActiveScene.Name, "green");
        this.AddText(builder, "    Active Entities: " + SceneManager.ActiveScene.GetAllEntities().Length, "green");
        this.AddText(builder);
        this.AddText(builder, "Network information:", "orange");
        this.AddText(builder, "    RTT: " + (StellutionClient.NetworkManager.IsConnected ? StellutionClient.NetworkManager.SmoothRTT : "0"), "yellow");
        this.AddText(builder, "    Client ID: " + (StellutionClient.NetworkManager.IsConnected ? StellutionClient.NetworkManager.Id : "0"), "yellow");
        
        this.DrawLines(builder);
    }

    public override Anchor? GetAnchor() {
        return Anchor.TopLeft;
    }

    protected override void OnKeyPress(Key key) {
#if DEBUG
        if (key == Key.F2) {
            this.Enabled ^= true;
        }
#endif
    }

    // TODO: Easel need to fix the memory issues!
    private void DrawLines(StringBuilder builder) {
        this.SpriteRenderer.DrawRectangle(Vector2.Zero, new Size<float>(600, 600), 0, 0, new Color(Color.Black, 0.2f), Color.White, 0, Vector2.Zero);
        this.DrawText(FontRegistry.Fontoe.Value, builder.ToString(), this._position, this._fontSize);
    }

    private void AddText(StringBuilder builder, string text = "", string color = "") {
        if (text != String.Empty) {
            builder.AppendLine($"[color={color}]" + text);
        } else {
            builder.AppendLine();
        }
    }
}