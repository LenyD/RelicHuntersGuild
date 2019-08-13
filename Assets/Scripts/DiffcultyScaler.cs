using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiffcultyScaler : MonoBehaviour
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
        if(isPaused){
            return;
        }
        timeInSec += Time.deltaTime*timeSpeed;
        level = Mathf.FloorToInt(timeInSec/numberOfSecondsPerLevel);
        if(level >= InterScene.jsonData.lootTables.Length){
            level =InterScene.jsonData.lootTables.Length-1;
        }
        InterScene.currentTier = level;
        levelText.text = level.ToString();
        timeText.text = Mathf.CeilToInt(numberOfSecondsPerLevel-(timeInSec%numberOfSecondsPerLevel)).ToString();
    }
    public void reduceTimeSpeed(float number){
        timeSpeed-=number;
    }
}
