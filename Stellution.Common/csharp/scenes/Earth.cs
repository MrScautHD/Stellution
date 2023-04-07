using System.Numerics;
using BulletSharp;
using Easel;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Math;
using Easel.Physics;
using Stellution.Common.csharp.entity;

namespace Stellution.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth(int initialCapacity = 128) : base("earth", initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();
        Physics.Initialize(new PhysicsInitializeSettings());
        
        Transform carPos = new Transform() { Position = new Vector3(0, 16, 0) };
        RigidEntity cyberCar = new RigidEntity(carPos, 2, new BoxShape(3), "cyber_car", "cyber_car");
        this.AddEntity(cyberCar);
        
        RigidEntity groundEntity = new RigidEntity(0, new BoxShape(10000, 1, 10000), "ground", "ground");
        this.AddEntity(groundEntity);
        
        Transform playPos = new Transform() { Position = new Vector3(10, 0, 0) };
        ModifiedEntity player = new ModifiedEntity(playPos, "player");
        this.AddEntity(player);

        Entity light = this.GetEntity("Sun");
        light.GetComponent<DirectionalLight>().Color = Color.Blue;
    }
    
    protected override void Update() {
        Physics.Timestep(Time.DeltaTime);

        RigidEntity cyberCar = (RigidEntity) this.GetEntity("cyber_car");
        Vector3 pos = cyberCar.Transform.Position;
        
        this.Hover(cyberCar, new Vector3(pos.X + 3, pos.Y - 3, pos.Z), 2F);
        this.Hover(cyberCar, new Vector3(pos.X - 3, pos.Y - 3, pos.Z), 2F);
        this.Hover(cyberCar, new Vector3(pos.X, pos.Y - 3, pos.Z + 3), 2F);
        this.Hover(cyberCar, new Vector3(pos.X, pos.Y - 3, pos.Z - 3), 2F);

        base.Update();
    }

    public void Hover(RigidEntity entity, Vector3 pos, float hoverHeight) {
        /**if (Physics.Raycast(pos, -Vector3.UnitY, 4, out RayHit hit)) {
            entity.GetBulletRigidBody().ApplyForce(new Vector3(0, (entity.GetBulletRigidBody().Gravity.Y * -1) + entity.Rigidbody.LinearVelocity.Y, 0), pos);
        }**/

        //Physics.Raycast(pos, -Vector3.UnitY, 4, out RayHit hit);

        if (Physics.Raycast(pos, -Vector3.UnitY, 10, out RayHit hit)) {
            float availableForce = 99;
            
            if (entity.Rigidbody.LinearVelocity.Y < 0) {
                // Cap out upward force based on yForce
                float cappedDampenForce = Math.Min(1F * -entity.Rigidbody.LinearVelocity.Y, availableForce);

                // How much force is available for the offset?
                availableForce -= cappedDampenForce;

                entity.GetBulletRigidBody().ApplyForce(Vector3.UnitY * cappedDampenForce, pos);
            }
            
            // Find upward force scaled by distance left to target height, and cap that amount
            float cappedOffsetForce = Math.Min(0.99F * (10 - (entity.Transform.Position.Y)), availableForce);
            //ogger.Error(hit.HitPosition.Y + " ");

            entity.GetBulletRigidBody().ApplyForce(Vector3.UnitY * cappedOffsetForce, pos);
        }
    }
}