using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    Canvas canvas;
    GameMaster _GM;
    public ItemButton[] itemButtons;


    // Start is called before the first frame update
    void Start()
    {
        _GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        canvas = GetComponent<Canvas>();
    }
   
    public void openShop(Merchant m){
        canvas.enabled = true;
        this.gameObject.SetActive(true);
        setItemButtons(m);
    }
    void setItemButtons(Merchant m){
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].setItem(m,i);
        }
    }
    public void closeShop(){
        canvas.enabled = false;
        this.gameObject.SetActive(false);
        _GM.closeShop();
    }
    public void buy(int itemId){
        itemButtons[itemId].reduceStock();
    }

    void soldOut(){

    }
}
