using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public Light fireLight;
    public ParticleSystem[] particlesArray;
    public bool isEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        if(isEnabled){
            lightTorch();
        }
    }
    
    void lightTorch(){
        for (int i = 0; i < particlesArray.Length; i++)
        {
                particlesArray[i].Play();
        }
        fireLight.enabled = true;
    }

    // Update is called once per frame
   
    void OnMouseDown(){
        lightTorch();
    }
}
