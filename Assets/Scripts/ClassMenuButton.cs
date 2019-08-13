using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClassMenuButton : MonoBehaviour
{
    GameObject[] buttons;
    public GameObject startButton;
    public RectTransform[] selectedPosition;
    public SoundEffect SE_click;
    public SoundEffect SE_badClick;
    
    public GameObject classButtonPrefab;

    int numberOfButtons = 0;
    float margin = 10;
    float buttonWidth;
    float buttonHeight;
    int numberOfCol = 7;

    int maxNumberOfMember = 3;
    int[] party;
    void Awake(){
        buttonWidth = classButtonPrefab.GetComponent<RectTransform>().rect.width;
        buttonHeight = classButtonPrefab.GetComponent<RectTransform>().rect.height;
		InterScene.jsonDataString = InterScene.fetchJsonData("data.json");
		InterScene.jsonSaveFileString= InterScene.fetchJsonData("savefile.json");
		InterScene.parseJsonString();
        numberOfButtons = InterScene.jsonData.explorers.Length;
        InterScene.resetSingleRunAchievementsProgress();
    }

    void Start(){
        party = new int[maxNumberOfMember];
        for (int i = 0; i < party.Length; i++)
        {
            party[i]=-1;
        }
        //fill button text
        buttons = new GameObject[numberOfButtons];
        float f = (float)numberOfButtons/(float)numberOfCol;
        int btId;
        for (int i = 0; i < Mathf.CeilToInt(f); i++)
        {
            for (int j = 0; j < numberOfCol; j++)
            {
                btId = i*numberOfCol+j;
                if(i*numberOfCol+j<numberOfButtons){
                    buttons[btId] = Instantiate(classButtonPrefab,new Vector3(0,0,0),Quaternion.identity,this.transform);
                    Vector2 pos = new Vector2(0+((buttonWidth+margin)*j),0+(((buttonHeight+margin)*i*-1)));
                    buttons[btId].GetComponent<RectTransform>().anchoredPosition = pos;
                    buttons[btId].GetComponent<ClassButton>().setId(btId,pos);
                    buttons[btId].GetComponent<ClassButton>().addClassMenuListener(this);
                    
                }
            }
        }
        fillStatsText();
    }

    

    void fillStatsText(){
    //Go through every button and fill the text
        for (int i = 0; i < buttons.Length; i++)
        {
            Text[] t = buttons[i].GetComponent<ClassButton>().getTexts();
            buttons[i].GetComponent<ClassButton>().setSprite(InterScene.jsonData.explorers[i].name);
            t[0].text = InterScene.jsonData.explorers[i].name;
            t[1].text = InterScene.jsonData.explorers[i].health.ToString();
            t[2].text = InterScene.jsonData.explorers[i].block.ToString();
            t[3].text = InterScene.jsonData.explorers[i].strength.ToString();
            t[4].text = InterScene.jsonData.explorers[i].evade.ToString();
            t[5].text = InterScene.jsonData.explorers[i].accuracy.ToString();
            t[6].text = InterScene.jsonData.explorers[i].effect;           

        }
    }
    
    public void activateButton(int id){
        //On click add id to party if party is full start the game and send party data
        for (int i=0; i < party.Length; i++)
        {
            //Remove from selected
            if(party[i] == id){
                changeButtonPosition(buttons[id],buttons[id].GetComponent<ClassButton>().startPosition);
                party[i]=-1;
                SE_click.playSound();
                hideStartBT();
                return;
            }
        }

        for (int i=0; i < party.Length; i++)
        {
            //Add to selected
            if(party[i] == -1){
                changeButtonPosition(buttons[id],selectedPosition[i].anchoredPosition);
                party[i]=id;
                SE_click.playSound();
                break;
            }
            if(i== party.Length-1){
                SE_badClick.playSound();
            }
        }
        bool partyHasEmptySlots=false;
        for (int i=0; i < party.Length; i++)
        {
            //Check if full
            if(party[i] == -1){
                partyHasEmptySlots = true;
                break;
            }
        }

        //Hide / show start button
        if(partyHasEmptySlots){
            hideStartBT();
        }else{
            string[] p = new string[maxNumberOfMember];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = InterScene.jsonData.explorers[party[i]].name;
            }
            InterScene.party = party;
            showStartBT();
        }
    }

    void changeButtonPosition(GameObject b,Vector2 t){
        b.GetComponent<RectTransform>().anchoredPosition = t;
    }

    void showStartBT(){
        startButton.SetActive(true);
    }
    void hideStartBT(){
        startButton.SetActive(false);
    }

    public void startGame(){
        //change the scene.
        SceneManager.LoadScene("Scene", LoadSceneMode.Single);
    }
    public void closeMenu(){
        StartCoroutine("closeClassMenu");
    }
    IEnumerator closeClassMenu(){
        SE_click.playSound();
        yield return new WaitForSeconds(0.1f);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("ClassMenu"));
    }
}
