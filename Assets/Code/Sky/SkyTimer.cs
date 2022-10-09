using Mewlist.MassiveClouds;
using Unity.Netcode;
using UnityEngine;

public class SkyTimer : MonoBehaviour {

    public AtmosPad atmosPad;
    public float time;
    
    [ClientRpc] //TODO CREATE A SERVER DAY TIMER THAT BINDED TO THIS
    public void Update() {
        float deltaTime = Time.deltaTime;
        this.time += deltaTime / 30;

        if (this.atmosPad.Hour > 24)
        {
            this.time = 0;
        }
        
        this.atmosPad.SetHour(this.time);
    }
}
