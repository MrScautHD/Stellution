using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public abstract class Entity : NetworkBehaviour {

    private NetworkObject entityNetwork;
    private List<Entity> passengers = new List<Entity>();
    
    [NonSerialized] public Vector3 rotation;

    [NonSerialized] public float xRot;
    [NonSerialized] public float yRot;
    
    public bool isPassenger { get; private set; }
    public bool isVehicle { get; private set; }
    
    public Entity vehicle { get; private set; }

    private void Awake() {
        
    }
    
    public override void OnNetworkSpawn() {
        if (!this.IsOwner) Destroy(this);
        if (!this.IsServer) return;

        // SET DEFAULT PARAMETERS
        Vector3 spawnPoint = new Vector3(this.GetPos().x, this.GetPos().y + 1, this.GetPos().z);
        
        this.SetPos(spawnPoint);
        this.entityNetwork = this.GetComponent<NetworkObject>();
    }

    public override void OnNetworkDespawn() {
        if (this.IsServer && this.entityNetwork != null && this.entityNetwork.IsSpawned) {
            entityNetwork.Despawn();
        }
    }

    public void FixedUpdate() {
        RaycastHit hit;
        bool ray = Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10);
        this.Interact(ray, hit);
    }

    protected virtual void Interact(bool ray, RaycastHit hit) {
        
    }

    public void AddPassenger(Entity passenger, Entity vehicle) {
        if (!vehicle.passengers.Contains(passenger)) {
            vehicle.passengers.Add(passenger);
            
            passenger.SetVehicle(passenger, vehicle);
            passenger.isPassenger = true;
            vehicle.isVehicle = true;
        }
    }

    public void RemovePassenger(Entity passenger, Entity vehicle) {
        if (vehicle.passengers.Contains(passenger)) {
            vehicle.passengers.Remove(passenger);
            
            passenger.SetVehicle(passenger, null);
            passenger.isPassenger = false;
            vehicle.isVehicle = false;
        }
    }
    
    public Entity GetMainPassenger() {
        return this.passengers.FirstOrDefault();
    }

    public List<Entity> GetAllPassengers() {
        return this.passengers;
    }

    public void SetVehicle(Entity passenger, Entity vehicle) {
        passenger.vehicle = vehicle;
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot) {
        this.SetPos(pos);
        this.SetRot(rot);
    }

    public void SetPos(Vector3 pos) {
        this.transform.position = pos;
    }

    public Vector3 GetPos() {
        return this.transform.position;
    }
    
    public void SetRot(Quaternion rot) {
        this.transform.rotation = rot;
    }

    public Quaternion GetRot() {
        return this.transform.rotation;
    }
}
