using System;
using Easel;
using Easel.Entities;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;
using Easel.Scenes;
using Pie;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class DebugOverlay : Overlay {

    private int _lines;
    private readonly uint _fontSize;

    public static string CpuInfo = SystemInfo.CpuInfo;
    public static string MemoryInfo = SystemInfo.MemoryInfo;
    public static string OsInfo = Environment.OSVersion.VersionString;

    public DebugOverlay() {
        this._fontSize = 18;
    }

    public override void Draw(SpriteRenderer renderer) {
        this.DrawLine("System information:", Color.Blue);
        this.DrawLine("    CPU: " + CpuInfo, Color.Aqua);
        this.DrawLine("    Memory: " + MemoryInfo, Color.Aqua);
        this.DrawLine("    Logical threads: " + SystemInfo.LogicalThreads, Color.Aqua);
        this.DrawLine("    OS: " + OsInfo, Color.Aqua);
        this.DrawLine("    Graphic API: " + this.Graphics.PieGraphics.Api.ToFriendlyString(), Color.Aqua);
        this.DrawLine("");
        this.DrawLine("Render information:", Color.DarkRed);
        this.DrawLine("    FPS: " + Metrics.FPS, Color.Red);
        this.DrawLine("    Total Frames: " + Metrics.TotalFrames, Color.Red);
        this.DrawLine("    Window Size: " + StellutionClient.Instance.Window.Size, Color.Red);
        this.DrawLine("");
        this.DrawLine("World information:", Color.DarkGreen);
        this.DrawLine("    XYZ: " + (int) Camera.Main.Transform.Position.X + " / " + (int) Camera.Main.Transform.Position.Y + " / " + (int) Camera.Main.Transform.Position.Z, Color.Green);
        this.DrawLine("    Active Scene: " + SceneManager.ActiveScene.Name, Color.Green);
        this.DrawLine("    Active Entities: " + SceneManager.ActiveScene.GetAllEntities().Length, Color.Green);
        this.DrawLine("");
        this.DrawLine("Network information:", Color.Orange);
        this.DrawLine("    MS: " + (StellutionClient.NetworkManager.IsConnected ? StellutionClient.NetworkManager.SmoothRTT : "0"), Color.Yellow);
        this.DrawLine("    Client ID: " + (StellutionClient.NetworkManager.IsConnected ? StellutionClient.NetworkManager.Id : "0"), Color.Yellow);
        
        this.ClearLines();
    }

    public void DrawLine(string text, Color? color = null) {
        if (text != "") {
            Position pos = new Position(new Vector2T<int>(5, 5 + (int)this._fontSize * this._lines));
            this.DrawText(FontRegistry.Fontoe.Value, text, pos, this._fontSize, color ?? Color.White);
        }

        this._lines += 1;
    }

    public void ClearLines() {
        this._lines = 0;
    }
}