using Mewlist.MassiveClouds;
using UnityEngine;

public class SkyTimer : MonoBehaviour
{

    public AtmosPad atmosPad;
    private float timer;

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        this.timer += deltaTime / 30;

        if (this.atmosPad.Hour > 24)
        {
            this.timer = 0;
        }
        
        this.atmosPad.SetHour(this.timer);
    }
}
