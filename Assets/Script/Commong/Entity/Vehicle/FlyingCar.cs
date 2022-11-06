using UnityEngine;

public class FlyingCar : AbstractVehicle {

    [SerializeField] private Rigidbody carBody;

    [SerializeField] private float speed = 20;

    private void Update() {
        base.Update();
        
        this.Move();
        this.PassangerMovement();
    }

    public override float SeatHeight() {
        return 1.6F;
    }

    private void Move() {
        if (this.GetMainPassenger() != null) {
            float horizontalInput = Input.GetAxis("Vertical");
            
            Vector3 move = this.transform.right * horizontalInput;
            this.carBody.MovePosition(move * this.speed * Time.deltaTime);
        }
    }

    private void PassangerMovement() {
        if (this.GetMainPassenger() == null) return;

        this.GetMainPassenger().transform.position = new Vector3(this.GetPos().x, this.GetPos().y + this.SeatHeight());;
        this.GetMainPassenger().transform.rotation = this.transform.rotation;
    }
}
