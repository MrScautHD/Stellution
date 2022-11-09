using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public abstract class Entity : NetworkBehaviour {

    private List<Entity> passengers = new List<Entity>();
    [NonSerialized] public Vector3 rotation;

    [NonSerialized] public float xRot;
    [NonSerialized] public float yRot;
    
    private bool isPassenger;
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

    protected virtual void Interact(bool ray, RaycastHit hit) {
        
    }

    public void AddPassenger(Entity passenger, Entity vehicle) {
        if (!vehicle.passengers.Contains(passenger)) {
            vehicle.passengers.Add(passenger);
            
            passenger.SetVehicle(passenger, vehicle);
            passenger.isPassenger = true;
        }
    }

    public void RemovePassenger(Entity passenger, Entity vehicle) {
        if (vehicle.passengers.Contains(passenger)) {
            vehicle.passengers.Remove(passenger);
            
            passenger.SetVehicle(passenger, null);
            passenger.isPassenger = false;
        }
    }
    
    public Entity GetMainPassenger() {
        return this.passengers.FirstOrDefault();
    }

    public List<Entity> GetAllPassengers() {
        return this.passengers;
    }
    
    public bool IsPassenger() {
        return this.isPassenger;
    }

    public void SetVehicle(Entity passenger ,Entity vehicle) {
        passenger.vehicle = vehicle;
        vehicle.isVehicle = vehicle != null;
    }

    public void SetPos(Vector3 pos) {
        this.transform.position = pos;
    }

    public Vector3 GetPos() {
        return this.transform.position;
    }

    public Entity GetVehicle() {
        return this.vehicle;
    }

    public bool IsVehicle() {
        return this.isVehicle;
    }
}
