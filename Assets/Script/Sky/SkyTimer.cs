using Unity.Netcode;
using UnityEngine;

public class SkyTimer : NetworkBehaviour {

    //[SerializeField] private AtmosPad atmosPad;
    
    private NetworkVariable<float> timeOfDay = new NetworkVariable<float>(6, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn() {
        if (this.IsClient) OnDestroy();
    }

    public void Update() {
        //this.timeOfDay.Value = (this.atmosPad.Hour > 24) ? 0 : this.timeOfDay.Value + 8e-5F;

        //this.atmosPad.SetHour(this.timeOfDay.Value);
    }
}
