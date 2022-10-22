using Mewlist.MassiveClouds;
using Unity.Netcode;
using UnityEngine;

public class SkyTimer : NetworkBehaviour {

    [SerializeField] private AtmosPad atmosPad;
    
    private NetworkVariable<float> timeOfDay = new NetworkVariable<float>(6, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public void Update() {
        if (this.IsClient) return;
        
        this.timeOfDay.Value = (this.atmosPad.Hour > 24) ? 0 : this.timeOfDay.Value + 8e-5F;

        this.atmosPad.SetHour(this.timeOfDay.Value);
    }
}
