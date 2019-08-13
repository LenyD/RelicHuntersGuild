using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementsMenuController : MonoBehaviour
{
    Achievements achievementsData;
    public GameObject achievementsContainer;
    public GameObject achievementsPrefab;
    public SoundEffect SE_click;
    // Start is called before the first frame update
    void Start()
    {
        achievementsData = InterScene.saveFile.achievements;
        setUpAchievements();
    }

    void setUpAchievements(){
        achievementsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(achievementsContainer.GetComponent<RectTransform>().sizeDelta.x,150*achievementsData.list.Length);
        for (int i = 0; i < achievementsData.list.Length; i++)
        {
            GameObject panel = (GameObject)Instantiate(achievementsPrefab);
            panel.transform.SetParent(achievementsContainer.transform,false);
            panel.GetComponent<AchievementPanel>().setUp(achievementsData.list[i],achievementsData.achievementsUnlock[i]);
            panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-150*i);
            panel.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        }
    }

    public void closeAchievementsMenu(){
        StartCoroutine("closeMenu");
    }
    IEnumerator closeMenu(){
        SE_click.playSound();
        yield return new WaitForSeconds(0.1f);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("AchievementsMenu"));
    }
}
