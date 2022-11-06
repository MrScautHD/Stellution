using UnityEngine;

public class FlyingCar : AbstractVehicle {

    [SerializeField] private Rigidbody carBody;

    [SerializeField] private float speed = 20;

    private void Update() {
        base.Update();
        
        this.Move();
        this.PassangerMovement();
    }

    private void Move() {
        Vector3 pos = this.transform.position;

        this.transform.position = new Vector3(pos.x, pos.y, pos.z);
        
        if (this.GetMainPassanger() != null) {
            float horizontalInput = Input.GetAxis("Vertical");
            
            Vector3 move = this.transform.right * horizontalInput;
            this.carBody.MovePosition(move * this.speed * Time.deltaTime);
        }
    }

    private void PassangerMovement() {
        if (this.GetMainPassanger() == null) return;
        
        this.GetMainPassanger().transform.position = this.transform.position;
        this.GetMainPassanger().transform.rotation = this.transform.rotation;
    }
}
