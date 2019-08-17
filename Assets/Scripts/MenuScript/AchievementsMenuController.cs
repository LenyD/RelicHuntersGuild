using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementsMenuController : MonoBehaviour
{
    Achievements achievementsData;
    public GameObject achievementsContainer;
    public GameObject achievementsPrefab;
    public GameObject click;
    // Start is called before the first frame update
    void Start()
    {
        //Get data
        achievementsData = InterScene.saveFile.achievements;
        //Fill panels with data
        setUpAchievements();
    }

    void setUpAchievements(){
        //set up the container to be the good height
        achievementsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(achievementsContainer.GetComponent<RectTransform>().sizeDelta.x,150*achievementsData.list.Length);
        for (int i = 0; i < achievementsData.list.Length; i++)
        {
            //instantiate each panel and fills info
            GameObject panel = (GameObject)Instantiate(achievementsPrefab);
            panel.transform.SetParent(achievementsContainer.transform,false);
            panel.GetComponent<AchievementPanel>().setUp(achievementsData.list[i],achievementsData.achievementsUnlock[i]);
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-150*i);
            panel.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        }
    }

    public void closeAchievementsMenu(){
        //Close scene
        GameObject sound = Instantiate(click);
        SceneManager.MoveGameObjectToScene( sound,SceneManager.GetSceneAt(0));
        sound.GetComponent<SoundEffect>().playSound();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("AchievementsMenu"));
    }
}
