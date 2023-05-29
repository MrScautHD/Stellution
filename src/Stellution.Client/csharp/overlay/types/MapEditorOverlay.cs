using System;
using System.Numerics;
using System.Reflection;
using Easel.Core;
using Easel.Entities;
using Easel.Graphics;
using Easel.GUI;
using Easel.Math;
using Pie.Windowing;
using Stellution.Client.csharp.registry.types;
using Stellution.Common.csharp.editor;
using Stellution.Common.csharp.entity;
using Stellution.Common.csharp.entity.environment;
using Stellution.Common.csharp.entity.player;
using Stellution.Common.csharp.entity.types;
using Stellution.Common.csharp.entity.vehicle;
using Stellution.Common.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class MapEditorOverlay : Overlay {

    private MapEditor _mapEditor;
    
    private int _viewDistance = 15;

    public MapEditorOverlay() {
        this._mapEditor = new MapEditor();
    }

    public override void Draw() {
        this.DrawText(FontRegistry.Fontoe.Value, "Map Editor", new Position(Anchor.TopLeft), 29);
        this.DrawImage(Texture2D.Black, new Position(Anchor.TopLeft), new Size<int>(105 * 2, 177 * 2));
    }

    protected void DrawButton(Texture2D texture, Position position, Size<int> size, Color color) {
        this.DrawImage(texture, position, size, color);
    }

    public override Anchor? GetAnchor() {
        return Anchor.TopLeft;
    }

    protected override void OnMousePress(MouseButton button) {
        if (button == MouseButton.Right) {
            Vector3 camPos = Camera.Main.Transform.Position;
            Vector3 camRot = Camera.Main.Transform.Forward;

            float positionX = camPos.X + camRot.X * this._viewDistance;
            float positionY = camPos.Y + camRot.Y * this._viewDistance;
            float positionZ = camPos.Z + camRot.Z * this._viewDistance;

            Transform transform = new Transform() {
                Position = new Vector3(positionX, positionY, positionZ)
            };

            CyberCarEntity entity = EntityPrefabRegistry.CyberCarEntity.Invoke();
            entity.Transform = transform;
                
            this._mapEditor.AddEntity(entity);
        }
    }

    protected override void OnKeyPress(Key key) {
        if (key == Key.F8) {
            this.Enabled ^= true;
        }
    }
}