using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DebugOverlay : NetworkBehaviour {
    
    public TMP_Text fpsText;
    public TMP_Text systemInfoText;
    
    [SerializeField] private FpsCounter fpsCounter;

    public override void OnNetworkSpawn() {
        if (!this.IsClient) OnDestroy();
    }

    public void Start() {
        this.systemInfoText.text = "System Info: " + SystemInfo.graphicsDeviceName;
    }
    
    public void Update() {
        this.fpsCounter.Update();
        this.fpsText.text = "FPS: " + this.fpsCounter.GetFps();
    }
}
