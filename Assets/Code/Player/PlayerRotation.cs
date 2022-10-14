using Unity.Netcode;
using UnityEngine;

public class PlayerRotation : NetworkBehaviour {

    [SerializeField] private Transform camera;
    [SerializeField] private float mouseSensitivity; //TODO ADD TO SETTINGS
    
    private float yRot;

    public void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public override void OnNetworkSpawn() {
        if (!this.IsOwner) Destroy(this);
    }
    
    public void Update() {
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
