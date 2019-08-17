using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyOffenceNode : EnemyNode
{
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        effect = 3;
        isPlayerPopUp = false;
        powerDivider = 10;
        delayToSend = 1f;
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
