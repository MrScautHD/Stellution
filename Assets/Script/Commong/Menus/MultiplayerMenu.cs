using Unity.Netcode;

public class MultiplayerMenu : LoadingScene {

    private void Host() {
        NetworkManager.Singleton.StartHost();
        this.LoadScene("Game");
    }

    private void Server() {
        NetworkManager.Singleton.StartServer();
        this.LoadScene("Game");
    }
    
    private void Client() {
        NetworkManager.Singleton.StartClient();
        this.LoadScene("Game");
    }
}
