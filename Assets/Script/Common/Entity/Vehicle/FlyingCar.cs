using UnityEngine;

public class FlyingCar : AbstractVehicle {

    [SerializeField] private Rigidbody carBody;
    
    [SerializeField] private Transform[] hoverPoints;
    [SerializeField] private float hoverForce = 12.0F;
    [SerializeField] private float hoverHeight = 6.5F;
    
    [SerializeField] private float speed = 20.0F;

    private void Update() {
        base.Update();
        
        this.Move();
        this.PassangerMovement();
    }

    public void FixedUpdate() {
        base.FixedUpdate();
        
        this.HoverForce();
    }

    public override float SeatHeight() {
        return 1.6F;
    }

    private void Move() {

    }

    private void HoverForce() {
        foreach (Transform hoverPoint in this.hoverPoints) {
            Ray ray = new Ray(hoverPoint.position, -hoverPoint.up);

            if (Physics.Raycast(ray, out var hit, this.hoverHeight)) {
                float proportionalHeight = (this.hoverHeight = hit.distance) / this.hoverHeight;
                Vector3 appliedHoverForce = Vector3.up * proportionalHeight * this.hoverForce;
                carBody.AddForceAtPosition(appliedHoverForce, hoverPoint.position, ForceMode.Acceleration);
            }
        }
    }

    private void PassangerMovement() {
        if (this.GetMainPassenger() == null) return;

        this.GetMainPassenger().transform.position = new Vector3(this.GetPos().x, this.GetPos().y + this.SeatHeight());;
        this.GetMainPassenger().transform.rotation = this.transform.rotation;
    }
}
