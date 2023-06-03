using System;
using System.Collections.Generic;
using System.Linq;
using Easel.Entities;
using Easel.Graphics;
using Easel.GUI;
using Easel.Math;
using Pie.Windowing;
using Stellution.Client.csharp.registry.types;
using Stellution.Client.csharp.util;
using Stellution.Common.csharp.editor;
using Stellution.Common.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class MapEditorOverlay : Overlay {

    private MapEditor _mapEditor;
    private readonly int _distance = 15;
    private Dictionary<string, Action> _buttons;

    public MapEditorOverlay() {
        this._mapEditor = new MapEditor();
        this._buttons = new();
    }

    public override void Draw() {
        this.DrawImage(Texture2D.Black, new Position(Anchor.TopLeft), new Size<int>(105 * 2, 177 * 2));
        this.DrawText(FontRegistry.Fontoe.Value, "Map Editor", new Position(Anchor.TopLeft), 29);
    }

    protected void DrawButton(Texture2D texture, Position position, Size<int> size, Color color) {
        this.DrawImage(texture, position, size, color);
    }

    public override Anchor? GetAnchor() {
        return Anchor.TopLeft;
    }

    protected override void OnMousePress(MouseButton button) {
        if (button == MouseButton.Right) {

            Entity entity = PrefabRegistry.Entities.ElementAt(2).Value.Invoke();
            entity.Transform = new Transform() {
                Position = CameraHelper.ReachDistance(this._distance)
            };

            this._mapEditor.AddEntity(entity);
        }
    }

    public void OnButtonPress(string key) {
        this._buttons[key].Invoke();
    }

    public void AddButton(string key, Action func) {
        this._buttons.Add(key, func);
    }

    protected override void OnKeyPress(Key key) {
        if (key == Key.F8) {
            this.Enabled ^= true;
        }
    }
}