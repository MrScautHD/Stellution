using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {

    [SerializeField] private GameObject loadingScreen;

    public void LoadScene(string scene) {
        NetworkManager.Singleton.SceneManager.LoadScene(scene, LoadSceneMode.Single);
        
        this.loadingScreen.SetActive(true);
    }
}
