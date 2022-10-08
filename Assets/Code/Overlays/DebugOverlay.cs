using TMPro;
using UnityEngine;

public class DebugOverlay : MonoBehaviour
{

    public TMP_Text fpsText;
    public TMP_Text systemInfoText;
    private FpsCounter FpsCounter = new();
    
    public void Start()
    {
        // SYSTEM INFO
        this.systemInfoText.text = "System Info: " + SystemInfo.graphicsDeviceName;
    }

    public void Update()
    {
        // FPS
        this.FpsCounter.Update();
        this.fpsText.text = "FPS: " + this.FpsCounter.GetFps();
    }
}
