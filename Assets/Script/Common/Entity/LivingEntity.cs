
public abstract class LivingEntity : Entity {

    public void Update() {
        base.Update();
        this.SetupRotation();
    }
    
    protected abstract void SetupRotation();
}