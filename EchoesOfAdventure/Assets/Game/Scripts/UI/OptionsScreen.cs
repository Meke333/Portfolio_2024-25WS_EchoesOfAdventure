using System;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD.Studio;

public class OptionsScreen : MonoBehaviour
{
    [Header("UI")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button backButton;
    public GameObject mainMenu;
    public PauseMenu pauseMenuScript;

    [Header("FMOD Busses")]
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    [SerializeField] private string masterBusPath = "bus:/";   
    [SerializeField] private string musicBusPath = "bus:/Music"; // Replace with your FMOD Music bus path
    [SerializeField] private string sfxBusPath = "bus:/SFX";     // Replace with your FMOD SFX bus path
    
    private void OnEnable()
    {
        // Get references to the buses
        masterBus = RuntimeManager.GetBus(masterBusPath);
        musicBus = RuntimeManager.GetBus(musicBusPath);
        sfxBus = RuntimeManager.GetBus(sfxBusPath);

        
        // Initialize sliders with current bus volumes
        float masterVolume;
        masterBus.getVolume(out masterVolume);
        masterSlider.value = masterVolume;
        
        // Initialize sliders with current bus volumes
        float musicVolume;
        musicBus.getVolume(out musicVolume);
        musicSlider.value = musicVolume;

        float sfxVolume;
        sfxBus.getVolume(out sfxVolume);
        sfxSlider.value = sfxVolume;

        // Add listeners for slider value changes
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
                
        backButton.onClick.AddListener(BackToMainMenu);
        masterSlider.Select();
    }


    private void BackToMainMenu()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);

        if (pauseMenuScript == null)
            return;

        pauseMenuScript.SelectFirstButton();
    }

    private void OnMasterVolumeChanged(float value)
    {
        // Set the master bus volume
        masterBus.setVolume(value);
    }
    
    private void OnMusicVolumeChanged(float value)
    {
        // Set the music bus volume
        musicBus.setVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        // Set the SFX bus volume
        sfxBus.setVolume(value);
    }

    private void OnDisable()
    {
        // Clean up listeners when the script is destroyed
        masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }
}