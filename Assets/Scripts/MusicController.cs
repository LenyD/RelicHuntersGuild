using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource AS;
    public AudioClip music;
    public float targetVolume = 1;
    // Start is called before the first frame update
    void Start()
    {
        AS.clip = music;
        AS.loop = true;
        AS.volume = 0;
        AS.Play();
        StartCoroutine(FadeIn());
    }
    IEnumerator FadeIn(){
        while (AS.volume<targetVolume)
        {
            yield return new WaitForSeconds(0.1f);
            AS.volume += 0.02f;
        }
         AS.volume = targetVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
