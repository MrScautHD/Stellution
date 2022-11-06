using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public abstract class Entity : NetworkBehaviour {

    public List<Entity> passangers = new List<Entity>();
    private Vector3 rotation;

    private float xRot;
    private float yRot;
    
    private bool isPassanger;
    private bool isVehicle;
    
    private Entity vehicle;

    public void Start() {
        
    }

    public void Update() {
        
    }

    public void FixedUpdate() {
        RaycastHit hit;
        bool ray = Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10);
        this.Interact(ray, hit);
    }

    public override void OnNetworkSpawn() {
        if (!this.IsOwner) OnDestroy();
    }

    public virtual bool Interact(bool ray, RaycastHit hit) {
        return ray;
    }

    public void AddPassanger(Entity passanger) {
        passanger.isPassanger = true;

        if (!this.passangers.Contains(passanger)) {
            this.passangers.Add(passanger);
            this.SetVehicle(passanger, vehicle);
        }
    }

    public void RemovePassanger(Entity passanger) {
        passanger.isPassanger = false;
        
        if (this.passangers.Contains(passanger)) {
            this.passangers.Remove(passanger);
            this.SetVehicle(passanger, null);
        }
    }
    
    public Entity GetMainPassanger() {
        return this.passangers.FirstOrDefault();
    }
    
    public bool IsPassanger() {
        return this.isPassanger;
    }

    public void SetVehicle(Entity passanger ,Entity vehicle) {
        passanger.vehicle = vehicle;
        this.isVehicle = vehicle != null;
    }

    public Entity GetVehicle() {
        return this.vehicle;
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
