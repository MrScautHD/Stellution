using System.Numerics;
using Easel.Entities;
using Easel.Physics.Shapes;
using JoltPhysicsSharp;
using BoxShape = Easel.Physics.Shapes.BoxShape;

namespace Stellution.Common.csharp.entity.vehicle; 

public class CyberCarEntity : RigidEntity {
    
    public CyberCarEntity(string? entityName = null, int initialCapacity = 16) : base("cyber_car", entityName, initialCapacity) {
    }
    
    public CyberCarEntity(Vector3 position, string? entityName = null, int initialCapacity = 16) : base(position, "cyber_car", entityName, initialCapacity) {
    }

    public CyberCarEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, "cyber_car", entityName, initialCapacity) {
    }
    
    protected override float GetMass() {
        return 10;
    }

    protected override IShape GetCollisionShape() {
        return new BoxShape(new Vector3(3));
    }

    protected override void Update() {
        base.Update();
        
        
        //this.Hover(new Vector3(this.Position.X + 3, this.Position.Y - 3, this.Position.Z), 2F);
        //this.Hover(new Vector3(this.Position.X - 3, this.Position.Y - 3, this.Position.Z), 2F);
        //this.Hover(new Vector3(this.Position.X, this.Position.Y - 3, this.Position.Z + 3), 2F);
        //this.Hover(new Vector3(this.Position.X, this.Position.Y - 3, this.Position.Z - 3), 2F);
    }
    
    public void Hover(Vector3 pos, float hoverHeight) {

        
        /*if (Physics.Raycast(pos, -Vector3.UnitY, 4, out RayHit hit)) {
            entity.GetBulletRigidBody().ApplyForce(new Vector3(0, (entity.Gravity.Y * -1) + entity.Rigidbody.LinearVelocity.Y, 0), pos);
        }

        //Physics.Raycast(pos, -Vector3.UnitY, 4, out RayHit hit);
    

        // TODO FIX THIS
        
        if (Physics.Raycast(pos, -Vector3.UnitY, 10, out RayHit hit)) {
            float availableForce = 99;
            
            if (this.Rigidbody.LinearVelocity.Y < 0) {
                // Cap out upward force based on yForce
                float cappedDampenForce = Math.Min(1F * -this.Rigidbody.LinearVelocity.Y, availableForce);

                // How much force is available for the offset?
                availableForce -= cappedDampenForce;

                this.Rigidbody.ApplyForce(Vector3.UnitY * cappedDampenForce);
            }
            
            // Find upward force scaled by distance left to target height, and cap that amount
            float cappedOffsetForce = Math.Min(0.99F * (10 - (this.Position.Y)), availableForce);
            //ogger.Error(hit.HitPosition.Y + " ");
            
            this.Rigidbody.ApplyForce(Vector3.UnitY * cappedOffsetForce);
        }*/
    }
}