using Mewlist.MassiveClouds;
using Unity.Netcode;
using UnityEngine;

public class SkyTimer : NetworkBehaviour {

    [SerializeField] private AtmosPad atmosPad;
    
    private NetworkVariable<float> timeOfDay = new NetworkVariable<float>(6, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public void Update() {
        this.timeOfDay.Value += 0.001F;
        this.timeOfDay.Value %= 24;

        this.atmosPad.SetHour(this.timeOfDay.Value);
    }
}
