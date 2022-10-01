using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    
    public Transform groundCheck;
    public float groundDistance = 0.4F;
    public LayerMask groundMask;
    
    public float speed = 12;
    public float gravity = -9.81F;
    public float jumpHeight = 3F;

    private Vector3 vec;
    
    private bool isOnGround;
    private bool isSprinthing;

    void Update()
    {
        this.isOnGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (this.isOnGround && vec.y < 0)
        {
            vec.y = -2F;
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // JUMP
        if (Input.GetButton("Jump") && isOnGround)
        {
            vec.y = Mathf.Sqrt(jumpHeight * -2F * gravity);
        }
        
        // SPRINTHING
        if (Input.GetKey(KeyCode.LeftShift) && isOnGround)
        {
            this.isSprinthing = true;
            this.speed = 16;
        }
        else
        {
            this.isSprinthing = false;
            this.speed = 12;
        }
        
        // MOVEMENT
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // GRAVITY
        vec.y += gravity * Time.deltaTime;
        controller.Move(vec * Time.deltaTime);
    }
}
