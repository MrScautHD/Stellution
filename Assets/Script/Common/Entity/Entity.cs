using Unity.Netcode;
using UnityEngine;

public abstract class Entity : NetworkBehaviour {
    
    private Vector3 rotation;

    private float xRot;
    private float yRot;
    
    private bool isPassanger;
    private bool isVehicle;
    
    public void Start() {

    }

    public void Update() {
        
    }
    
    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }
    
    public void SetPassanger(bool condition) {
        this.isPassanger = condition;
    }

    public bool IsPassanger() {
        return this.isPassanger;
    }
    
    public void SetVehicle(bool condition) {
        this.isVehicle = condition;
    }

    public bool IsVehicle() {
        return this.isVehicle;
    }

    public void SetRot(Vector3 rotation) {
        this.rotation = rotation;
    }

    public Vector3 GetRot() {
        return this.rotation;
    }

    public void SetXRot(float rotation) {
        this.xRot = rotation;
    }

    public float GetXRot() {
        return this.xRot;
    }
    
    public void SetYRot(float rotation) {
        this.yRot = rotation;
    }
    
    public float GetYRot() {
        return this.yRot;
    }
}
