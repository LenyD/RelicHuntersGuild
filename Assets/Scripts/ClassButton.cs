using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassButton : MonoBehaviour
{
    public Vector2 startPosition;
    // Start is called before the first frame update
    public Text[] texts;
    public Image img;
    bool isUnlocked = true;
    int id;
    ClassMenuButton classMenu;
    PartyEditor _pe;
    public Button bt;
    public Image lockImg;

    public void addClassMenuListener(ClassMenuButton cmb){
        classMenu=cmb;
        setLocked(!InterScene.saveFile.charactersUnlock[id]);
        bt.onClick.AddListener(OnClickClassMenu);
    }
    public void addPartyEditorListener(PartyEditor editor){
        _pe=editor;
        bt.onClick.AddListener(OnClickPartyEditor);
    }

    void OnClickClassMenu(){
        if(!isUnlocked){
            return;
        }
        classMenu.activateButton(id);
    }
    void OnClickPartyEditor(){
        if(!isUnlocked){
            return;
        }
        _pe.activateButton(id);
    }

    public void setSprite(string spriteName){
        Sprite characterSprite = Resources.Load<Sprite>("CharacterSprites/"+spriteName);
        if(characterSprite == null){
            characterSprite = Resources.Load<Sprite>("CharacterSprites/Default");
        }
        img.sprite = characterSprite;
    }
    public Text[] getTexts(){
        return texts;
    }
    public void setLocked(bool isLocked){
        if(isLocked){
            lockImg.enabled=true;
            bt.interactable = false;
        }
        isUnlocked = !isLocked;
    }
    public void setId(int i,Vector2 sPos){
        id = i;
        startPosition = sPos;
    }
    
}

