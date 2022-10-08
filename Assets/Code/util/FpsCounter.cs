using System;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    
    private float t;
    private long lastTicks = DateTime.Now.Ticks;
    private int count;
    private int frames;
    
    public void Update ()
    {
        long ticks = DateTime.Now.Ticks;
        this.t += (ticks - this.lastTicks);
        this.lastTicks = ticks;
        this.count++;
        
        if (this.t >= 10000000)
        {
            this.frames = this.count;
            this.count = 0;
            this.t %= 10000000;
        }
    }

    public int GetFps()
    {
        return this.frames;
    }
}
