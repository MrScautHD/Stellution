using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerMenu : MonoBehaviour {

    private void Host() {
        NetworkManager.Singleton.StartHost();
        this.LoadScene();
    }

    private void Server() {
        NetworkManager.Singleton.StartServer();
        this.LoadScene();
    }
    
    private void Client() {
        NetworkManager.Singleton.StartClient();
        this.LoadScene();
    }

    private void LoadScene() {
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
