using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPanel : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Description;
    public TMP_Text UnlockDescription;
    public Toggle toggle;
    // Start is called before the first frame update
    public void setUp(Achievement data, bool isUnlocked){
        //Set up text
        //if unlocked, check toggle and strike text.

        toggle.isOn = isUnlocked;
        if(isUnlocked){
            Title.fontStyle = FontStyles.Strikethrough;
            Description.fontStyle = FontStyles.Strikethrough;
            UnlockDescription.fontStyle = FontStyles.Strikethrough;
        }
        Title.text = data.name;
        Description.text = data.description;
        UnlockDescription.text = data.unlockDescription+" ("+data.progress+" / "+data.goal+")";

    }
}
