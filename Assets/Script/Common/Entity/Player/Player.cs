using Unity.Netcode;
using UnityEngine;

public class Player : LivingEntity {
    
    [SerializeField] private CharacterController controller;
    [SerializeField] private float mouseSensitivity;
    
    [SerializeField] private float speed = 12;
    [SerializeField] private float gravity = -9.81F;
    [SerializeField] private float jumpHeight = 3F;
    
    private Transform cam;
    private Vector3 movement;

    private bool isSprinting;

    public void Update() {
        base.Update();
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        this.Move(horizontalInput, verticalInput);
        this.SetupCamera();

        if (Input.GetKey(KeyCode.LeftShift) && this.IsPassanger()) {
            this.RemovePassanger(this);
        }
    }
    
    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }

    public override bool Interact(bool ray, RaycastHit hit) {
        base.Interact(ray, hit);

        if (ray) {
            FlyingCar flyingCar = hit.collider.gameObject.GetComponent<FlyingCar>();
            
            if (flyingCar != null) {
                if (Input.GetKey(KeyCode.E)) {
                    flyingCar.AddPassanger(this);
                }
            }
        }

        return ray;
    }

    private void Move(float horizontalInput, float verticalInput) {
        if (this.IsPassanger()) return;
        
        // JUMP
        if (Input.GetButton("Jump") && this.IsOnGround()) {
            this.movement.y = Mathf.Sqrt(this.jumpHeight * -2F * this.gravity);
        }

        // SPRINTING
        if (Input.GetKey(KeyCode.LeftShift) && this.IsOnGround()) {
            this.isSprinting = true;
            this.speed = 16;
        } else {
            this.isSprinting = false;
            this.speed = 12;
        }
        
        // MOVEMENT
        Vector3 move = this.transform.right * horizontalInput + this.transform.forward * verticalInput;
        this.controller.Move(move * this.speed * Time.deltaTime);
        
        // GRAVITY
        this.movement.y += (this.gravity - 2) * Time.deltaTime;
        this.controller.Move(movement * Time.deltaTime);
    }
    
    protected override void SetupRotation() {
        float mouseX = Input.GetAxis("Mouse X") * this.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * this.mouseSensitivity * Time.deltaTime;
        
        Vector3 xVec = Vector3.up * mouseX;
        
        this.SetXRot(this.GetXRot() + xVec.y);
        this.transform.Rotate(xVec);
        
        this.SetYRot(this.GetYRot() - mouseY);
        this.SetYRot(Mathf.Clamp(this.GetYRot(), -90F, 90F));

        this.SetRot(new Vector3(this.GetYRot(), this.GetXRot(), 0));
        this.RotateCam(Quaternion.Euler(this.GetRot()));
    }
    
    private void SetupCamera() {
        if (UnityEngine.Camera.main != null && this.cam == null) {
            this.cam = UnityEngine.Camera.main.transform;
        }
    }
    
    private void RotateCam(Quaternion quaternion) {
        if (this.cam != null && this.IsClient) {
            this.cam.localRotation = quaternion;
        }
    }

    public bool IsOnGround() {
        return this.controller.isGrounded;
    }

    public bool IsSprinting() {
        return this.isSprinting;
    }
}
