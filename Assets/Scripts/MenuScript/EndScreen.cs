using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject congratulation;
    public DifficultyScaler _difficultyScaler;
    public SoundEffect SE_click;
    public Player _player;

    // Start is called before the first frame update

    public void endGame(bool isGameOver){
        //Stop game and pause it.
        gameObject.SetActive(true);
        _difficultyScaler.pauseTimer();
        Time.timeScale = 0;
        //Enable gameover text;
        gameOver.SetActive(isGameOver);
        //congratulation.SetActive(!isGameOver);
    }
    public void toggleEndScreen(){
        //Open screen, disable gameover Text
        gameOver.SetActive(false);
        congratulation.SetActive(false);
        //open menu
        gameObject.SetActive(!gameObject.activeSelf);
        //pause time
        if(gameObject.activeSelf){
            _difficultyScaler.pauseTimer();
            Time.timeScale = 0;
        }else{
            _difficultyScaler.playTimer();
            Time.timeScale = 1;
        }
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
    public void glossary(){
        SE_click.playSound();
        SceneManager.LoadScene("GlossaryMenu", LoadSceneMode.Additive);
    }
    public void toMainMenu(){
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu",LoadSceneMode.Single);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
