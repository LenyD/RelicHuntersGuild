using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{

	Player _player;
    GameMaster _GM;
    BattleController _bc;
    public Text[] text;
    public Text curseText;
    public Text blessText;
    public Text meditateText;
    public Text goldText;
    public Text energyText;
    public Text bombText;
    int numberOfHiddenRoom;
    int resetRoomUntilHeal = 1000;
    int roomUntilHeal;
    bool isStepHealEnable = false;
    bool isIncomeEnable = false;
    float chanceToSteal = 0;
    int trainingPower = 2;
    bool partyHasCook = false;
    bool partyHasCultist = false;
    bool partyHasDemo = false;
    bool restIsImproved = false;
    bool ankhProtectionOn = false;
    int squirePower = 0;
    
    int[] stats;
    Dictionary<string,int> statsId = new Dictionary<string, int>();
    
	int curseCountdown = 0;
    int curseStrength = 0;
	int blessCountdown = 0;
    int blessStrength = 0;
    int[] buffCountdown = new int[4];
    int[] buffStrength = new int[4];
    int counterPerCurse = 2;
    int counterPerBlessing = 2;
    int numberOfMeditate = 3;
    int energy = 100;
    int gold = 0;
    int bombs = 3;
    int energyMax;
    int block;
    int dodge;
    int maxDodge = 90;
	public ParticleSystem meditateParticles;
    public Image lootPopUp;
    Animator lootpopUpAnim;
    public EndScreen _endScreen;
    public DiffcultyScaler _difficultyScaler;
    public GameObject popUpPrefab;
    public Sprite[] popUpSprites;
    /*
        0 plus
        1 minus
        2 hp
        3 str
        4 block
        5 evade
        6 acc
        7 bomb
        8 rest
        9 energy
    */
    List<int[]> popUpQueue = new List<int[]>();
    Coroutine popUpCoroutine;
    public SoundEffect SE_coinScramble;
    public SoundEffect SE_hurt;
    
    void Awake(){
		_GM = FindObjectOfType<GameMaster>();
        _player = FindObjectOfType<Player>();
        _bc=GameObject.Find("Battle Canvas").GetComponent<BattleController>();
        lootpopUpAnim = lootPopUp.GetComponent<Animator>();
        energyMax = energy;
        statsId.Add("hp",0);
        statsId.Add("hpMax",1);
        statsId.Add("dmg",2);
        statsId.Add("dmgMax",3);
        statsId.Add("def",4);
        statsId.Add("defMax",5);
        statsId.Add("spd",6);
        statsId.Add("spdMax",7);
        statsId.Add("acc",8);
        statsId.Add("accMax",9);
        stats = new int[10];
        applyClassEffect(InterScene.party);
    }
    void Start(){
        fillStats(InterScene.party);
        roomUntilHeal = resetRoomUntilHeal;
        setMeditateText();
        setGoldText();
        setBombText();
        _player.setCharacterSprites(InterScene.party);

        //numberOfHiddenRoom = _GM.getNumberOfTilePlaced();
    }

    public void fillStats(int[] party){
        for (int i = 0; i < party.Length; i++)
        {
            addStat("hp",       InterScene.jsonData.explorers[party[i]].health);
            addStat("hpMax",    InterScene.jsonData.explorers[party[i]].health);
            addStat("dmg",      InterScene.jsonData.explorers[party[i]].strength);
            addStat("dmgMax",   InterScene.jsonData.explorers[party[i]].strength);
            addStat("def",      InterScene.jsonData.explorers[party[i]].block);
            addStat("defMax",   InterScene.jsonData.explorers[party[i]].block);
            addStat("spd",      InterScene.jsonData.explorers[party[i]].evade);
            addStat("spdMax",   InterScene.jsonData.explorers[party[i]].evade);
            addStat("acc",      InterScene.jsonData.explorers[party[i]].accuracy);
            addStat("accMax",   InterScene.jsonData.explorers[party[i]].accuracy);
        }
    }

    public void removeCharactersStats(int[] party){
        for (int i = 0; i < party.Length; i++)
        {
            reduceStat("hp",       InterScene.jsonData.explorers[party[i]].health);
            reduceStat("hpMax",    InterScene.jsonData.explorers[party[i]].health);
            reduceStat("dmg",      InterScene.jsonData.explorers[party[i]].strength);
            reduceStat("dmgMax",   InterScene.jsonData.explorers[party[i]].strength);
            reduceStat("def",      InterScene.jsonData.explorers[party[i]].block);
            reduceStat("defMax",   InterScene.jsonData.explorers[party[i]].block);
            reduceStat("spd",      InterScene.jsonData.explorers[party[i]].evade);
            reduceStat("spdMax",   InterScene.jsonData.explorers[party[i]].evade);
            reduceStat("acc",      InterScene.jsonData.explorers[party[i]].accuracy);
            reduceStat("accMax",   InterScene.jsonData.explorers[party[i]].accuracy);
        }
    }
    public void addStat(string statName, int increment){
        stats[statsId[statName]] += increment;

        if(InterScene.saveFile.achievements.list[4].progress<stats[statsId[statName]]){
		    InterScene.setAchievementProgress(4,stats[statsId[statName]]);
        }

        setText(statsId[statName]);
    }
    public void reduceStat(string statName, int decrement){
        if(stats[statsId[statName]] - decrement >=0){
            stats[statsId[statName]] -= decrement;
        }else{
            stats[statsId[statName]] = 0;
        }
        setText(statsId[statName]);
    }
    public void reduceHp(int damage, bool isArmorPiercing = false){
        if(!isArmorPiercing){
            //damage -= stats[statsId["def"]];
        }
        stats[0] -= damage;
        if(stats[0]<=0){
            if(ankhProtectionOn){
                stats[1] = Mathf.FloorToInt(stats[1]/2);
                stats[0] = stats[1];
                setText(0);
                setText(1);
                ankhProtectionOn = false;
                return;
            }
            stats[0] = 0;
            setText(0);
            _player.setCanMove(false);
            _endScreen.endGame(true);
            return;
        }
        setText(0);
        
    }
    public void reduceBomb(){
        bombs --;
		InterScene.setAchievementProgress(6,bombs);
        if(bombs<0){
            bombs = 0;
        }
        setBombText();
    }
    public void healHp(int heal){
        stats[0] += heal;
        if(stats[0]>stats[1]){
            stats[0] = stats[1];
        }
        setText(0);
    }

    void setText(int id){
        if(text[id]!= null){
            text[id].text = stats[id].ToString();
        }
    }
    public void useAlchemyTable(){
        //Heal hp and buff for 2 event
        meditateParticles.Play();
        healHp(stats[1]);
        int buffPower = 2;
        int buffDuration = 2;
        for (int i = 0; i < buffCountdown.Length; i++)
        {
            buffCountdown[i]+=buffDuration;
            buffStrength[i]+=buffPower;

        }
        addStat("dmg",buffPower);
        addStat("def",buffPower);
        addStat("spd",buffPower);
        addStat("acc",buffPower);

    }
    public void camp(float restPower){
        meditate(restPower);
        //Counteract the -1 meditate in the meditate function
        addMeditate(1);
    }
    public void training(int id){
        string targetStatName;
        int energyCost = 25;
        switch(id){
            case 0:targetStatName = "dmg"; addToPopUpQueue(new int[]{0,3});break;
            case 1:targetStatName = "def"; addToPopUpQueue(new int[]{0,4});break;
            case 2:targetStatName = "spd"; addToPopUpQueue(new int[]{0,5});break;
            case 3:targetStatName = "acc"; addToPopUpQueue(new int[]{0,6});break;
            default:targetStatName = "hp"; addToPopUpQueue(new int[]{0,2});break;
        }
        addStat(targetStatName,trainingPower);
        addToPopUpQueue(new int[]{1,9});
        reduceEnergy(energyCost);
    }
    
    
    public void addCurseCounter(){
        blessCountdown =0;
        curseCountdown+= counterPerCurse;
        curseStrength++;
        resetStatsFromBless();
        if(partyHasCultist){
            addStat("def",1);
            addStat("dmg",1);
            addStat("spd",1);
            addStat("acc",1);
        }else{
            reduceStat("def",1);
            reduceStat("dmg",1);
            reduceStat("spd",1);
            reduceStat("acc",1);
        }
        curseText.text = curseCountdown.ToString();
    }
    public void addBlessCounter(){
        curseCountdown=0;
        blessCountdown+= counterPerBlessing;
        blessStrength++;
        resetStatsFromCurse();
         if(partyHasCultist){
            reduceStat("def",1);
            reduceStat("dmg",1);
            reduceStat("spd",1);
            reduceStat("acc",1);
        }else{
            addStat("def",1);
            addStat("dmg",1);
            addStat("spd",1);
            addStat("acc",1);
        }
        blessText.text = blessCountdown.ToString();
    }
    public void reduceCounter(){
        if(curseCountdown > 0){
            curseCountdown--;
        }else{
            curseCountdown = 0;
        }
        if(curseCountdown==0){
            resetStatsFromCurse();
        }
        if(blessCountdown > 0){
            blessCountdown--;
        }else{
            blessCountdown = 0;
        }
        if(blessCountdown==0){
            resetStatsFromBless();
        }
        for (int i = 0; i < buffCountdown.Length; i++)
        {
            if(buffCountdown[i] > 0){
                buffCountdown[i]--;
            }else{
                buffCountdown[i] = 0;
            }
            if(buffCountdown[i]==0){
                switch(i){
                    case 0:reduceStat("dmg",buffStrength[i]);break;
                    case 1:reduceStat("def",buffStrength[i]);break;
                    case 2:reduceStat("spd",buffStrength[i]);break;
                    case 3:reduceStat("acc",buffStrength[i]);break;
                }
                buffStrength[i]-=buffStrength[i];
            }
        }
        curseText.text = curseCountdown.ToString();
        blessText.text = blessCountdown.ToString();
    }
    void resetStatsFromCurse(){
        while (curseStrength>0)
        {
            addStat("def",1);
            addStat("dmg",1);
            addStat("spd",1);
            addStat("acc",1);
            curseStrength--;
        }
        curseText.text = curseCountdown.ToString();
    }
    void resetStatsFromBless(){
        while (blessStrength>0)
        {
            if(curseCountdown>0){
		        InterScene.addAchievementProgress(11,1);
            }
            reduceStat("def",1);
            reduceStat("dmg",1);
            reduceStat("spd",1);
            reduceStat("acc",1);
            blessStrength--;
        }
        blessText.text = blessCountdown.ToString();
    }

    void resetStatsToMax(){
        stats[2] = stats[3];
        stats[4] = stats[5];
        stats[6] = stats[7];
        stats[8] = stats[9];
        setText(2);
        setText(4);
        setText(6);
        setText(8);
    }

    public void reduceNumberOfHiddenRoom(){
        numberOfHiddenRoom--;
        roomUntilHeal--;
        clericPassiveHeal();
        
    }
    public void addIncome(){
        if(isIncomeEnable){
            giveGold(1,false);
            
        }
    }
    void clericPassiveHeal(){
        if(isStepHealEnable){
            if(roomUntilHeal <=0){
                healHp(1);
                roomUntilHeal = resetRoomUntilHeal;
            }
        }
    }
    public void receiveDamage(int damage,int piercingDamage){
        Sprite[] sprites;
        if(evadeCheck()){
            resetDodge();
            sprites = new Sprite[]{popUpSprites[1],popUpSprites[5]};
            _bc.createPopUp(sprites,false);
            return;
        }
        damage = Mathf.Max(0,damage - block);
        int finalDmg = damage+piercingDamage;
        reduceBlock(finalDmg);
        if(finalDmg>0 || piercingDamage>0){
            sprites = new Sprite[]{popUpSprites[1],popUpSprites[2]}; 
        }else{
            sprites = new Sprite[]{popUpSprites[1],popUpSprites[4]}; 
        }
        _bc.createPopUp(sprites,false);
        reduceHp(finalDmg,true);
        SE_hurt.playSound();
        if(getHp() <= 0 ){
			_bc.StartCoroutine("endCombat");
		}
    }
    bool evadeCheck(){
        return (100<Random.Range(0+dodge,100+dodge));
    }

    public int[] getCurrentStatProfile(){
        return new int[]{getAttack(),getBlock(),getSpeed(),getAccuracy()};
    }
    public int getStatById(int id){
        return stats[id];
    }
    public int getAttack(){
        return stats[statsId["dmg"]];
    }
    public int getHp(){
        return stats[statsId["hp"]];
    }
    public int getSpeed(){
        return stats[statsId["spd"]];
    }
    public int getBlock(){
        return stats[statsId["def"]];
    }
    public int getAccuracy(){
        return stats[statsId["acc"]];
    }
    public int getBombs(){
        return bombs;
    }
    public void giveGold(int number,bool showPopUp = true){
        gold += number;
        setGoldText();
		InterScene.setAchievementProgress(5,gold);

        if(showPopUp){
            addToPopUpQueue(new int[]{0,10});
            SE_coinScramble.playSound();
        }
    }
    public void giveBomb(int number){
        bombs+=number;
		InterScene.setAchievementProgress(6,bombs);
        setBombText();
    }
    public void reduceGold(int cost){
        gold -= cost;
		InterScene.setAchievementProgress(5,gold);
        addToPopUpQueue(new int[]{1,10});
        setGoldText();
    }
    void setGoldText(){
        goldText.text = gold.ToString();
    }
    public int getGold(){
        return gold;
    }
    public void setNumberOfHiddenRoom(int number){
        numberOfHiddenRoom = number;
    }
    void setStepHealCounter(int numberOfRoom,bool b = true){
        //heal 1 for every x new room discovered
        resetRoomUntilHeal = numberOfRoom;
        roomUntilHeal = numberOfRoom;
        isStepHealEnable = b;
    }
    public void createItem(){
        int tier = InterScene.currentTier+squirePower;
        int itemId = InterScene.jsonData.lootTables[tier].table[Random.Range(0, InterScene.jsonData.lootTables[tier].table.Length)];
        Item currentItem = InterScene.jsonData.items[itemId];
        lootPopUp.sprite = Resources.Load<Sprite>( "ItemSprite/"+currentItem.sprite);
        lootPopUp.gameObject.SetActive(true);
        lootpopUpAnim.Play("",0);
        StartCoroutine(applyItemEffect(currentItem.applyEffectFunction));
    }
    IEnumerator applyItemEffect(string functionName){
        yield return new WaitForSeconds(0.5f);
        SendMessage(functionName);
    }

    void meditate(float power=1){
		if(numberOfMeditate>0){
            reduceMeditate(1);
            healHp(Mathf.RoundToInt(stats[1]*power));
            healFatigue(Mathf.RoundToInt(energyMax*power));
            curseCountdown = 0;
            blessCountdown = 0;
            reduceCounter();
            if(power>=1){
		        InterScene.addAchievementProgress(10,1);
                if(partyHasDemo){
                        giveBomb(3);
                        addToPopUpQueue(new int[]{0,7});
                        addToPopUpQueue(new int[]{0,9});
                }
                if(restIsImproved){
                    buffCountdown[0]+=3;
                    buffStrength[0]+=2;
                    buffCountdown[1]+=3;
                    buffStrength[1]+=2;
                    buffCountdown[2]+=3;
                    buffStrength[2]+=2;
                    buffCountdown[3]+=3;
                    buffStrength[3]+=2;
                    addStat("dmg",2); 
                    addStat("def",2); 
                    addStat("spd",2); 
                    addStat("acc",2); 
                    addToPopUpQueue(new int[]{0,3});
                    addToPopUpQueue(new int[]{0,4});
                    addToPopUpQueue(new int[]{0,5});
                    addToPopUpQueue(new int[]{0,6});
                }
            }
            meditateParticles.Play();
		}
	}
    void addMeditate(int increment){
        numberOfMeditate += increment;
        setMeditateText();
    }
    void reduceMeditate(int decrement){
        numberOfMeditate -= decrement;
        if(numberOfMeditate<0){
            numberOfMeditate = 0;
        }
        setMeditateText();
    }
    void setMeditateText(){
        meditateText.text = numberOfMeditate.ToString();
    }
    void healFatigue(int nb){
        energy += nb;
        if(energy>energyMax){
            energy = energyMax;
        }
        addToPopUpQueue(new int[]{0,9});
        setEnergyText();
    }
    public void reduceEnergy(int cost){
        energy-=cost;
        if(energy%10==0){
            addToPopUpQueue(new int[]{1,9});
            if(energy<0){
                stats[1]--;
                if(stats[1]<stats[0]){
                    stats[0]=stats[1];
                    reduceHp(0);
                    addToPopUpQueue(new int[]{1,2});
                }
                setText(1);
            }
        }
        setEnergyText();
    }
    public bool getpartyHasCook(){
        return partyHasCook;
    }
    public void cookEffect(){
        healFatigue(10);    
        healHp(1);    

    }
    void setEnergyText(){
        energyText.text = energy.ToString();
    }
    void setBombText(){
        bombText.text = bombs.ToString();
    }
    public bool thiefRoll(){
        int rng = Random.Range(1,101);
        return rng<chanceToSteal;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Rest")){
            if(!_player.getIsLockedToRoom()){
                meditate();
            }
        }
    }

    //Classes effect
    public void applyClassEffect(int[] group){
        for (int i = 0; i < group.Length; i++)
        {
            this.SendMessage(InterScene.jsonData.explorers[group[i]].applyEffectFunction);
        }
    }
    public void removeClassEffect(int[] group){
        for (int i = 0; i < group.Length; i++)
        {
            this.SendMessage(InterScene.jsonData.explorers[group[i]].removeEffectFunction);
        }
    }
    void addToPopUpQueue(int[] ids){
        popUpQueue.Add(ids);
        startPopUpCoroutine();
    }
    void startPopUpCoroutine(){
        if(popUpCoroutine==null){
            popUpCoroutine = StartCoroutine(emptyPopUpQueue());
        }
    }
    IEnumerator emptyPopUpQueue(){
        while (popUpQueue.Count>0)
        {
            createPopUp(popUpQueue[0]);
            popUpQueue.RemoveAt(0);
            yield return new WaitForSeconds(0.3f);
        }
        popUpCoroutine = null;
    }
    void createPopUp(int[] imagesId){
        Sprite[] images = new Sprite[imagesId.Length];
        for (int i = 0; i < imagesId.Length; i++)
        {
            images[i]=popUpSprites[imagesId[i]];
        }
        GameObject bpopup = Instantiate(popUpPrefab,new Vector3(),Quaternion.identity,this.transform);
        Vector2 v2 = new Vector2();
        bpopup.GetComponent<BattlePopUp>().setUp(images,v2);
    }
    public void addBlock(int increment){
        block += increment;
        _bc.updateBlockText(block);
    }
    public void reduceBlock(int decrement){
        block = Mathf.Max(0,block-decrement);
        _bc.updateBlockText(block);

    }
    public void resetBlock(){
        block = 0;
        _bc.updateBlockText(block);
    }
    public void addDodge(int increment){
        dodge = Mathf.Min(maxDodge, dodge+increment);
        _bc.updateDodgeText(dodge);
    }
    public void resetDodge(){
        dodge = 0;
        _bc.updateDodgeText(dodge);
    }


    void addMercenary(){

    }
    void removeMercenary(){

    }
    void addCartographer(){
        _player.addDiscoveryRange(2);
    }
    void removeCartographer(){
        _player.addDiscoveryRange(-2);
    }
    void addTrapper(){
        _GM.setSpoiler(1);
    }
    void removeTrapper(){
        _GM.setSpoiler(1,false);
    }
    void addCleric(){
        setStepHealCounter(20);
    }
    void removeCleric(){
        setStepHealCounter(1000,false);
    }
    void addTracker(){
        _GM.setSpoiler(2);
    }
    void removeTracker(){
        _GM.setSpoiler(2,false);
    }
    void addExorcist(){
        _GM.setSpoiler(3); 
        counterPerCurse--;
    }
    void removeExorcist(){
        _GM.setSpoiler(3,false); 
        counterPerCurse++;
    }
    void addArbalest(){

    }
    void removeArbalest(){

    }
    void addOracle(){
        _player.setCheckAroundForEvent();
    }
    void removeOracle(){
        _player.setCheckAroundForEvent(false);
    }
    void addCook(){
        partyHasCook = true;        
    }
    void removeCook(){
        partyHasCook = false;
    }
    void addSquire(){
        squirePower = 1;
    }
    void removeSquire(){
        squirePower = 0;
    }
    void addMonk(){
        restIsImproved = true;
    }
    void removeMonk(){
        restIsImproved = false;        
    }
    void addSpearman(){
        _bc.setSpearmanPower(1);
    }
    void removeSpearman(){
        _bc.setSpearmanPower(0);        
    }
    void addScholar(){
        trainingPower+=2;
    }
    void removeScholar(){
        trainingPower-=2;        
    }
    void addThief(){
        chanceToSteal = 30;
    }
    void removeThief(){
        chanceToSteal = 0;        
    }
    void addNobleman(){
        isIncomeEnable=true;
        giveGold(50);
    }
    void removeNobleman(){
        isIncomeEnable=false;        
    }
    void addJester(){
        _bc.setMetronomeDamageScale(1.1f);
    }
    void removeJester(){
        _bc.setMetronomeDamageScale(1f);

    }
    void addBard(){
        _bc.setMetronomeSpeed(1.25f);
    }
    void removeBard(){
        _bc.setMetronomeSpeed(1.5f);
        
    }
    void addCultist(){
        partyHasCultist=true;
    }
    void removeCultist(){
        partyHasCultist=false;
    }
    void addDemolitionist(){
        giveBomb(3);
        partyHasDemo = true;     
    }
    void removeDemolitionist(){
        partyHasDemo = false;             
    }

    //Items effect
    void potion5(){
        healHp(5);
        addToPopUpQueue(new int[]{0,2});
    }
    void potion10(){
        healHp(10);
        addToPopUpQueue(new int[]{0,2});
    }
    void potion15(){
        healHp(15);
        addToPopUpQueue(new int[]{0,2});
    }
    void dagger1(){
        addStat("dmg",1);
        addStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,3});
    }
    void dagger2(){
        addStat("dmg",2);
        addStat("dmgMax",2);
        addToPopUpQueue(new int[]{0,3});
    }
    void dagger3(){
        addStat("dmg",3);
        addStat("dmgMax",3);
        addToPopUpQueue(new int[]{0,3});
    }
    void qualityArrows1(){
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,6});
    }
    void qualityArrows2(){
        addStat("acc",2);
        addStat("accMax",2);
        addToPopUpQueue(new int[]{0,6});
    }
    void qualityArrows3(){
        addStat("acc",3);
        addStat("accMax",3);
        addToPopUpQueue(new int[]{0,6});
    }
    void boots1(){
        addStat("spd",1);
        addStat("spdMax",1);
        addToPopUpQueue(new int[]{0,5});
    }
    void boots2(){
        addStat("spd",2);
        addStat("spdMax",2);
        addToPopUpQueue(new int[]{0,5});
    }
    void boots3(){
        addStat("spd",3);
        addStat("spdMax",3);
        addToPopUpQueue(new int[]{0,5});
    }
    void shield1(){
        addStat("def",1);
        addStat("defMax",1);
        addToPopUpQueue(new int[]{0,4});
    }
    void shield2(){
        addStat("def",2);
        addStat("defMax",2);
        addToPopUpQueue(new int[]{0,4});
    }
    void shield3(){
        addStat("def",3);
        addStat("defMax",3);
        addToPopUpQueue(new int[]{0,4});
    }
    void firewood(){
        addMeditate(1);
        addToPopUpQueue(new int[]{0,8});
    }
    void firewood2(){
        addMeditate(2);
        addToPopUpQueue(new int[]{0,8});
    }
    void hvyPlateArmor1(){
        addStat("def",3);
        addStat("defMax",3);
        reduceStat("spd",1);
        reduceStat("spdMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{1,5});
    }
    void hvyPlateArmor2(){
        addStat("def",5);
        addStat("defMax",5);
        reduceStat("spd",2);
        reduceStat("spdMax",2);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{1,5});
    }
    void thickPlateArmor1(){
        addStat("def",3);
        addStat("defMax",3);
        reduceStat("dmg",1);
        reduceStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{1,3});
    }
    void thickPlateArmor2(){
        addStat("def",5);
        addStat("defMax",5);
        reduceStat("dmg",2);
        reduceStat("dmgMax",2);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{1,3});
    }
    void helm1(){
        addStat("def",3);
        addStat("defMax",3);
        reduceStat("acc",1);
        reduceStat("accMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{1,6});
    }
    void helm2(){
        addStat("def",5);
        addStat("defMax",5);
        reduceStat("acc",2);
        reduceStat("accMax",2);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{1,6});
    }
    void leatherBracers1(){
        addStat("spd",3);
        addStat("spdMax",3);
        reduceStat("dmg",1);
        reduceStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{1,3});
    }
    void leatherBracers2(){
        addStat("spd",5);
        addStat("spdMax",5);
        reduceStat("dmg",2);
        reduceStat("dmgMax",2);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{1,3});
    }
    void eyePatch1(){
        addStat("spd",3);
        addStat("spdMax",3);
        reduceStat("acc",1);
        reduceStat("accMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{1,6});
    }
    void eyePatch2(){
        addStat("spd",5);
        addStat("spdMax",5);
        reduceStat("acc",2);
        reduceStat("accMax",2);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{1,6});
    }
    void evasionCloak1(){
        addStat("spd",3);
        addStat("spdMax",3);
        reduceStat("def",1);
        reduceStat("defMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{1,4});
    }
    void evasionCloak2(){
        addStat("spd",5);
        addStat("spdMax",5);
        reduceStat("def",2);
        reduceStat("defMax",2);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{1,4});
    }
    void greatsword1(){
        addStat("dmg",3);
        addStat("dmgMax",3);
        reduceStat("spd",1);
        reduceStat("spdMax",1);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{1,5});
    }
    void greatsword2(){
        addStat("dmg",5);
        addStat("dmgMax",5);
        reduceStat("spd",2);
        reduceStat("spdMax",2);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{1,5});
    }
    void flail1(){
        addStat("dmg",3);
        addStat("dmgMax",3);
        reduceStat("acc",1);
        reduceStat("accMax",1);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{1,6});
    }
    void flail2(){
        addStat("dmg",5);
        addStat("dmgMax",5);
        reduceStat("acc",2);
        reduceStat("accMax",2);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{1,6});
    }
    void halberd1(){
        addStat("dmg",3);
        addStat("dmgMax",3);
        reduceStat("def",1);
        reduceStat("defMax",1);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{1,4});
    }
    void halberd2(){
        addStat("dmg",5);
        addStat("dmgMax",5);
        reduceStat("def",2);
        reduceStat("defMax",2);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{1,4});
    }
    void shortBow1(){
        addStat("acc",3);
        addStat("accMax",3);
        reduceStat("dmg",1);
        reduceStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{1,3});
    }
    void shortBow2(){
        addStat("acc",5);
        addStat("accMax",5);
        reduceStat("dmg",2);
        reduceStat("dmgMax",2);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{1,3});
    }
    void longBow1(){
        addStat("acc",3);
        addStat("accMax",3);
        reduceStat("spd",1);
        reduceStat("spdMax",1);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{1,5});
    }
    void longBow2(){
        addStat("acc",5);
        addStat("accMax",5);
        reduceStat("spd",2);
        reduceStat("spdMax",2);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{1,5});
    }
    void woodenBow1(){
        addStat("acc",3);
        addStat("accMax",3);
        reduceStat("def",1);
        reduceStat("defMax",1);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{1,4});
    }
    void woodenBow2(){
        addStat("acc",5);
        addStat("accMax",5);
        reduceStat("def",2);
        reduceStat("defMax",2);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{1,4});
    }
    void lightLeatherArmor1(){
        addStat("def",1);
        addStat("defMax",1);
        addStat("spd",1);
        addStat("spdMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{0,5});
    }
    void lightLeatherArmor2(){
        addStat("def",2);
        addStat("defMax",2);
        addStat("spd",1);
        addStat("spdMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{0,5});
    }
    void lightLeatherArmor3(){
        addStat("def",1);
        addStat("defMax",1);
        addStat("spd",2);
        addStat("spdMax",2);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{0,4});
    }
    void studdedLeatherArmor1(){
        addStat("def",1);
        addStat("defMax",1);
        addStat("dmg",1);
        addStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{0,3});
    }
    void studdedLeatherArmor2(){
        addStat("def",2);
        addStat("defMax",2);
        addStat("dmg",1);
        addStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{0,3});
    }
    void studdedLeatherArmor3(){
        addStat("def",1);
        addStat("defMax",1);
        addStat("dmg",2);
        addStat("dmgMax",2);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{0,4});
    }
    void leatherGloves1(){
        addStat("def",1);
        addStat("defMax",1);
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{0,6});
    }
    void leatherGloves2(){
        addStat("def",2);
        addStat("defMax",2);
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,4});
        addToPopUpQueue(new int[]{0,6});
    }
    void leatherGloves3(){
        addStat("def",1);
        addStat("defMax",1);
        addStat("acc",2);
        addStat("accMax",2);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{0,4});
    }
    void studdedPants1(){
        addStat("spd",1);
        addStat("spdMax",1);
        addStat("dmg",1);
        addStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{0,3});
    }
    void studdedPants2(){
        addStat("spd",2);
        addStat("spdMax",2);
        addStat("dmg",1);
        addStat("dmgMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{0,3});
    }
    void studdedPants3(){
        addStat("spd",1);
        addStat("spdMax",1);
        addStat("dmg",2);
        addStat("dmgMax",2);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{0,5});
    }
    void Glasses1(){
        addStat("spd",1);
        addStat("spdMax",1);
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{0,6});
    }
    void Glasses2(){
        addStat("spd",2);
        addStat("spdMax",2);
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{0,6});
    }
    void Glasses3(){
        addStat("spd",1);
        addStat("spdMax",1);
        addStat("acc",2);
        addStat("accMax",2);
        addToPopUpQueue(new int[]{0,6});
        addToPopUpQueue(new int[]{0,5});
    }
    void crossbow1(){
        addStat("dmg",1);
        addStat("dmgMax",1);
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{0,5});
    }
    void hvyCrossbow(){
        addStat("dmg",2);
        addStat("dmgMax",2);
        addStat("acc",1);
        addStat("accMax",1);
        addToPopUpQueue(new int[]{0,3});
        addToPopUpQueue(new int[]{0,5});
    }
    void crossbow2(){
        addStat("dmg",1);
        addStat("dmgMax",1);
        addStat("acc",2);
        addStat("accMax",2);
        addToPopUpQueue(new int[]{0,5});
        addToPopUpQueue(new int[]{0,3});
    }
    void strengthPotion(){
        buffCountdown[0]+=3;
        buffStrength[0]+=4;
        addStat("dmg",4);
        addToPopUpQueue(new int[]{0,3});
    }
    void potentStrengthPotion(){
        buffCountdown[0]+=3;
        buffStrength[0]+=8;
        addStat("dmg",8);
        addToPopUpQueue(new int[]{0,3});
    }
    void reflexePotion(){
        buffCountdown[2]+=3;
        buffStrength[2]+=4;
        addStat("spd",4); 
        addToPopUpQueue(new int[]{0,5});
    }
    void potentReflexePotion(){
        buffCountdown[2]+=3;
        buffStrength[2]+=8;
        addStat("spd",8); 
        addToPopUpQueue(new int[]{0,5});
    }
    void nightVisionPotion(){
        buffCountdown[3]+=3;
        buffStrength[3]+=4;
        addStat("acc",4); 
        addToPopUpQueue(new int[]{0,6});
    }
    void potentNightVisionPotion(){
        buffCountdown[3]+=3;
        buffStrength[3]+=8;
        addStat("acc",8); 
        addToPopUpQueue(new int[]{0,6});
    }
    void ironSkinPotion(){
        buffCountdown[1]+=3;
        buffStrength[1]+=4;
        addStat("def",4); 
        addToPopUpQueue(new int[]{0,4});
    }
    void potentIronSkinPotion(){
        buffCountdown[1]+=3;
        buffStrength[1]+=8;
        addStat("def",8); 
        addToPopUpQueue(new int[]{0,4});
    }
    void food1(){
        addStat("hp",3);
        addStat("hpMax",3);
        addToPopUpQueue(new int[]{0,2});
    }
    void food2(){
        addStat("hp",7);
        addStat("hpMax",7);
        addToPopUpQueue(new int[]{0,2});
    }
    void food3(){
        addStat("hp",11);
        addStat("hpMax",11);
        addToPopUpQueue(new int[]{0,2});
    }
    void money1(){
        giveGold(25);
    }
    void money2(){
        giveGold(40);
    }
    void money3(){
        giveGold(60);
    }
    void energyDrink1(){
        energyMax+=5;
    }
    void energyDrink2(){
        energyMax+=10;
    }
    void energyDrink3(){
        energyMax+=15;
    }
    void bomb1(){
        giveBomb(1);
        addToPopUpQueue(new int[]{0,7});
    }
    void bomb3(){
        giveBomb(3);
        addToPopUpQueue(new int[]{0,7});
    }
    void bomb5(){
        giveBomb(5);
        addToPopUpQueue(new int[]{0,7});
    }
    void artifact(){
        InterScene.saveFile.stats.artifact +=1;
        InterScene.saveToFile();
    }
    void holyWater(){
        curseCountdown = 0;
        reduceCounter();
    }


    void excalibur(){
        addStat("dmg",7);
        addStat("dmgMax",7);
        addToPopUpQueue(new int[]{0,3});
    } 
    void talaria(){
        addStat("spd",7);
        addStat("spdMax",7);
        addToPopUpQueue(new int[]{0,6});
    } 
    void aegis(){
        addStat("def",7);
        addStat("defMax",7);
        addToPopUpQueue(new int[]{0,4});
    } 
    void artemisBow(){
        addStat("acc",7);
        addStat("accMax",7);
        addToPopUpQueue(new int[]{0,5});
        
    } 
    void gjallarhorn(){
        _bc.increaseChancesForNodesToBeFilled(25);
    } 
    void ankh(){
        ankhProtectionOn = true;
    } 
    void medusaHead(){
        _bc.increaseMedusaStrength(1f);
    } 
    void gleipnir(){
        _bc.increaseStunStrength(0.25f);
    } 
    void bookThoth(){
        _difficultyScaler.reduceTimeSpeed(0.15f);
    }
    void anemoiWind(){
        _player.addDiscoveryRange(1);
    }
}
