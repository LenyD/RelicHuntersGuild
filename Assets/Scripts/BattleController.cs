using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    GameMaster _GM;
    Canvas _c;
    Stats _s;
    public Player player;
    public Monster monster;
    public Metronome metronome;
    public Image playerImage;
    public Image monsterImage;
    public GameObject popUpPrefab;
    public Sprite[] bombPopUpSprites;
    public GameObject bombBt;
    Image bombBtImage;
    public Animator bombBtAnim;
    int[] playerProcessedStats;
    int[] monsterProcessedStats;
    Node[] nodes;
    bool isInCombat = false;
    float bombCD = 0;
    float bombCDReset = 2;
    int chancesForNodesToBeFilled = 0;
    float medusaStrength = 0;
    float stunStrengthMultiplier = 1;
    public Text blockText;
    public Text dodgeText;
    // Start is called before the first frame update
    void Awake()
    {
        _GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        _s = GameObject.Find("Party stats").GetComponent<Stats>();
        _c = GetComponent<Canvas>();
        bombBtImage = bombBt.GetComponent<Image>();
        
    }
    void Start()
    {
        nodes = metronome.nodes;
    }

    public void init(){
        _s.resetBlock();
        _s.resetDodge();
        bombCD = bombCDReset;
        bombBtImage.fillAmount = 1-bombCD;
        _c.enabled = true;
        gameObject.SetActive(true);
        bombBtAnim.gameObject.SetActive(false);
        monster.init(medusaStrength,stunStrengthMultiplier);
        playerProcessedStats = processStats(_s.getCurrentStatProfile(),monster.getCurrentStatProfile());
        monsterProcessedStats = processStats(monster.getCurrentStatProfile(),_s.getCurrentStatProfile());
        string DebugString ="";
        for (int i = 0; i < playerProcessedStats.Length; i++)
        {
            DebugString+=  playerProcessedStats[i]+","; 
        }
            DebugString+=  "||"; 
        for (int i = 0; i < monsterProcessedStats.Length; i++)
        {
            DebugString+=  monsterProcessedStats[i]+","; 
        }
        Debug.Log(DebugString);
        metronome.init();
        playerImage.sprite = Resources.Load<Sprite>("CharacterSprites/"+InterScene.jsonData.explorers[InterScene.party[0]].name);

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].init(this,monster,playerProcessedStats[i],chancesForNodesToBeFilled);
        }
        StartCoroutine(startCombat());
    }
    int[] processStats(int[] profile1,int[] profile2){
        //return difference between 1-5
        int[] processedProfile = new int[profile1.Length];
        for (int i = 0; i < profile1.Length; i++)
        {
            int stat = profile1[i]-profile2[i];
            //if > 4 => 4
            stat = Mathf.Min(stat,4);
            //if < 0 => 0
            stat = Mathf.Max(stat,0);
            processedProfile[i] = stat+1;
        }
        return processedProfile;
    }

    public void createPopUp(Sprite[] images,bool isTargetingPlayer){
        GameObject bpopup = Instantiate(popUpPrefab,new Vector3(),Quaternion.identity,this.transform);
        Vector2 v2;
        if(isTargetingPlayer){
            v2 = playerImage.GetComponent<RectTransform>().anchoredPosition;
        }else{
            v2 = monsterImage.GetComponent<RectTransform>().anchoredPosition;
        }
        bpopup.GetComponent<BattlePopUp>().setUp(images,v2);
    }
    IEnumerator startCombat(){
        //Time to zoomin and set up
        yield return new WaitForSeconds(2f);
        isInCombat = true;
        monster.startCombat();
        metronome.startCombat();
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].startCombat();
        }
    }
    public IEnumerator endCombat(){
		//Activate at the end of combat
        isInCombat = false;
		//May generate loot
        monster.endCombat();
        metronome.endCombat();
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].endCombat();
        }
        yield return new WaitForSeconds(1f);
        _c.enabled = false;
        gameObject.SetActive(false);
		_GM.endCombat(_s.getHp()>0);
	}

    // Update is called once per frame
    void Update()
    {
        if(isInCombat){
            bombCD-=Time.deltaTime;
            bombBtImage.fillAmount = 1-bombCD;
            if(bombCD<0){
                bombBtImage.color = new Color(1f,1f,1f,1f);
                if(Input.GetButtonUp("Bomb")){
                    StartCoroutine("useBomb");
                }
            }else{
                bombBtImage.color = new Color(0.35f,0.35f,0.35f,1f);
            }
        }else{
            bombBtImage.color = new Color(0.35f,0.35f,0.35f,1f);
        }
    }
    IEnumerator useBomb(){
        if(_s.getBombs()>0){
            bombBtAnim.gameObject.SetActive(true);
            bombBtAnim.Play("sendPower",0);
            _s.reduceBomb();
            bombCD = bombCDReset;
            yield return new WaitForSeconds(0.5f);
            monster.receiveDamage(0,2);
            createPopUp(bombPopUpSprites,false);

        }
    }
    
    public void updateBlockText(int block){
        blockText.text = block.ToString();
    }
    public void updateDodgeText(int dodge){
        dodgeText.text = dodge.ToString();
    }

    public void setMetronomeDamageScale(float number=1){
        metronome.setDamageScale(number);
    }
    public void increaseChancesForNodesToBeFilled(int number=0){
        chancesForNodesToBeFilled += number;
    }
    public void increaseMedusaStrength(float number=0){
        medusaStrength += number;
    }
    public void increaseStunStrength(float number=0){
        medusaStrength += number;
    }
    
    public void setMetronomeSpeed(float number = 1){
        metronome.setSpeed(number);
    }
    public void setSpearmanPower(int power){
        monster.setSpearmanPower(power);
    }
}
