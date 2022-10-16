using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DebugOverlay : MonoBehaviour {
    
    public TMP_Text fpsText;
    public TMP_Text systemInfoText;
    private FpsCounter FpsCounter = new();
    
    [ClientRpc]
    public void Start() {
        // SYSTEM INFO
        this.systemInfoText.text = "System Info: " + SystemInfo.graphicsDeviceName;
    }

    [ClientRpc]
    public void Update() {
        // FPS
        this.FpsCounter.Update();
        this.fpsText.text = "FPS: " + this.FpsCounter.GetFps();
    }
}
