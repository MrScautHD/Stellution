using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {

    [SerializeField] private CharacterController controller;

    [SerializeField] private float groundDistance = 0.4F;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private float speed = 12;
    [SerializeField] private float gravity = -9.81F;
    [SerializeField] private float jumpHeight = 3F;

    private Vector3 vec;

    private bool isSprinting;

    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }

    public void Update() {
        if (this.controller.isGrounded && vec.y < 0) {
            vec.y = -2F;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // JUMP
        if (Input.GetButton("Jump") && this.IsOnGround()) {
            vec.y = Mathf.Sqrt(this.jumpHeight * -2F * this.gravity);
        }

        // SPRINTHING
        if (Input.GetKey(KeyCode.LeftShift) && this.IsOnGround()) {
            this.isSprinting = true;
            this.speed = 16;
        } else {
            this.isSprinting = false;
            this.speed = 12;
        }

        // MOVEMENT
        Vector3 move = this.transform.right * x + this.transform.forward * z;
        this.controller.Move(move * speed * Time.deltaTime);

        // GRAVITY
        vec.y += this.gravity * Time.deltaTime;
        this.controller.Move(vec * Time.deltaTime);
    }

    public bool IsOnGround() {
        return this.controller.isGrounded;
    }

    public bool IsSprinting() {
        return this.isSprinting;
    }
}
