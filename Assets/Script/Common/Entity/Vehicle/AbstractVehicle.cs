using UnityEngine;

public abstract class AbstractVehicle : Entity {

    public abstract float SeatHeight();

    public Vector3 GetSeatHeight() {
        return new Vector3(this.GetPos().x, this.GetPos().y + this.SeatHeight(), this.GetPos().z);
    }
}
