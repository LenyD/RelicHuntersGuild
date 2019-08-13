using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


[System.Serializable]
public class JsonData
{
    public Item[] items;
    public Item[] relics;
    public LootTable[] lootTables;
    public Explorer[] explorers;
    public EnemiesTable[] EnemiesTables;
}
[System.Serializable]
public class Item
{
    public int id;
    public string name;
    public string effect;
    public string applyEffectFunction;
    public int cost;
    public string sprite;
}
[System.Serializable]
public class LootTable
{
    public int[] table;
}
[System.Serializable]
public class EnemiesTable
{
    public Enemy[] table;
}
[System.Serializable]
public class Explorer
{
    public string name;
    public int health;
    public int block;
    public int evade;
    public int strength;
    public int accuracy;
    public string effect;
    public string applyEffectFunction;
    public string removeEffectFunction;
}
[System.Serializable]
public class Enemy
{
    public int health;
    public int block;
    public int evade;
    public int strength;
    public int accuracy;
    public string image;
}
[System.Serializable]
public class SaveFile{
    public AccountStatistic stats;
    public bool[] charactersUnlock;
    public Achievements achievements;
}
[System.Serializable]
public class AccountStatistic{
    public int artifact;
}
[System.Serializable]
public class Achievements{
    public Achievement[] list;
    public bool[] achievementsUnlock;
}
[System.Serializable]
public class Achievement{
    public int id;
    public string name;
    public string description;
    public string unlockFunction;
    public string unlockDescription;
    public int progress;
    public int goal;
}
public static class InterScene
{
    public static string jsonDataString;
    public static string jsonSaveFileString;
    public static JsonData jsonData = new JsonData();
    public static SaveFile saveFile = new SaveFile();
    public static int currentTier {get;set;} = 0;
    public static float timer {get;set;} = 0;
    public static Unlocks unlocks= new Unlocks();

    public static int[] party {get;set;}
    public static string fetchJsonData(string fileName){
        return File.ReadAllText(Application.streamingAssetsPath + "/"+fileName);
    }
    public static void parseJsonString(){
        JsonUtility.FromJsonOverwrite(jsonDataString,jsonData);
        JsonUtility.FromJsonOverwrite(jsonSaveFileString,saveFile);
    }
    public static void saveToFile(){
        string dataToSave = JsonUtility.ToJson(saveFile);
        File.WriteAllText(Application.streamingAssetsPath + "/savefile.json",dataToSave);
    }
    public static void addAchievementProgress(int id, int progress){
        if(!saveFile.achievements.achievementsUnlock[id]){
            saveFile.achievements.list[id].progress+=progress;
            if(saveFile.achievements.list[id].progress>=saveFile.achievements.list[id].goal){
                saveFile.achievements.achievementsUnlock[id] = true;
                unlocks.GetType().GetMethod(saveFile.achievements.list[id].unlockFunction).Invoke(unlocks,new object[0]);
            }else{
                saveToFile();
            }
        }
    }
    public static void setAchievementProgress(int id, int progress){
        if(!saveFile.achievements.achievementsUnlock[id]){
            saveFile.achievements.list[id].progress=progress;
            if(saveFile.achievements.list[id].progress>=saveFile.achievements.list[id].goal){
                saveFile.achievements.achievementsUnlock[id] = true;
                unlocks.GetType().GetMethod(saveFile.achievements.list[id].unlockFunction).Invoke(unlocks,new object[0]);
            }else{
                saveToFile();
            }
        }
    }
    public static void resetAchievementProgress(int id){
        if(!saveFile.achievements.achievementsUnlock[id]){
            saveFile.achievements.list[id].progress=0;
            saveToFile();
        }
    }
    public static void resetSingleRunAchievementsProgress(){
        int[] idList = new int[]{0,2,3,4,5,6,10,12};
        for (int i = 0; i < idList.Length; i++)
        {
            resetAchievementProgress(idList[i]);
        }
    }
}

public class Unlocks{
    public void unlockCook(){
        unlock(7);
    }
    public void unlockSquire(){
        unlock(8);
    }
    public void unlockTracker(){
        unlock(9);
    }
    public void unlockTrapper(){
        unlock(10);
    }
    public void unlockMonk(){
        unlock(12);
    }
    public void unlockExorcist(){
        unlock(11);
    }
    public void unlockScholar(){
        unlock(13);
    }
    public void unlockNobleman(){
        unlock(14);
    }
    public void unlockJester(){
        unlock(15);
    }
    public void unlockBard(){
        unlock(16);
    }
    public void unlockCultist(){
        unlock(17);
    }
    public void unlockDemolitionist(){
        unlock(18);
    }
    void unlock(int id){
        if(!InterScene.saveFile.charactersUnlock[id]){
            InterScene.saveFile.charactersUnlock[id]=true;
            InterScene.saveToFile();
        }
    }

}
