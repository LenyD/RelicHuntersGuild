using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    //Dictionary<string,int> statProfile = new Dictionary<string,int>();
    int[] statProfile = new int[5];
    float minTimer = 3000f;
    float maxTimer = 13000f;
    int spearmanPower = 0;
    public EnemyNode[] nodes;
    public BattleController _bc;
    public Sprite[] popUpSprites;
    public SoundEffect SE_hurt;
    int block;
    int dodge;
    int maxDodge = 90;
    public Text blockText;
    public Text dodgeText;
    Coroutine[] cr;
    // Start is called before the first frame update
    void Awake(){
        _bc=GameObject.Find("Battle Canvas").GetComponent<BattleController>();
    }
    void Start()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].setMonster(this);
        }
    }
    public void init(float medusaStrength,float stunStrengthMultiplier){
        generateProfile();
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].init(_bc);
            nodes[i].setCooldownLength(stunStrengthMultiplier);
            nodes[i].setCooldown(medusaStrength);
        }
        resetBlock();
        resetDodge();
    }
    public void startCombat(){
        cr = new Coroutine[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            cr[i] = StartCoroutine(battle(i));
        }
    }
    public void endCombat(){
        for (int i = 0; i < cr.Length; i++)
        {
            if(cr[i]!=null){
                StopCoroutine(cr[i]);
            }
        }
    }
    public int[] getCurrentStatProfile(){
        return new int[] {statProfile[1],statProfile[2],statProfile[3],statProfile[4]};
    }

    IEnumerator battle(int i){
        float timer = randomTimer();
        yield return new WaitForSeconds(timer);
        nodes[i].increaseSize(statProfile[i+1]);
        cr[i] = StartCoroutine(battle(i));
    }
    float randomTimer(){
        return Random.Range(minTimer,maxTimer)/1000;
        
    }
    void generateProfile(){
        int tier = InterScene.currentTier;
        int rng = Random.Range(0,InterScene.jsonData.EnemiesTables[tier].table.Length);
        statProfile[0] = InterScene.jsonData.EnemiesTables[tier].table[rng].health;
        statProfile[1] = InterScene.jsonData.EnemiesTables[tier].table[rng].strength;
        statProfile[2] = InterScene.jsonData.EnemiesTables[tier].table[rng].block;
        statProfile[3] = InterScene.jsonData.EnemiesTables[tier].table[rng].evade;
        statProfile[4] = InterScene.jsonData.EnemiesTables[tier].table[rng].accuracy;
        _bc.monsterImage.sprite = Resources.Load<Sprite>("MonsterSprites/"+InterScene.jsonData.EnemiesTables[tier].table[rng].image);
    }
    public void receiveDamage(int damage,int piercingDamage){
        Sprite[] sprites;
        if(evadeCheck()){
            resetDodge();
            sprites = new Sprite[]{popUpSprites[1],popUpSprites[5]};
            _bc.createPopUp(sprites,false);
            return;
        }
        piercingDamage+=spearmanPower;
        damage = Mathf.Max(0,damage-spearmanPower);
        damage = Mathf.Max(0,damage - block);
        int finalDmg = damage+piercingDamage;
        reduceBlock(finalDmg);
        if(finalDmg>0 || piercingDamage>0){
            sprites = new Sprite[]{popUpSprites[1],popUpSprites[2]}; 
        }else{
            sprites = new Sprite[]{popUpSprites[1],popUpSprites[4]}; 
        }
        _bc.createPopUp(sprites,false);
        _bc.createPopUp(sprites,false);
        statProfile[0] -= finalDmg;
        SE_hurt.playSound();
        if(getHp() <= 0 ){
			_bc.StartCoroutine("endCombat");
		}
    }
    bool evadeCheck(){
        return (100<Random.Range(0+dodge,100+dodge));
    }
    public int getHp(){
        //return statProfile["hp"];
        return statProfile[0];
    }
    public void setSpearmanPower(int power){
        spearmanPower = 1;
    }
    public void addBlock(int increment){
        block += increment;
        updateBlockText();
    }
    public void reduceBlock(int decrement){
        block = Mathf.Max(0,block-decrement);
        updateBlockText();
    }
    public void resetBlock(){
        block = 0;
        updateBlockText();
    }
    public void addDodge(int increment){
        dodge = Mathf.Min(maxDodge, dodge+increment);
        updateDodgeText();
    }
    public void resetDodge(){
        dodge = 0;
        updateDodgeText();
    }
    void updateBlockText(){
        blockText.text = block.ToString();
    }
    void updateDodgeText(){
        dodgeText.text = dodge.ToString();
    }
}
