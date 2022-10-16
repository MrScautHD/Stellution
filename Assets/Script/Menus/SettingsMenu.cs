using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour {

    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionsDropdown;
    
    private Resolution[] resolutions;
    
    [ClientRpc]
    public void Start() {
        this.SetUpResolution();
    }

    [ClientRpc]
    private void SetUpResolution() {
        this.resolutions = Screen.resolutions;
        
        resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionsDropdown.AddOptions(options);
    }

    [ClientRpc]
    public void SetResolution(int resolutionsIndex) {
        Resolution resolution = this.resolutions[resolutionsIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    
    [ClientRpc]
    public void SetVolume(float volume) {
        this.audioMixer.SetFloat("volume", volume);
    }

    [ClientRpc]
    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    [ClientRpc]
    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }
}
