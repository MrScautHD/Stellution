using Unity.Netcode;
using UnityEngine;

public class PlayerRotation : NetworkBehaviour {

    public Transform camera;
    
    public float mouseSensitivity; //TODO ADD TO SETTINGS
    private float yRot;

    public void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void Update() {
        if (!this.IsOwner) return;
        
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;

        // Y ROT
        this.yRot -= mouseY;
        this.yRot = Mathf.Clamp(this.yRot, -90F, 90F);
        
        this.camera.localRotation = Quaternion.Euler(this.yRot, 0F, 0F);
        
        // X ROT
        this.transform.Rotate(Vector3.up * mouseX);
    }
}
