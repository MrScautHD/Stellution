using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlyingCar : AbstractVehicle {
    
    private Rigidbody carBody;

    [SerializeField] private Transform[] hoverPoints;
    [SerializeField] private float hoverForce = 12.0F;
    [SerializeField] private float hoverHeight = 6.5F;
    
    [SerializeField] private float speed = 20.0F;

    private void Awake() {
        this.carBody = this.GetComponent<Rigidbody>();
    }

    private void Update() {
        base.Update();
    }

    public void FixedUpdate() {
        //base.FixedUpdate();
        
        this.Movement();
        this.PassangerMovement();
    }

    public override float SeatHeight() {
        return 1.6F;
    }

    private void Movement() {
        this.HoverForce();
    }

    private void HoverForce() {
        foreach (Transform hoverPoint in this.hoverPoints) {
            Ray ray = new Ray(hoverPoint.position, -hoverPoint.up);

            if (Physics.Raycast(ray, out var hit, this.hoverHeight)) {
                float proportionalHeight = (this.hoverHeight - hit.distance) / this.hoverHeight;
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
