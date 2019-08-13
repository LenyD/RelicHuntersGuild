using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyNode : Node
{
    float defaultCooldownLength =3;
    // Start is called before the first frame update
    void Start()
    {
        powerDivider = 10;
        delayToSend = 1f;
        image = GetComponent<Image>();
        startFill = 0;
        targetFill = 0;
        isPlayerPopUp = false;
    }
    public void init(BattleController battleC){
        anim.gameObject.SetActive(false);
        _bc = battleC;
        targetFill = startFill;
        image.fillAmount = targetFill;
        icon.color = onCDColor;
    }
    public void setMonster(Monster monster){
        _m = monster;
    }
    public void setCooldown(float multiplier){
        cooldown = cooldownLength*multiplier;
    }
    public void setCooldownLength(float multiplier){
        cooldownLength = defaultCooldownLength*multiplier;
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
