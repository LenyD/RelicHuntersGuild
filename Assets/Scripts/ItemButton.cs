using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton:MonoBehaviour
{   
    public Text nameTxt;
    public Text effectTxt;
    public Text costTxt;
    public GameObject goldImg;
    public GameObject relicImg;
    public Text soldOutTxt;
    public Image image;    int cost;
    public SoundEffect SE_coinTransfer;
    public SoundEffect SE_coinMissing;
    string effectName;
    Stats _s;
    Merchant currentMerchant;
    int id;

    void Start(){
        _s = GameObject.Find("Party stats").GetComponent<Stats>();
    }

    public void setItem(Merchant m,int arrayPos){
        //Set up item from merchant info
        id = arrayPos;
        currentMerchant = m;

        Item values = currentMerchant.getInventory()[id];
        nameTxt.text = values.name;
        effectTxt.text = values.effect;
        effectName = values.applyEffectFunction;
        cost = values.cost;
        costTxt.text = cost.ToString();
        image.sprite = Resources.Load<Sprite>("ItemSprite/"+values.sprite);
        setSoldOut();
    }

    public void reduceStock(){
        //Reduce the stock of the item
        if(currentMerchant.getIsRelic()){
            //If it's a relic shop, consume relics
            if(InterScene.saveFile.stats.artifact>=cost){
                if(currentMerchant.getInStock(id)){
                    currentMerchant.setInStock(id,false);
                    SE_coinTransfer.playSound();
                    InterScene.saveFile.stats.artifact -= cost;
                    InterScene.saveToFile();
                    _s.SendMessage(effectName);
                }
            }else{
                SE_coinMissing.playSound();
            }
        }else{
            //Consume gold
            if(_s.getGold()>=cost){
                if(currentMerchant.getInStock(id)){
                    currentMerchant.setInStock(id,false);
                    SE_coinTransfer.playSound();
                    if(_s.thiefRoll()){
                    }else{
                        _s.reduceGold(cost);
                    }
                    _s.SendMessage(effectName);
                }
            }else{
                SE_coinMissing.playSound();
            }
        }
        setSoldOut();
    }

    void setSoldOut(){
        soldOutTxt.enabled = !currentMerchant.getInStock(id);
        costTxt.enabled = currentMerchant.getInStock(id);
        goldImg.SetActive(currentMerchant.getInStock(id)&&!currentMerchant.getIsRelic());
        relicImg.SetActive(currentMerchant.getInStock(id)&&currentMerchant.getIsRelic());

    }

}
