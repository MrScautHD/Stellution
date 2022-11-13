
public abstract class LivingEntity : Entity {

    public void Update() {
        this.SetupRotation();
    }

    protected abstract void SetupRotation();
}