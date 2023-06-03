using System.Numerics;
using Easel.Entities;
using Easel.Math;

namespace Stellution.Client.csharp.util; 

public class CameraHelper {

    public static Vector3 ReachDistance(int distance) {
        Vector3 camPos = Camera.Main.Transform.Position;
        Vector3 camRot = Camera.Main.Transform.Forward;

        float positionX = camPos.X + camRot.X * distance;
        float positionY = camPos.Y + camRot.Y * distance;
        float positionZ = camPos.Z + camRot.Z * distance;

        return new Vector3(positionX, positionY, positionZ);
    }

    public static void SetFov(int fov) {
        Camera.Main.FieldOfView = EaselMath.ToRadians(fov);
    }
}