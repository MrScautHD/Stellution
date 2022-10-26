using Unity.Netcode;
using UnityEngine;

public class PlayerRotation : NetworkBehaviour {
    
    [SerializeField] private float mouseSensitivity; //TODO ADD TO SETTINGS
    private Transform cam;
    
    private Vector3 Rotation;

    private float xRot;
    private float yRot;

    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }
    
    public void Update() {
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;
        
        this.SetupCamera();
        this.SetupRotation(mouseX, mouseY);
    }

    private void SetupCamera() {
        if (Camera.main != null && this.cam == null) {
            this.cam = Camera.main.transform;
        }
    }

    private void SetupRotation(float mouseX, float mouseY) {
        Vector3 xVec = Vector3.up * mouseX;
        this.xRot += xVec.y;
        this.transform.Rotate(xVec);
        
        this.yRot -= mouseY;
        this.yRot = Mathf.Clamp(this.yRot, -90F, 90F);

        this.Rotation = new Vector3(this.yRot, this.xRot, 0);
        this.RotateCam(Quaternion.Euler(this.Rotation));
    }

    private void RotateCam(Quaternion quaternion) {
        if (this.cam != null && this.IsClient) {
            this.cam.localRotation = quaternion;
        }
    }
}
