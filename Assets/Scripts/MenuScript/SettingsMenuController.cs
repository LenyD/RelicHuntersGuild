using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
public class SettingsMenuController : MonoBehaviour
{
    Resolution[] resolutions;
    public AudioMixer _mixer;
    public TMP_Dropdown _resolutionDropdown;
    public Slider _masterVolumeSlider;
    public Slider _musicVolumeSlider;
    public Slider _soundEffectVolumeSlider;
    public Toggle _fullscreenToggle;
    public Toggle _tutorialToggle;
    public SoundEffect SE_click;
    public GameObject click;
    // Start is called before the first frame update
    void Awake() {
        //Initiate form to fit current settings
        _fullscreenToggle.isOn = Screen.fullScreen;
        _tutorialToggle.isOn = InterScene.toggleTutorial;
        float volMasterValue;
        float volMusicValue;
        float volSoundEffectValue;
        bool volMaster = _mixer.GetFloat("VolMaster", out volMasterValue);
        bool volMusic = _mixer.GetFloat("VolMusic", out volMusicValue);
        bool volSoundEffect = _mixer.GetFloat("VolSoundEffect", out volSoundEffectValue);
        //Set sound to saved sound file for each channel
        if(volMaster){
            _masterVolumeSlider.value = volumeDBToPercent(volMasterValue);
        }else{            
            volMasterValue = 100;        
        } 

        if(volMusic){
            _musicVolumeSlider.value = volumeDBToPercent(volMusicValue);
        }else{            
            volMusicValue = 100;        
        } 

        if(volSoundEffect){
            _soundEffectVolumeSlider.value = volumeDBToPercent(volSoundEffectValue);
        }else{            
            volSoundEffectValue = 100;        
        }  
        int currentResolution = 0;
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();
        //Initiate possible resolutions for the dropdown
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " X "+resolutions[i].height+" @"+resolutions[i].refreshRate+"Hz");
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height){
                currentResolution = i;
            }
        }
        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentResolution;
    }
    void Start()
    {
        
    }
    public void setResolution(int id){
        //Link with according field onChange
        InterScene.settings.resolutionId = id;
        Screen.SetResolution(resolutions[id].width,resolutions[id].height,Screen.fullScreenMode);
    }
    public void setFullScreen(bool b){
        //Link with according field onChange
        InterScene.settings.fullscreen = b;
        Screen.fullScreen = b;
    }
    public void setTutorial(bool b){
        //Link with according field onChange
        InterScene.settings.toggleTutorial = b;
        InterScene.toggleTutorial = b;
    }
    public void setMasterVolume(float v){
        //Link with according field onChange
        float scaledV = volumePercentToDB(v);
        InterScene.settings.volume.master = scaledV;
        _mixer.SetFloat("VolMaster", scaledV);
    }
    public void setMusicVolume(float v){
        //Link with according field onChange
        float scaledV = volumePercentToDB(v);
        InterScene.settings.volume.music = scaledV;
        _mixer.SetFloat("VolMusic", scaledV);
    }
    public void setSoundEffectVolume(float v){
        //Link with according field onChange
        float scaledV = volumePercentToDB(v);
        InterScene.settings.volume.soundEffect = scaledV;
        _mixer.SetFloat("VolSoundEffect", scaledV);
    }
    float volumePercentToDB(float p){
        //Return volume scaled from % to DB
        float db = 20*Mathf.Log10(p/100);
        return db;
    }
    float volumeDBToPercent(float db){
        //Return volume scaled from DB to %
        float p = Mathf.Pow(10,db/20)*100;
        return p;
    }

    public void closeOptionsMenu(){
        GameObject sound = Instantiate(click);
        SceneManager.MoveGameObjectToScene( sound,SceneManager.GetSceneAt(0));
        sound.GetComponent<SoundEffect>().playSound();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("OptionMenu"));
    }
}
