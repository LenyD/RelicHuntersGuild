using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Room : MonoBehaviour {
	public int id;
	GameMaster _GM;
	int eventId = 0;
	bool isCurrentlyShowing = false;
	public Tile floor;
	GameObject spoiler;
	public GameObject tutorialTextPrefab;
	float restPower = 1f;
	void Awake(){
		_GM = FindObjectOfType<GameMaster>();
		generateRandomEvent();
	}
	void Start () {
		//show(true);
	}
	void generateRandomEvent(){
		if(id != 0 && id != 16){
			if(Random.Range(0,4)>=3){
				//25% chance of event
				//choose 1 of the event
				int[] eventWeight = new int[]{
					0,  //0	 - NoEvent
					250,//1  - trap
					250,//2  - battle	
					250,//3  - curse
					70,	//4  - chest
					200,//5  - blessing		
					25,	//6  - alchemy table
					20,	//7  - rest
					10,	//8  - lost adventurer			
					30, //9 -  training1
					30, //10 - training2
					30, //11 - training3
					30, //12 - training4
					0, //13 - debris 
				};
				//Total : 1330;
				int totalWeight = 0;
				for (int i = 0; i < eventWeight.Length; i++)
				{
					totalWeight+= eventWeight[i];
				}
				int eventRng = Random.Range(1,totalWeight+1);
				for (int i = 0; i < eventWeight.Length; i++)
				{
					eventRng-=eventWeight[i];
					if(eventRng<=0){
						eventId = i;
						break;
					}
				}				
				if(_GM.getSpoiler(eventId)){
					instantiateSpoiler();
				}
			}
		}
	}
	public void addShopFloor(){
		GameObject shopFloor = (GameObject)Instantiate(Resources.Load("Shop"),new Vector3(0,0,-0.01f),Quaternion.Euler(0,0,0));
		shopFloor.transform.SetParent(floor.transform,false);
		Merchant _merchant = this.gameObject.AddComponent<Merchant>();
		_merchant.setMaxLootTableId(InterScene.jsonData.lootTables[InterScene.currentTier].table.Length);
		_merchant.generateInventory();
	}
	public void addArtifactShopFloor(){
		GameObject shopFloor = (GameObject)Instantiate(Resources.Load("Shop"),new Vector3(0,0,-0.01f),Quaternion.Euler(0,0,0));
		shopFloor.transform.SetParent(floor.transform,false);
		Merchant _merchant = this.gameObject.AddComponent<Merchant>();
		_merchant.setIsRelic(true);
		_merchant.setMaxLootTableId(InterScene.jsonData.relics.Length);
		_merchant.generateInventory();		
	}
	
	public void addBossFloor(){
		GameObject bossFloor = (GameObject)Instantiate(Resources.Load("BossRoom"),new Vector3(0,0,-0.02f),Quaternion.Euler(0,0,0));
		floor.removeAlchTable();
		floor.removeDummy();
		floor.removeAdventurer();
		bossFloor.transform.SetParent(floor.transform,false);
	}
	public void openChest(){
		floor.openChest();
	}
	public float getRoomRestPower(){
		return restPower;
	}
	public void reduceRestPower(){
		restPower = restPower/2;
	}
	public void useAlchTable(){
		floor.useAlchTable();
	}
	public void removeDummy(){
		floor.removeDummy();
	}
	public void removeAdventurer(){
		floor.removeAdventurer();
	}
	public int getRoomId(){
		return id;
	}
	public int getEventId(){
		return eventId;
	}
	public void setRoomEvent(int eventNum){
		if(eventId>0){
			destroySpoiler();
		}
		eventId = eventNum;
		if(eventId>0 && eventId<4){
			instantiateSpoiler();
		}
	}
	public void setTutorialText(string t){
		GameObject text = (GameObject)Instantiate(tutorialTextPrefab,new Vector3(0,2,1),Quaternion.Euler(90,0,0));
		text.GetComponent<TextMeshPro>().text = t;
		text.transform.SetParent(this.transform,false);
	}

	public void show(bool isShowing){
		if(isShowing == true){
			if(isCurrentlyShowing == false){
				_GM.increaseDiscoveredRoom();
				isCurrentlyShowing = isShowing;
				if(_GM.createBossRoom()){
					addBossFloor();
					eventId = 100;//Become boss Room
				}
			}
		}
		float posX = transform.position.x;
		float posZ = transform.position.z;
		if(_GM.listOfWalls.ContainsKey("["+posX+","+posZ+"]")){
			foreach (var wall in _GM.listOfWalls["["+posX+","+posZ+"]"])
			{
				wall.showWalls(isShowing);
			}
		}
		if(spoiler==null){
			if(_GM.getSpoiler(eventId)){
				instantiateSpoiler();
			}
		}
	}
	void destroySpoiler(){
		Destroy(spoiler);
	}
	void instantiateSpoiler(){
		spoiler = (GameObject)Instantiate(Resources.Load("Spoiler"),new Vector3(transform.position.x,0.5f,transform.position.z),new Quaternion());
	}
	
	
	
}
