using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public SoundEffect SEClick;
    void Start() {
        InterScene.jsonDataString = InterScene.fetchJsonData("data.json");
		InterScene.jsonSaveFileString= InterScene.fetchJsonData("savefile.json");
		InterScene.parseJsonString();
        InterScene.resetSingleRunAchievementsProgress();
    }
    public void newGame(){
        SEClick.playSound();
        StartCoroutine("loadSingleScene");
    }
    IEnumerator loadSingleScene(){
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("ClassMenu", LoadSceneMode.Additive);
    }
    public void option(){
        SEClick.playSound();
        SceneManager.LoadScene("OptionMenu", LoadSceneMode.Additive);
    }
    public void credits(){
        SEClick.playSound();
        SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Additive);
    }
    public void achievements(){
        SEClick.playSound();
        SceneManager.LoadScene("AchievementsMenu", LoadSceneMode.Additive);
    }
    public void quit(){
        SEClick.playSound();
        Application.Quit();
    }

}
