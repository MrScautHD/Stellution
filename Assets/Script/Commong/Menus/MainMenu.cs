using Unity.Netcode;
using UnityEngine;

public class MainMenu : LoadingScene {
    
    private void PlayGame() {
        NetworkManager.Singleton.StartHost();
        this.LoadScene("Game");
    }

    private void QuitGame() {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
