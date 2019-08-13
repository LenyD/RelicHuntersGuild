using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Node : MonoBehaviour
{
    public Stats _s;
    public Animator anim;
    public Image icon;
    public Sprite[] popUpSprites;
    [HideInInspector] public Monster _m;
    [HideInInspector] public Image image;
    [HideInInspector] public float threshold = 1f, thresholdMargin = 0.02f, startFill = 0, targetFill = 0, resizeSpeed = 3f, cooldownLength =3, cooldown = 0f, delayToSend = 0;
    [HideInInspector] public float income = 0.05f;//getStats income
    [HideInInspector] public int effect = 0, power=1, powerDivider = 5;
    [HideInInspector] public bool isInCombat = true, isPlayerPopUp = true;
    [HideInInspector] public Color offCDColor = new Color(1f,1f,1f,1f), onCDColor = new Color(.35f,.35f,.35f,1f), isReadyColor = new Color(1f,.5f,.5f,1f);
    [HideInInspector] public BattleController _bc;
    [HideInInspector] public Coroutine sendCoroutine;

    // Start is called before the first frame update

    void Start()
    {
        image = GetComponent<Image>();
    }
    void Update()
    {
        reduceCooldown();
    }
    public void init(BattleController battleC,Monster m,int p,int chances){
        anim.gameObject.SetActive(false);
        _bc = battleC;
        _m = m;
        enabled = true;
        targetFill = startFill;
        image.fillAmount = startFill;
        power = p;
        icon.color = onCDColor;
        if(Random.Range(chances,chances+100)>100){
            increaseSize(100f);
        }
    }

    public void startCombat(){
        isInCombat = true;
        //cr = StartCoroutine(incomeTimer());
    }
    public void endCombat(){
        enabled = false;
        /*
         if(cr!=null){
            StopCoroutine(cr);
        }
        */
        isInCombat = false;
    }

    // Update is called once per frame
    public void reduceCooldown(){
        cooldown-=Time.deltaTime;
        if(cooldown>0){
            icon.color = onCDColor;
        }else{
            if(image.fillAmount < threshold || targetFill < threshold){
                icon.color = offCDColor;
                if(image.fillAmount+thresholdMargin >= threshold){
                    image.fillAmount = threshold;
                }
            }else{
                icon.color = isReadyColor;
                targetFill = threshold;
            }
        }
        image.fillAmount = Mathf.Lerp(image.fillAmount,targetFill,resizeSpeed * Time.deltaTime);
        
        
    }

    public bool increaseSize(float increment){
        if(cooldown<=0){
            if(targetFill>= threshold){
                sendPower();
            }else{
                targetFill += 1*increment*power/powerDivider;
                Sprite[] sprites = new Sprite[]{popUpSprites[0],popUpSprites[3]};
                _bc.createPopUp(sprites,isPlayerPopUp);
            }
            return true;
        }
        return false;
    }
    public void reduceSize(float decrement, float divider){
        targetFill = Mathf.Max(targetFill - (1 * decrement/divider),0);
        Sprite[] sprites = new Sprite[]{popUpSprites[1],popUpSprites[3]};
        _bc.createPopUp(sprites,isPlayerPopUp);
    }

    public void sendPower(){
        if(cooldown<=0){
            if(image.fillAmount >= threshold){
                sendCoroutine = StartCoroutine(sendPowerEnum());
            }
        }
    }
    public virtual void applyEffect(){

    }
    public IEnumerator sendPowerEnum(){
        yield return new WaitForSeconds(delayToSend);
        if(cooldown<0){
            reset();
            anim.gameObject.SetActive(true);
            anim.Play("sendPower",0);
            applyEffect();
            yield return new WaitForSeconds(0.9f);
        }
        sendCoroutine = null;
    }

    public void reset(){
        resetCooldown(cooldownLength);
        targetFill = startFill;
    }
    public void resetCooldown(float length){
        cooldown = length;
    }

    IEnumerator incomeTimer(){
        while(isInCombat){
            yield return new WaitForSeconds(1);
            increaseSize(income);
        }
    }
}
