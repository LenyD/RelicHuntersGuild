using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyScaler : MonoBehaviour
{
    public Text timeText;
    public Text levelText;
    float timeInSec;
    int numberOfSecondsPerLevel = 300;
    float timeSpeed = 1;
    int level;
    bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        timeInSec = InterScene.timer;
    }

    public void pauseTimer(){
        InterScene.timer = timeInSec;
        isPaused = true;
    }
    public void playTimer(){
        timeInSec = InterScene.timer;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Do nothing if paused
        if(isPaused){
            return;
        }
        //Reduce time
        //set difficulty level
        timeInSec += Time.deltaTime*timeSpeed;
        level = Mathf.FloorToInt(timeInSec/numberOfSecondsPerLevel);
        if(level >= InterScene.jsonData.lootTables.Length){
            level =InterScene.jsonData.lootTables.Length-1;
        }
        //Save current level
        InterScene.currentTier = level;
        levelText.text = level.ToString();
        timeText.text = Mathf.CeilToInt(numberOfSecondsPerLevel-(timeInSec%numberOfSecondsPerLevel)).ToString();
    }
    public void reduceTimeSpeed(float number){
        timeSpeed-=number;
    }
}
