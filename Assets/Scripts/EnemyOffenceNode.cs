﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyOffenceNode : EnemyNode
{
    // Start is called before the first frame update
    void Start()
    {
        powerDivider = 10;
        delayToSend = 1f;
        image = GetComponent<Image>();
        startFill = 0;
        targetFill = 0;
        isPlayerPopUp = false;
        effect = 3;
    }
    public override void applyEffect(){
        _s.receiveDamage(effect,0);

    }
    // Update is called once per frame
    void Update()
    {
        reduceCooldown();
        if(sendCoroutine == null){
            if(image.fillAmount>= threshold){
                sendCoroutine = StartCoroutine(sendPowerEnum());
            }
        }
    }
}
