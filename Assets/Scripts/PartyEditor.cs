using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyEditor : MonoBehaviour
{

    int numberOfMemberToGenerate = 3;
    int numberOfPartyMemberMax = 4;
    int[] partySpaces;
    int[] holdingSpaces;
    public GameObject[] buttons;
    GameMaster _GM;
    Player _player;
    public Stats _s;
    public SoundEffect SE_click;
    public SoundEffect SE_badClick;

    void Awake(){
        _GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].GetComponent<ClassButton>().setId(i,buttons[i].transform.position);
            buttons[i].GetComponent<ClassButton>().addPartyEditorListener(this);
        }
    }

    public void setUpCanvas(){
        //Set up on event
        this.GetComponent<Canvas>().enabled=true;
        gameObject.SetActive(true);
        generateCharacters();
        refreshButtonsStats();
    }
    void refreshButtonsStats(){
        //set Buttons stats and active by following buttons[] array
        for (int i = 0; i < partySpaces.Length; i++)
        {
            fillButtonStats(partySpaces[i],i);
        }
        for (int i = 0; i < holdingSpaces.Length; i++)
        {
            fillButtonStats(holdingSpaces[i],i+4);
        }
    }

    void fillButtonStats(int id,int btId){
        
        if(id<0){
            buttons[btId].SetActive(false);
        }else{
            buttons[btId].SetActive(true);
            Text[] t = buttons[btId].GetComponent<ClassButton>().getTexts();
            buttons[btId].GetComponent<ClassButton>().setSprite(InterScene.jsonData.explorers[id].name);
            t[0].text = InterScene.jsonData.explorers[id].name;
            t[1].text = InterScene.jsonData.explorers[id].health.ToString();
            t[2].text = InterScene.jsonData.explorers[id].block.ToString();
            t[3].text = InterScene.jsonData.explorers[id].strength.ToString();
            t[4].text = InterScene.jsonData.explorers[id].evade.ToString();
            t[5].text = InterScene.jsonData.explorers[id].accuracy.ToString();
            t[6].text = InterScene.jsonData.explorers[id].effect;
        }
    }

    void generateCharacters(){
        //num
        holdingSpaces = new int[numberOfMemberToGenerate+1];
        partySpaces = new int[numberOfPartyMemberMax];
        //set holding space if party already has 4 character
        for (int i = 0; i < partySpaces.Length; i++)
        {
            partySpaces[i]=-1;
        }
        for (int i = 0; i < holdingSpaces.Length; i++)
        {
            holdingSpaces[i]=-1;
        }
        for (int i = 0; i < numberOfMemberToGenerate; i++)
        {
            holdingSpaces[i] = fetchRandomCharacter();
        }
        for (int i = 0; i < InterScene.party.Length; i++)
        {
            partySpaces[i]=InterScene.party[i];
        }
    }

    int fetchRandomCharacter(){
        //Get a random character not already generated or return empty id(-1)
        int numberOfCharMax = InterScene.party.Length *-1;
        for (int i = 0; i < InterScene.saveFile.charactersUnlock.Length; i++)
        {
            if(InterScene.saveFile.charactersUnlock[i]){
                numberOfCharMax++;
            }
        }
        for (int i = 0; i < numberOfMemberToGenerate; i++)
        {
            if(holdingSpaces[i]==-1){
                numberOfCharMax++;
            }
        }
        if(numberOfMemberToGenerate <= numberOfCharMax){
            int rngId = generateNewRandomCharacter();
            return rngId;
        }
        return -1;
    }

    int generateNewRandomCharacter(){
        int rngId = Random.Range(0,InterScene.jsonData.explorers.Length);
        int returnValue = rngId;
        if(!InterScene.saveFile.charactersUnlock[rngId]){
            returnValue = generateNewRandomCharacter();
        }
        for (int i = 0; i < InterScene.party.Length; i++)
        {
            if(InterScene.party[i] == rngId){
                returnValue = generateNewRandomCharacter();
                break;
            }
        }
        for (int i = 0; i < holdingSpaces.Length; i++)
        {
            if(holdingSpaces[i] == rngId){
                returnValue = generateNewRandomCharacter();
                break;
            }
        }
        return returnValue;
    }
    public void activateButton(int id){
        //OnClick event method
        if(id>3){
            for (int i = 0; i < partySpaces.Length; i++)
            {
                if(partySpaces[i]==-1){
                    partySpaces[i] = holdingSpaces[id-4];
                    holdingSpaces[id-4] = -1;
                    SE_click.playSound();
                    refreshButtonsStats();
                    return;
                }
            }
        }else{
            for (int i = 0; i < holdingSpaces.Length; i++)
            {
                if(holdingSpaces[i]==-1){
                    holdingSpaces[i] = partySpaces[id];
                    partySpaces[id] = -1;
                    SE_click.playSound();
                    refreshButtonsStats();
                    return;
                }
            }
        }
        SE_badClick.playSound();
    }
    public void close(){
        List<int> newCharacters = new List<int>();
        List<int> removedCharacters = new List<int>();
        List<int> newParty = new List<int>();
        //Set new current party
        //Get removed character
        for (int i = 0; i < InterScene.party.Length; i++)
        {
            for (int j = 0; j < partySpaces.Length; j++)
            {
                if(partySpaces[j]==InterScene.party[i]){
                    break;
                }else{
                    if(j==partySpaces.Length-1){
                        removedCharacters.Add(InterScene.party[i]);
                    }
                }
            }
        }
        for (int i = 0; i < partySpaces.Length; i++)
        {
            if(partySpaces[i]!=-1){
                for (int j = 0; j < InterScene.party.Length; j++)
                {
                    if(partySpaces[i]==InterScene.party[j]){
                        break;
                    }else{
                        if(j==InterScene.party.Length-1){
                            newCharacters.Add(partySpaces[i]);
                        }
                    }
                }
                newParty.Add(partySpaces[i]);
            }
        }
        
        if(newParty.Count>0){
		    InterScene.setAchievementProgress(12,partySpaces.Length - newParty.Count);
            InterScene.party = newParty.ToArray();
            _player.setCharacterSprites(InterScene.party);
            //Remove effect of removed char & add effect of new char
            _s.fillStats(newCharacters.ToArray());
            _s.removeCharactersStats(removedCharacters.ToArray());
            _s.applyClassEffect(newCharacters.ToArray());
            _s.removeClassEffect(removedCharacters.ToArray());
            this.GetComponent<Canvas>().enabled=false;
            gameObject.SetActive(false);
            _GM.closeShop();
        }else{
            SE_badClick.playSound();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
