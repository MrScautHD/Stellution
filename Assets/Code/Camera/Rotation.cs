using UnityEngine;

public class Rotation : MonoBehaviour
{

    public float mouseSensitivity;
    public Transform playerBody;

    public float yRot;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;

        // Y ROT
        this.yRot -= mouseY;
        this.yRot = Mathf.Clamp(this.yRot, -90F, 90F);
        
        transform.localRotation = Quaternion.Euler(this.yRot, 0F, 0F);
        
        // X ROT
        this.playerBody.Rotate(Vector3.up * mouseX);
    }
}
