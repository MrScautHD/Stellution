
public abstract class AbstractVehicle : Entity {

    private void Start() {
        base.Start();
    }

    private void Update() {
        base.Update();
    }

    public abstract float SeatHeight();
}
