using UnityEngine;

public class Car : AbstractVehicle {

    private Transform passanger;
    [SerializeField] private Rigidbody carBody;

    [SerializeField] private float speed = 20;

    private void Update() {
        
        this.Move();
        this.PassangerMovement();
    }

    private void Move() {
        Vector3 pos = this.transform.position;

        this.transform.position = new Vector3(pos.x, pos.y + 1, pos.z);
        
        if (passanger != null) {
            float horizontalInput = Input.GetAxis("Horizontal");
            
            Vector3 move = this.transform.right * horizontalInput;
            this.carBody.MovePosition(move * this.speed * Time.deltaTime);
        }
    }

    private void PassangerMovement() {
        if (this.passanger == null) return;
        
        this.passanger.position = this.transform.position;
        this.passanger.rotation = this.transform.rotation;
    }

    public void Dismount() {
        this.passanger = null;
    }

    public void SetPassanger(Transform passanger) {
        this.passanger = passanger;
    }
}
