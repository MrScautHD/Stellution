using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {

    [SerializeField] private CharacterController controller;
    
    [SerializeField] private float speed = 12;
    [SerializeField] private float gravity = -9.81F;
    [SerializeField] private float jumpHeight = 3F;
    
    private Vector3 vec;

    private bool isSprinting;
    
    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }

    private void Update() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // JUMP
        if (Input.GetButton("Jump") && this.IsOnGround()) {
            this.vec.y = Mathf.Sqrt(this.jumpHeight * -2F * this.gravity);
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
        this.controller.Move(move * speed * Time.deltaTime);
        
        // GRAVITY
        this.vec.y += (this.gravity - 2) * Time.deltaTime;
        this.controller.Move(vec * Time.deltaTime);
    }

    public bool IsOnGround() {
        return this.controller.isGrounded;
    }

    public bool IsSprinting() {
        return this.isSprinting;
    }
}
