using Unity.Netcode;
using UnityEngine;

public class PlayerRotation : NetworkBehaviour {
    
    [SerializeField] private float mouseSensitivity; //TODO ADD TO SETTINGS
    private Transform cam;
    
    private Vector3 xRot;
    private float yRot;

    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }
    
    public void Update() {
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;
        
        this.SetupCamera();
        
        this.SetXRot(mouseX);
        this.SetYRot(mouseY);
    }

    private void SetupCamera() {
        if (Camera.main != null && this.cam == null) {
            this.cam = Camera.main.transform;
        }
    }

    private void SetXRot(float mouseX) {
        this.xRot = Vector3.up * mouseX;
        this.transform.Rotate(this.xRot);
        
        if (this.cam != null) {
            this.cam.localRotation = this.transform.rotation;
        }
    }

    private void SetYRot(float mouseY) {
        this.yRot -= mouseY;
        this.yRot = Mathf.Clamp(this.yRot, -90F, 90F);
        
        if (this.cam != null) {
            this.cam.localRotation = Quaternion.Euler(this.yRot, 0F, 0F);
        }
    }
}
