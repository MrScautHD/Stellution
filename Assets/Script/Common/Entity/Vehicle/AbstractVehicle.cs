
public abstract class AbstractVehicle : Entity {

    private void Start() {
        base.Start();
    }

    private void Update() {
        base.Update();
    }

    private void FixedUpdate() {
        base.FixedUpdate();
    }

    public abstract float SeatHeight();
}
