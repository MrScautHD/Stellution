using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorArrow : NetworkBehaviour {

    public Texture2D CURSOR_TEXTURE;

    public override void OnNetworkSpawn() {
        if (!this.IsClient) return;
    }

    private void Start() {
        Cursor.SetCursor(CURSOR_TEXTURE, Vector2.zero, CursorMode.ForceSoftware);
    }
    
    private void Update() {
        if (SceneManager.GetActiveScene().name.Equals("Game")) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
