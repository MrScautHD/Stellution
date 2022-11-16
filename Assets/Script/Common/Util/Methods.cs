using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Methods : MonoBehaviour {

    public static bool IsInGame() {
        return SceneManager.GetActiveScene().name.Equals("Game");
    }
    
    public static void Spawn(string entityPath, Vector3 pos, Quaternion rot) {
        Entity entitySpawn = Instantiate((Entity) Resources.Load(entityPath, typeof(Entity)), pos, rot);
        entitySpawn.GetComponent<NetworkObject>().Spawn();
    }
}
