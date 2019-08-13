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
    public Slider _volumeSlider;
    public Toggle _fullscreenToggle;
    public SoundEffect SEClick;
    // Start is called before the first frame update
    void Start()
    {
        _fullscreenToggle.isOn = Screen.fullScreen;
        float volumeValue;
        bool volumeResult = _mixer.GetFloat("volume", out volumeValue);
        Debug.Log(volumeResult);
        if(volumeResult){
            _volumeSlider.value = volumeDBToPercent(volumeValue);
        }else{
            volumeValue = 100;
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
    public void setVolume(float v){
        float scaledV = volumePercentToDB(v);
        _mixer.SetFloat("volume", scaledV);
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
        SEClick.playSound();
        yield return new WaitForSeconds(0.1f);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("OptionMenu"));
    }
}
