﻿using System.Collections;
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
    public SoundEffect SE_click;
    // Start is called before the first frame update
    void Start()
    {
        _fullscreenToggle.isOn = Screen.fullScreen;
        float volMasterValue;
        float volMusicValue;
        float volSoundEffectValue;
        bool volMaster = _mixer.GetFloat("VolMaster", out volMasterValue);
        bool volMusic = _mixer.GetFloat("VolMusic", out volMusicValue);
        bool volSoundEffect = _mixer.GetFloat("VolSoundEffect", out volSoundEffectValue);
        
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
    public void setResolution(int id){
        Screen.SetResolution(resolutions[id].width,resolutions[id].height,Screen.fullScreenMode);
    }
    public void setFullScreen(bool b){
        Screen.fullScreen = b;
    }
    public void setMasterVolume(float v){
        float scaledV = volumePercentToDB(v);
        _mixer.SetFloat("VolMaster", scaledV);
    }
    public void setMusicVolume(float v){
        float scaledV = volumePercentToDB(v);
        _mixer.SetFloat("VolMusic", scaledV);
    }
    public void setSoundEffectVolume(float v){
        float scaledV = volumePercentToDB(v);
        _mixer.SetFloat("VolSoundEffect", scaledV);
    }
    float volumePercentToDB(float p){

        float db = 20*Mathf.Log10(p/100);
        return db;
    }
    float volumeDBToPercent(float db){
        float p = Mathf.Pow(10,db/20)*100;
        return p;
    }

    public void closeOptionsMenu(){
        StartCoroutine("closeMenu");
    }
    IEnumerator closeMenu(){
        SE_click.playSound();
        yield return new WaitForSeconds(0.1f);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("OptionMenu"));
    }
}
