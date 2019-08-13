using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenuController : MonoBehaviour
{
    Achievements achievementsData;
    public SoundEffect SEClick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void closeCreditsMenu(){
        StartCoroutine("closeMenu");
    }
    IEnumerator closeMenu(){
        SEClick.playSound();
        yield return new WaitForSeconds(0.1f);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CreditsMenu"));
    }
}
