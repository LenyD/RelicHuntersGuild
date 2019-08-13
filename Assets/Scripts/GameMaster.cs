using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public class GameMaster : MonoBehaviour {

	Camera mainCam;
	bool toggleTutorial = true;
	float zoomedOutCamSize;
	float zoomedInCamSize = 0.8f;
	Coroutine zoomCoroutine;
	int[,] map ;
	//CAUTION
	//More than 100x100 (10000 squares) start to slow down;
	//Min:3,200
	//Max:9,216
	public int dimX=9;
	public int dimY=9;
	int floorSizeIncrement = 6;
	int maxFloorSize = 100;
	int NumberOfTilePlaced = 0;
	int discoveredRooms =0;
	int MinNumberOfTile = 3;
	int MaxNumberOfTile = 100;
	int chanceOfRelicShop = 5;
	int currentPosX;
	int currentPosY;
	Player _player;
	Stats _s;
	Shop _shop;
	public GameObject  curse;
	public GameObject  blessing;
	
	public Dictionary<int, bool[]> PossibleSidesFromId = new Dictionary<int, bool[]>();
	Dictionary<string, int[]> PossibleIdFromSide = new Dictionary<string, int[]>();

	public Dictionary<string,Room> listOfRooms = new Dictionary<string, Room>();
	public Dictionary<string,List<Wall>> listOfWalls = new Dictionary<string, List<Wall>>();
	int numberOfProjectilesInstances = 0;
	//Size of the map in square unit
	//Get number of prefab in Resources/ DIRECTORY VAR
	bool isBossSpawned = false;
	bool[] spoilersOn = {false,false,false,false};
	public int NumberOfShopToGenerate = 3;
	public BattleController _bc;
	public PartyEditor _pe;
	public EndScreen _endScreen;
	public LoadScreen _loadScreen;
	public SoundEffect SE_curse;
    public SoundEffect SE_bless;
	public SoundEffect SE_brewing;
	public SoundEffect SE_chest;

	void Awake(){
		//get nescessary data
		_s = GameObject.Find("Party stats").GetComponent<Stats>();
		_player = GameObject.Find("Player").GetComponent<Player>();
		mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
		_shop = GameObject.Find("Shop Canvas").GetComponent<Shop>();
		zoomedOutCamSize = mainCam.orthographicSize;
	}
	// Use this for initialization
	void Start () {
		createLinks();
		if(toggleTutorial){
			setUpTutorial();
		}else{
			setUpMap();
		}
		InterScene.addAchievementProgress(0,1);
		InterScene.addAchievementProgress(3,1);
	}
	void Update() {
		if(Input.GetButtonUp("Escape")){
			_endScreen.toggleEndScreen();
		}
	}
	void nextFloor(){
		//Destoy current floor
		destroyRooms();
		dimX+=floorSizeIncrement;
		dimY+=floorSizeIncrement;
		if(dimX>maxFloorSize){
			dimX=maxFloorSize;
		}
		if(dimY>maxFloorSize){
			dimY=maxFloorSize;
		}
		InterScene.addAchievementProgress(0,1);
		if(InterScene.timer<1200){
			InterScene.addAchievementProgress(3,1);
		}
		//Load nextFloor;
		setUpMap();
	}
	void destroyRooms(){
		for (int x = 0; x < dimX; x++)
		{
			for (int y = 0; y < dimY; y++)
			{
				int room = getRoomIdAt(x,y);
				if(room!=0){
					Destroy(getRoomAt(x,y).gameObject);
				}
			}
		}
		listOfRooms = new Dictionary<string, Room>();
		listOfWalls = new Dictionary<string, List<Wall>>();
	}
	void setUpMap(){
		//Set position and max/min number of tiles
		currentPosX = Mathf.CeilToInt(dimX/2);
		currentPosY = Mathf.CeilToInt(dimY/2);
		discoveredRooms = 0;
		isBossSpawned = false;
		//Set player position to the middle
		_player.targetPos = new Vector3(currentPosX,0.3f,currentPosY);
		_player.transform.position = new Vector3(currentPosX,_player.transform.position.y,currentPosY);
		_player.clearTrail();
		MinNumberOfTile= (dimX-2 )*(dimY-2) / 3;
		MaxNumberOfTile= (dimX-2 )*(dimY-2)-(dimX*2)-(dimY*2)+4;
		//Generate the map
		CreateMap();
		//Set the starting room
		Room startingRoom = getRoomAt(currentPosX,currentPosY);
		startingRoom.setRoomEvent(0);
		//Set shops to open room
		setUpShops();
		//Reveal the wall around the spawn at the start of the game
		StartCoroutine(revealWall());
	}
	void setUpTutorial(){
		//Set position and max/min number of tiles
		currentPosX = 1;
		currentPosY = 1;
		
		discoveredRooms = 0;
		isBossSpawned = true;
		//Set player position to the middle
		_player.targetPos = new Vector3(currentPosX,0.3f,currentPosY);
		_player.transform.position = new Vector3(currentPosX,_player.transform.position.y,currentPosY);
		_player.clearTrail();
		//MinNumberOfTile= (dimX-2 )*(dimY-2) / 3;
		//MaxNumberOfTile= (dimX-2 )*(dimY-2)-(dimX*2)-(dimY*2)+4;

		//Generate the map
		CreateTutorialMap();

		//Debug.Log(MinNumberOfTile+" - "+MaxNumberOfTile);
		//Debug.Log(NumberOfTilePlaced);

		//Set the starting room
		Room startingRoom = getRoomAt(currentPosX,currentPosY);
		startingRoom.setRoomEvent(0);
		//Set shops to open room
		//Reveal the wall around the spawn at the start of the game
		StartCoroutine(revealWall());
	}

	//MAP CREATION
	void createLinks(){
	//create a variable for each openned/closed sides
		int[] openLeft = 	new int[7] { 1, 2 , 3 , 4 , 5 , 6 , 7 };
		int[] openUp = 		new int[7] { 1, 2 , 3 , 4 , 9 , 10, 11};
		int[] openRight = 	new int[7] { 1, 2 , 5 , 6 , 9 , 10, 13};
		int[] openDown = 	new int[7] { 1, 3 , 5 , 7 , 9 , 11, 13};
		int[] closeLeft = 	new int[8] { 9, 10, 11, 12, 13, 14, 15, 16};
		int[] closeUp = 	new int[8] { 5, 6 , 7 , 8 , 13, 14, 15, 16};
		int[] closeRight = 	new int[8] { 3, 4 , 7 , 8 , 11, 12, 15, 16};
		int[] closeDown = 	new int[8] { 2, 4 , 6 , 8 , 10, 12, 14, 16};

		//fill PossibleIdFromSide
		PossibleIdFromSide.Add("openLeft"	,openLeft);
		PossibleIdFromSide.Add("openUp"		,openUp);
		PossibleIdFromSide.Add("openRight"	,openRight);
		PossibleIdFromSide.Add("openDown"	,openDown);
		PossibleIdFromSide.Add("closeLeft"	,closeLeft);
		PossibleIdFromSide.Add("closeUp"	,closeUp);
		PossibleIdFromSide.Add("closeRight"	,closeRight);
		PossibleIdFromSide.Add("closeDown"	,closeDown);
		
		// Link [ LEFT	,UP		,RIGHT	,BOTTOM]
		// 00 - [ True	,True	,True	,True ]
		// 01 - [ True	,True	,True	,True ]
		// 02 - [ True	,True	,True	,False ]
		// 03 - [ True	,True	,False	,True ]		
		// 04 - [ True	,True	,False	,False ]
		// 05 - [ True	,False	,True	,True ]
		// 06 - [ True	,False	,True	,False ]
		// 07 - [ True	,False	,False	,True ]
		// 08 - [ True	,False	,False	,False ]
		// 09 - [ False	,True	,True	,True ]
		// 10 - [ False	,True	,True	,False ]
		// 11 - [ False	,True	,False	,True ]
		// 12 - [ False	,True	,False	,False ]
		// 13 - [ False	,False	,True	,True ]
		// 14 - [ False	,False	,True	,False ]
		// 15 - [ False	,False	,False	,True ]
		// 16 - [ False	,False	,False	,False ]

		//Add the empty tile to be all false;
		PossibleSidesFromId.Add(0, new bool[4]{true, true, true, true});
		//Create this binary sorted Dictionary
		for (int i = 0; i < 2; i++)
		{ 
			for (int j = 0; j < 2; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 2; l++)
					{
						bool[] tempArray = new bool[4]{
							(i<1),
							(j<1),
							(k<1),
							(l<1)
						};
						int roomId = i*8+j*4+k*2+l+1;
						PossibleSidesFromId.Add(roomId, tempArray);
					}
				}
			}
		}
	}
	void CreateTutorialMap(){
	// Create the map
		//Directory in ressources/...
		//Create the array
		map = new int[dimX,dimY];
		//Fill the borders of the map & set the starting tile to an open tile
		addRoomToMap(1,1,1);
		addRoomToMap(2,1,1);
		addRoomToMap(3,1,1);
		addRoomToMap(4,1,1);
		addRoomToMap(5,1,1);
		addRoomToMap(6,1,1);
		addRoomToMap(7,1,1);
		addRoomToMap(8,1,1);
		addRoomToMap(9,1,1);
		addRoomToMap(9,2,1);
		addRoomToMap(9,3,1);
		addRoomToMap(8,3,1);
		addRoomToMap(8,4,1);
		addRoomToMap(8,5,1);
		addRoomToMap(7,5,1);
		addRoomToMap(6,5,1);
		addRoomToMap(5,5,1);
		addRoomToMap(5,4,1);
		addRoomToMap(5,3,1);
		addRoomToMap(4,3,1);
		addRoomToMap(3,3,1);
		addRoomToMap(2,3,1);
		addRoomToMap(1,3,1);
		addRoomToMap(1,4,1);
		addRoomToMap(1,5,1);
		addRoomToMap(1,6,1);
		addRoomToMap(1,7,1);
		addRoomToMap(1,8,1);
		addRoomToMap(2,8,1);
		addRoomToMap(3,8,1);
		addRoomToMap(4,8,1);
		addRoomToMap(5,8,1);
		addRoomToMap(6,8,1);



		for (int x = 0; x < dimX; x++)
		{
			for (int y = 0; y < dimY; y++)
			{
				if(map[x,y]==0){
					map[x,y]=16;
				}
			}
		}
		//Instantiate every room
		for (int x = 0; x < dimY; x++)
		{
			for (int y = 0; y < dimY; y++)
			{
				InstantiateRoom(x,y);	
			}
		}
		for (int x = 0; x < dimX; x++)
		{
			for (int y = 0; y < dimY; y++)
			{
				if(map[x,y]==1){
					getRoomAt(x,y).setRoomEvent(0);
				}
			}
		}
		getRoomAt(1,1).setTutorialText("WASD / Arrows to move\nR to heal hp & stamina");
		getRoomAt(4,1).setRoomEvent(4);
		//getRoomAt(4,1).setTutorialText("Test");
		getRoomAt(5,1).setRoomEvent(3);
		//getRoomAt(6,1).setTutorialText("Cool text that is pretty long to test how long i can write a description for these tutorial thingy-ma-jiggy");
		getRoomAt(5,1).setTutorialText("Cursed room reduce your stats temporarely, effect countered from blessing");
		getRoomAt(9,1).setRoomEvent(5);
		getRoomAt(9,1).setTutorialText("Blessed room increase your stats temporarely, effect countered from curse");
		getRoomAt(8,5).setRoomEvent(6);
		getRoomAt(8,5).setTutorialText("Alchemy tables increase your stats temporarely and heal you");
		getRoomAt(5,5).setRoomEvent(7);
		getRoomAt(5,5).setTutorialText("Rest camp heals your hp & stamina, reduce effectiveness after every use");
		getRoomAt(4,3).setRoomEvent(9);
		getRoomAt(4,3).setTutorialText("Increase a stat at the cost of stamina");
		getRoomAt(1,3).setRoomEvent(1);
		getRoomAt(1,3).setTutorialText("Trapped room can be dodged");
		getRoomAt(1,6).setRoomEvent(2);
		getRoomAt(0,6).setTutorialText("While in combat, fill your nodes to attack(1-4), acc & att to deal damage while block & dodge increase your defense");
		getRoomAt(3,8).setRoomEvent(50);
		getRoomAt(3,8).addShopFloor();
		getRoomAt(6,8).setRoomEvent(100);
		getRoomAt(6,8).setTutorialText("This room will take you to the next floor");
		getRoomAt(6,8).addBossFloor();

	}

	void CreateMap(){
	// Create the map
		//Directory in ressources/...
		//Create the array
		map = new int[dimX,dimY];
		
		//Fill the borders of the map & set the starting tile to an open tile
		createBorders();
		addRoomToMap(currentPosX,currentPosY,1);

		//Will create tiles around the given position(createNextTile is recursive)
		createNextTile(currentPosX,currentPosY);

		//open the map by removing the half closed room
		for (int x = 0; x < dimX; x++)
		{
			for (int y = 0; y < dimY; y++)
			{
				if(map[x,y]!=0 && map[x,y]!=16){
					map[x,y]=1;
					NumberOfTilePlaced++;
				}
			}
		}

		//If the numberoftile is not within the set limit, recreate the map.
		if(NumberOfTilePlaced<MinNumberOfTile || NumberOfTilePlaced > MaxNumberOfTile){
			NumberOfTilePlaced = 0;
			CreateMap();
			return;
		}
		//keep track of the number of room still hidden
		_s.setNumberOfHiddenRoom(NumberOfTilePlaced);
		//Instantiate every room
		for (int x = 0; x < dimX; x++)
		{
			for (int y = 0; y < dimY; y++)
			{
				InstantiateRoom(x,y);	
			}
		}

	}
	void createBorders(){
		for (int x = 0; x < dimX; x++)
		{
			map[x,0] = 16;
			map[x,dimY-1] = 16;
		}
		for (int y = 0; y < dimY; y++)
		{
			map[0,y] = 16;			
			map[dimX-1,y] = 16;
		}
	}
	void createNextTile(int x,int y){
		if(x==0 || y==0 || x==dimX-1 ||y==dimY-1){
			//If away from border
		}else{
			//If the tile to the LEFT is still unasigned & the current tile is open left, start generating a tile there
			if(map[x-1,y]==0){
				if(PossibleSidesFromId[map[x,y]][0]){
					//Generate a tile the fits the surrounding tiles
					generateWithSurrounding(x-1,y,PossibleIdFromSide["openRight"][Random.Range(1,PossibleIdFromSide["openRight"].Length)]);
					//Continue generation from the newly created tile
					createNextTile(x-1,y);
				}else{
					//If LEFT is assign or current tile is closed left, left tile become a walled off tile
					generateWithSurrounding(x-1,y,16);
				}
			}
			//Same for UP
			if(map[x,y+1]==0){
				if(PossibleSidesFromId[map[x,y]][1]){
					generateWithSurrounding(x,y+1,PossibleIdFromSide["openDown"][Random.Range(1,PossibleIdFromSide["openDown"].Length)]);
					createNextTile(x,y+1);
				}else{
					generateWithSurrounding(x,y+1,16);
				}
			}
			//Same for Right
			if(map[x+1,y]==0){
				if(PossibleSidesFromId[map[x,y]][2]){
					generateWithSurrounding(x+1,y,PossibleIdFromSide["openLeft"][Random.Range(1,PossibleIdFromSide["openLeft"].Length)]);
					createNextTile(x+1,y);

				}else{
					generateWithSurrounding(x+1,y,16);
				}
			}
			//Same for Down
			if(map[x,y-1]==0){
				if(PossibleSidesFromId[map[x,y]][3]){
					generateWithSurrounding(x,y-1,PossibleIdFromSide["openUp"][Random.Range(1,PossibleIdFromSide["openUp"].Length)]);
					createNextTile(x,y-1);
				}else{
					generateWithSurrounding(x,y-1,16);
				}
			}
		}
	}

	void generateWithSurrounding(int x,int y, int roomId = 0){
		//If the room is unassign, generate a random room
		if(roomId ==0){
			roomId = Random.Range(1,15);//checkSides(x,y);
		}
		//Add the id to the map array at the indicated coordinates
		addRoomToMap(x,y, roomId);
	}
	void addRoomToMap(int x,int y, int roomId){
	//Add the roomID to the map array at the indicated coordinates
		map[x,y] = roomId;
	}
	void InstantiateRoom(int x,int y){
		//Instantiates room object according to the map array's id
		string directory = "rooms/";
		if(map[x,y]!=0){
			GameObject theRoomGameObject = (GameObject)Instantiate(Resources.Load(directory+"Room"+map[x,y].ToString()),new Vector3(x,0,y), new Quaternion());
			//Change the room.name for easy find later
			theRoomGameObject.name = "Room["+x+","+y+"]";
			Room theRoom = theRoomGameObject.GetComponent<Room>();
			addRoomToList(Mathf.FloorToInt(x),Mathf.FloorToInt(y), theRoom);
			//Hide the room to reveal when in sight;
			theRoom.show(false);
		}
	}
	void setUpShops(){
		//Generate x number of shop and set the room event
		for (int i = 0; i < NumberOfShopToGenerate; i++)
		{
			int x = Random.Range(1,dimX-1);
			int y = Random.Range(1,dimY-1);
			if(getRoomIdAt(x,y)<=0 || getRoomIdAt(x,y)>=16){
				//If there is no room or is walled off
				//reduce counter to get new random coordinates
				i--;
			}else{
				Room _r = getRoomAt(x,y);
				int rng = Random.Range(0,100);
				if(rng>100-chanceOfRelicShop){
					_r.setRoomEvent(51);
					_r.addArtifactShopFloor();
				}else{
					_r.setRoomEvent(50);
					_r.addShopFloor();
				}
			}
		}
	}

	//PUBLIC FUNCTION
	public void addRoomToList(int x, int y, Room r){
		//Add room object to the list at the key "[X,Y]"
		StringBuilder sb = new StringBuilder();
		sb.Append("[").Append(x).Append(",").Append(y).Append("]");
		string key = sb.ToString();
		//If the key exist, replace the room, 
		//if not add the key and the room
		if(listOfRooms.ContainsKey(key)){
			listOfRooms[key]=r;
		}else{
			listOfRooms.Add(key,r);
		}
	}
	public void addWallsToList(int x, int y, Wall w){
		//Add wall object to the list at the key "[X,Y]"
		StringBuilder sb = new StringBuilder();
		sb.Append("[").Append(x).Append(",").Append(y).Append("]");
		string key = sb.ToString();
		//If the key exist, add the wall to the list of wall relevent to that room, 
		//if not add the key and create a new List of wall 
		//and then recursive to add the wall
		if(listOfWalls.ContainsKey(key)){
			listOfWalls[key].Add(w);
		}else{
			listOfWalls.Add(key,new List<Wall>());
			addWallsToList(x,y,w);
		}
	}
	public bool createBossRoom(){
		if(!isBossSpawned){
		//Discover 2/3 of the dungeon before spawning boss room
			if(discoveredRooms>NumberOfTilePlaced/1.5f){
				//More room discovered give more chance to spawn
				//1 chance per 100 room spawn out of the number of room left
				if(Random.Range(0,NumberOfTilePlaced-discoveredRooms)<=NumberOfTilePlaced/100){
					isBossSpawned = true;
					return true;
				}
			}
		}
		return false;
	}
	public void setSpoiler(int id,bool b=true){
		//Enable spoilers for a room type
		//1 = trap
		//2 = monster
		//3 = curse
		spoilersOn[id] = b;
	}
	
	public bool getSpoiler(int id){
		//return bool isSpoilerXEnabled
		if(spoilersOn.Length>id){
			return spoilersOn[id];
		}
		return false;
	}
	public void closeShop(){
		//Reset zoom and roomLock when closing shop
		setZoom(false);
		switchControl(false);
	}
	public void removeProjectileInstance(){
		//Count how many projectile from roomTrap event are left
		//If there is none left, reset zoom and roomLock
		numberOfProjectilesInstances--;
		if(numberOfProjectilesInstances<=0){
			setZoom(false);
			switchControl(false);
			_s.reduceCounter();
		}
	}
	public void activateEvent(int id,float x, float y, Room currentRoom){
		currentRoom.setRoomEvent(0);
		//Check if there is an event for the currentRoom
		//set current room to no further event
		//activate event according to the event id parameter
		_s.reduceEnergy(1);
		_s.addIncome();
		switch(id){
			case 1: trap();break;
			case 2:initiateCombat();break;
			case 3:StartCoroutine(createCurse());break;
			case 4:StartCoroutine(createLoot());currentRoom.openChest();break;
			case 5:StartCoroutine(createBlessing());break;
			case 6:StartCoroutine(alchemyTable());currentRoom.useAlchTable();break;
			case 7:StartCoroutine(campSite(currentRoom.getRoomRestPower(),currentRoom));currentRoom.reduceRestPower();break;
			case 8:generatePartyEditor(currentRoom);;break;
			case 9 :StartCoroutine(trainingRoom(0,currentRoom));break;
			case 10:StartCoroutine(trainingRoom(1,currentRoom));break;
			case 11:StartCoroutine(trainingRoom(2,currentRoom));break;
			case 12:StartCoroutine(trainingRoom(3,currentRoom));break;
			case 50:generateShop();break;
			case 51:generateRelicShop();break;
			case 100:loadNextFloor();break;
			default:setZoom(false);return;
		}
		//if there is an event, zoomin and lock to room
		switchControl(true);
		//Reset shop event
		switch(id){
			case 7: currentRoom.setRoomEvent(7);break;
			case 50: currentRoom.setRoomEvent(50);break;
			case 51: currentRoom.setRoomEvent(51);break;
		}
		setZoom(true);
	}
	public void endCombat(bool isGivingLoot){
		//Activate at the en of combat
		//May generate loot
		if(isGivingLoot){
			generateMonsterLoot();
			if(_s.getpartyHasCook()){
				_s.cookEffect();
			}
		}
		StartCoroutine(exitCombat());
	}


	//ROOMEVENT related function

	void setZoom(bool zoomIn){
		//Start coroutines to zoom in & out
		if(zoomCoroutine!= null){
			StopCoroutine(zoomCoroutine);
		}
		if(zoomIn){
			zoomCoroutine = StartCoroutine(ZoomIn());
		}else{
			zoomCoroutine =	StartCoroutine(ZoomOut());
		}
	}
	void switchControl(bool isRoomLocked){
		//Set player bool to switch to controls that lock them to the room
		_player.switchControl(isRoomLocked);
	}
	void loadNextFloor(){
		StartCoroutine("loadFloorWithLoadScreen");
	}
	IEnumerator loadFloorWithLoadScreen(){
		//Give time to zoomIn
		_s._difficultyScaler.pauseTimer();
		yield return new WaitForSeconds(1f);
		setLoadScreen(true);
		nextFloor();
		yield return new WaitForSeconds(2f);
		setLoadScreen(false);
		switchControl(false);
		setZoom(false);
		_s._difficultyScaler.playTimer();


	}
	void setLoadScreen(bool b){
		_loadScreen.gameObject.SetActive(b);
	}
	void trap(){
		InterScene.addAchievementProgress(7,1);
		//Generate a random number of projectile at random interval
		//between maxNumberOfProjectile/2 & maxNumberOfProjectile

		bool[][] possibility = { new bool[3],new bool[3],new bool[3]};
		possibility[0][0] = true;
		possibility[1][1] = true;
		possibility[2][2] = true;
		bool[] horizontal = possibility[Random.Range(0,possibility.Length)];
		bool[] vertical = possibility[Random.Range(0,possibility.Length)];
		int[] possibleHorizontalDirection = {90,270};
		int[] possibleVerticalDirection = {0,180};
		//0=up,90=right,180=down,270=left
		int direction;
		for (int i = 0; i < horizontal.Length; i++)
		{
			if(!horizontal[i]){
				direction = Random.Range(0,2);
				StartCoroutine(createProjectile(_player.transform.position.x + (1*(direction*2-1)),_player.transform.position.z + 0.3f*(i-1),possibleHorizontalDirection[direction]));
				numberOfProjectilesInstances++;
			}
		}
		for (int i = 0; i < vertical.Length; i++)
		{
			if(!vertical[i]){
				direction = Random.Range(0,2);
				StartCoroutine(createProjectile(_player.transform.position.x + 0.3f*(i-1),_player.transform.position.z + (1*(direction*2-1)),possibleVerticalDirection[direction]));
				numberOfProjectilesInstances++;
			}
		}
		/*
		numberOfProjectiles = Random.Range(maxNumberOfProjectiles/2,maxNumberOfProjectiles+1);
		while (numberOfProjectiles>0)
		{
			//Add & remove to counter
			StartCoroutine(createProjectile());
			numberOfProjectiles--;
			numberOfProjectilesInstances++;
		}
		 */

	}


	void generateShop(){
		Merchant m = getRoomAt(_player.transform.position.x,_player.transform.position.z).GetComponent<Merchant>();
		_shop.openShop(m);
	}
	void generateRelicShop(){
		Merchant m = getRoomAt(_player.transform.position.x,_player.transform.position.z).GetComponent<Merchant>();
		_shop.openShop(m);
	}
	void generatePartyEditor(Room currentRoom){
		_pe.setUpCanvas();
		currentRoom.removeAdventurer();
		//yield return new WaitForSeconds(2f);
		//setZoom(false);
		//switchControl(false);
	}
	void initiateCombat(){
		InterScene.addAchievementProgress(8,1);
		_bc.init();

	}
	void generateMonsterLoot(){
		int min = 5;	//inclusive
		int max = 15;	//inclusive
		int loot = Random.Range(min,max+1);
		_s.giveGold(loot);
	}

	//Coroutine
	IEnumerator createProjectile(float x, float y,int direction){
		//Direction 0 ==
		float waitFor = Random.Range(1f,3f);
		yield return new WaitForSeconds(waitFor);
		Instantiate(Resources.Load("Projectile"),new Vector3(x,0,y),Quaternion.Euler(0,direction,0));
	}
	/*
	IEnumerator startCombat(Monster m){
	
		float waitFor = 0.5f;
		yield return new WaitForSeconds(waitFor);		

		if(m.getSpeed()>_s.getSpeed()){
			StartCoroutine(attack(m,false));
		}else{
			StartCoroutine(attack(m,true));
		}
	}
	 */
	IEnumerator createLoot(){
		SE_chest.playSound();
		InterScene.addAchievementProgress(1,1);
		InterScene.addAchievementProgress(2,1);
		float waitFor = 1f;
		yield return new WaitForSeconds(waitFor);
		_s.createItem();
		yield return new WaitForSeconds(waitFor);
		setZoom(false);
		switchControl(false);
	}
	IEnumerator alchemyTable(){
		float waitFor = 1f;
		SE_brewing.playSound();
		yield return new WaitForSeconds(waitFor);
		_s.useAlchemyTable();
		yield return new WaitForSeconds(waitFor);
		setZoom(false);
		switchControl(false);
	}
	IEnumerator campSite(float power,Room r){
		float waitFor = 1f;
		yield return new WaitForSeconds(waitFor);
		_s.camp(power);
		yield return new WaitForSeconds(waitFor);
		setZoom(false);
		switchControl(false);
	}
	IEnumerator trainingRoom(int id, Room currentRoom){
		float waitFor = 1f;
		yield return new WaitForSeconds(waitFor);
		_s.training(id);
		yield return new WaitForSeconds(waitFor);
		currentRoom.removeDummy();
		setZoom(false);
		switchControl(false);
	}
	


	
	
	
	/*
	IEnumerator attack(Monster m,bool isPlayersTurn){
		float waitFor = 0.5f;
		if(isPlayersTurn){
			int attack = _s.getAttack();
			m.receiveDamage(attack);
		}else{
			int attack = m.getAttack();
			_s.receiveDamage(attack);
		}

		yield return new WaitForSeconds(waitFor);
		if(m.getHp() > 0 && _s.getHp()>0 ){
			StartCoroutine(attack(m,!isPlayersTurn));
		}else{
			endCombat(_s.getHp()>0);
		}
	}
	*/

	
	IEnumerator exitCombat(){
		float waitFor = 1f;
		yield return new WaitForSeconds(waitFor);
		_s.reduceCounter();
		setZoom(false);
		switchControl(false);
	}
	IEnumerator createCurse(){
		InterScene.addAchievementProgress(9,1);
		//Instantiate(Resources.Load("Curse"),_player.transform.position,new Quaternion());
		curse.transform.position = _player.transform.position;
		curse.SetActive(true);
		SE_curse.playSound();
		float waitFor = 3f;
		yield return new WaitForSeconds(waitFor);
		curse.SetActive(false);
		_s.addCurseCounter();
		setZoom(false);
		switchControl(false);
	}
	IEnumerator createBlessing(){
		blessing.transform.position = _player.transform.position;
		blessing.SetActive(true);
		SE_bless.playSound();
		float waitFor = 3f;
		yield return new WaitForSeconds(waitFor);
		blessing.SetActive(false);
		_s.addBlessCounter();
		setZoom(false);
		switchControl(false);
	}
	

	//OTHER IEnumerator
	IEnumerator revealWall(){
		//reveal the wall around the starting zone with a delay
		//The delay is so that the gameobject are all created when calling it
		float waitFor = 0.1f;
		yield return new WaitForSeconds(waitFor);
		_player.revealWall();
	}
	public int getRoomIdAt(float x, float y){
		int intX = Mathf.RoundToInt(x);
		int intY = Mathf.RoundToInt(y);
		//return the roomId at x , y
		return map[intX,intY];
	}

	public Room getRoomAt(float x, float y){
		int intX = Mathf.RoundToInt(x);
		int intY = Mathf.RoundToInt(y);
		Room room = listOfRooms["["+intX+","+intY+"]"];
		//return the Room at x , y
		return room;
	}
	public void increaseDiscoveredRoom(){
		//Increase the counter of discovered room
		discoveredRooms++;
		_s.reduceNumberOfHiddenRoom();
	}
	
	IEnumerator ZoomIn(){
		float loopIncrement = 0.04f;
		_player.setCanMove(false);
		while (mainCam.orthographicSize > zoomedInCamSize)
		{
			mainCam.orthographicSize -= loopIncrement;
			yield return new WaitForSeconds (0.01f);
		}
		_player.setCanMove(true);
		mainCam.orthographicSize = zoomedInCamSize;

		/*
		float camIncrement = (zoomedOutCamSize - zoomedInCamSize)*loopIncrement;
		for (float i = 0; i < 1; i += loopIncrement)
		{
			//float newZoom = Mathf.SmoothDamp(mainCam.orthographicSize,zoomedCamSize,ref yVelocity,0.1f,100);
			//mainCam.fieldOfView -= camIncrement;//perspective
			mainCam.orthographicSize -= camIncrement; //orthographic
			yield return new WaitForSeconds (0.01f);
		}
		 */
	}

	IEnumerator ZoomOut(){
		float loopIncrement = 0.04f;
		_player.setCanMove(false);
		while (mainCam.orthographicSize < zoomedOutCamSize)
		{
			mainCam.orthographicSize += loopIncrement;
			yield return new WaitForSeconds (0.01f);
		}
		_player.setCanMove(true);
		mainCam.orthographicSize = zoomedOutCamSize;
/*
		//float camIncrement = (zoomedOutCamSize - zoomedInCamSize)*loopIncrement;
		for (float i = 0; i < 1; i += loopIncrement)
		{
			//float newZoom = Mathf.SmoothDamp(mainCam.orthographicSize,zoomedCamSize,ref yVelocity,0.1f,100);
			//mainCam.fieldOfView += camIncrement;
			mainCam.orthographicSize += camIncrement; //orthographic
			yield return new WaitForSeconds (0.01f);


		}
 */
		
	}

}
