using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Merchant : MonoBehaviour
{
    
    int inventorySize = 4; 
    Item[] inventory;
    bool[] isInStock;
    int maxLootTableId=0;
    int tier = InterScene.currentTier;
    bool isRelic = false;

    // Start is called before the first frame update
    
    void Awake()
    {
       
    }
    public void generateInventory(){
        //Fetch 4 different items from interscene table and save them
        inventory = new Item[inventorySize];
        isInStock = new bool[inventorySize];
        for (int i = 0; i < isInStock.Length; i++)
        {
            setInStock(i,true);
        }
        List<int> itemId = new List<int>();
        for (int i = 0; i < inventorySize; i++)
        {
            int rng = fetchNewRandomItemId(itemId);
            itemId.Add(rng);
            if(isRelic){
                inventory[i] = InterScene.jsonData.relics[itemId[i]];
            }else{
                inventory[i] = InterScene.jsonData.items[itemId[i]];
            }
        }
    }
    int fetchNewRandomItemId(List<int> idList){
        //Fetch 1 random item from the loot table
        //If it's already in the shop, reroll
        int tableId = Random.Range(0,maxLootTableId);
        int id = InterScene.jsonData.lootTables[tier].table[tableId];
        if(isRelic){
            for (int i = 0; i < idList.Count; i++)
            {
                if(tableId == idList[i]){
                    return fetchNewRandomItemId(idList);
                }
            }
            return tableId;
        }else{
            for (int i = 0; i < idList.Count; i++)
            {
                if(id == idList[i]){
                    return fetchNewRandomItemId(idList);
                }
            }
            return id;
        }
    }
    public void setMaxLootTableId(int id){
        maxLootTableId = id;
    }
    public void setIsRelic(bool b){
        isRelic = b;
    }
    public bool getIsRelic(){
        return isRelic;
    }
    public Item[] getInventory(){
        return inventory;
    }
    public bool getInStock(int id){
        return isInStock[id];
    }
    public void setInStock(int id,bool value){
        isInStock[id] = value;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
