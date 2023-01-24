using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer; 

public abstract class AbstractGui : AbstractRenderer {

    private string _guiName;
    private Dictionary<string, int> _buttons;

    private int _guiWidth;
    private int _guiHeight;

    protected AbstractGui(string name, int width, int height) {
        this._guiName = name;
        this._guiWidth = width;
        this._guiHeight = height;
    }
    
    protected override void DrawOnScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        this.DrawGui(graphicsDevice, spriteBatch, view, projection, time);
    }

    protected virtual void DrawGui(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        
    }

    protected virtual void DrawToolTip() {
        
    }

    public void AddButton() {
        
    }

    public void RemoveButton() {
        
    }
    
    public void AddSlot() {
        
    }

    public void RemoveSLot() {
        
    }

    public void DrawSlots() {
        
    }

    public void DrawItems() {
        
    }

    /**
     * like in a pos or something...
     */
    protected bool IsHovering() {
        return true;
    }

    protected virtual bool CanDrawMouse() {
        return true;
    }
    
    public virtual bool CanOpen() {
        return true; //IF Already open one...
    }
    
    public void Open() {
        
    }
    
    public virtual bool CanClose() {
        return true; //IF OPEN
    }

    public void Close() {
        if (this.CanClose()) {
            // CLOSE GUI
        }
    }
}