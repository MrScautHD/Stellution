using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DebugOverlay : MonoBehaviour {
    
    public TMP_Text fpsText;
    public TMP_Text systemInfoText;
    private FpsCounter fpsCounter = new();
    
    [ClientRpc]
    public void Start() {
        this.systemInfoText.text = "System Info: " + SystemInfo.graphicsDeviceName;
    }

    [ClientRpc]
    public void Update() {
        this.fpsCounter.Update();
        this.fpsText.text = "FPS: " + this.fpsCounter.GetFps();
    }
}
