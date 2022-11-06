using Unity.Netcode;
using UnityEngine;

public class CursorArrow : NetworkBehaviour {

    public Texture2D CURSOR_TEXTURE;

    public override void OnNetworkSpawn() {
        if (!this.IsClient) OnDestroy();
    }

    private void Start() {
        DontDestroyOnLoad(this);
        //Cursor.SetCursor(CURSOR_TEXTURE, Vector2.zero, CursorMode.ForceSoftware);
    }
    
    private void Update() {
        if (Methods.IsInGame()) {
            Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }
    }
}
