using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    // Start is called before the first frame update
    
    public AudioSource AS;
    public AudioClip[] clips;
    public float volume = 0.5f;
    void Start()
    {
    }

    public void playSound(){
        if(AS.isPlaying){
            return;
        }
        int rng = Random.Range(0,clips.Length);
        AS.clip =clips[rng];
        AS.volume = volume;
        AS.Play();
    }
    
    public void setMute(bool b){
        AS.mute = b;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
