using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject congratulation;
    public SoundEffect SE_click;

    // Start is called before the first frame update

    public void endGame(bool isGameOver){
        gameObject.SetActive(true);
        gameOver.SetActive(isGameOver);
        congratulation.SetActive(!isGameOver);
    }
    public void toggleEndScreen(){
        gameOver.SetActive(false);
        congratulation.SetActive(false);
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void endExe(){
        Application.Quit();
    }
    public void option(){
        SE_click.playSound();
        SceneManager.LoadScene("OptionMenu", LoadSceneMode.Additive);
    }
    public void achievements(){
        SE_click.playSound();
        SceneManager.LoadScene("AchievementsMenu", LoadSceneMode.Additive);
    }
    public void toMainMenu(){
        SceneManager.LoadScene("StartMenu",LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
