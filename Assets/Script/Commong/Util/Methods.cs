using UnityEngine;
using UnityEngine.SceneManagement;

public class Methods : MonoBehaviour {

    public static bool IsInGame() {
        return SceneManager.GetActiveScene().name.Equals("Game");
    }
}
