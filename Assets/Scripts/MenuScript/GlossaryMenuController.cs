using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlossaryMenuController : MonoBehaviour
{
    Achievements achievementsData;
    public GameObject click;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void closeGlossaryMenu(){
        //Close glossary menu
        GameObject sound = Instantiate(click);
        SceneManager.MoveGameObjectToScene( sound,SceneManager.GetSceneAt(0));
        sound.GetComponent<SoundEffect>().playSound();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("GlossaryMenu"));
    }
}
