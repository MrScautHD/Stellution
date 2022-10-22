using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour {
    
    private Transform player;
    
    public override void OnNetworkSpawn() {
        if (this.IsClient) {
            this.player = NetworkManager.LocalClient.PlayerObject.transform;
        }
    }
    
    private void Update() {
        if (this.IsClient) {
            Vector3 playerPos = this.player.position;
            
            this.transform.position = new Vector3(playerPos.x, playerPos.y + 2, playerPos.z);
        }
    }
}
