using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.Input;

public class Player : MonoBehaviour {

	
	GameMaster _GM;
	public Vector3 targetPos;
	public SoundEffect SE_footStep;
	Room currentRoom;
	public float Speed=22f;
	bool isLockedToRoom = false;
	bool canMove = true;
	public int DiscoveryRange = 3;
	bool checkAroundForEvent = false;
	public Light oracleLight;
	public SpriteRenderer[] characterSprites;
	Coroutine oracleLightCoroutine;
	// Use this for initialization
	public TrailRenderer trailRenderer;
	string spritesPath = "CharacterSprites/";
	public bool isPaused {get; set;}
	

	void Awake(){
		isPaused=false;
		//disableTrail();
		_GM = FindObjectOfType<GameMaster>();
		targetPos = transform.position;	
		//enableTrail();
	}
	void Start () {
	}
	
	public void setCharacterSprites(int[] party){
		for (int i = 0; i < characterSprites.Length; i++)
		{
			characterSprites[i].enabled=(i<party.Length);
			if(i<party.Length){
				characterSprites[i].sprite= Resources.Load<Sprite>(spritesPath+InterScene.jsonData.explorers[party[i]].name) ;
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if(isPaused){
			return;
		}
		//MoveSet 1 (Locked to room)
		//move between 9 part of a square unit
		if(canMove){
			if(isLockedToRoom){
				if(transform.position == targetPos){
					targetPos = transform.position + (Vector3.right * Input.GetAxis("Horizontal") *0.3f);
					//targetPos = transform.position + (Vector3.forward * Input.GetAxis("Vertical") *0.3f);
					if(Mathf.Round(targetPos.x)!=Mathf.Round(transform.position.x) || Mathf.Round(targetPos.z)!=Mathf.Round(transform.position.z)){
						targetPos = transform.position;
					}
				}

				if(transform.position == targetPos){
					//targetPos = transform.position + (Vector3.right * Input.GetAxis("Horizontal") *0.3f);
					targetPos = transform.position + (Vector3.forward * Input.GetAxis("Vertical") *0.3f);
					if(Mathf.Round(targetPos.x)!=Mathf.Round(transform.position.x) || Mathf.Round(targetPos.z)!=Mathf.Round(transform.position.z)){
						targetPos = transform.position;
					}
				}else{
					moveTo(targetPos,Speed*4);
				}
			}else{
			//MoveSet 2 (around the map)
			//move between the 4 cardinal direction if the maps allow it
				if(transform.position == targetPos){
					if(Input.GetAxis("Horizontal")<0){
						if(!checkLeftRoom(transform.position.x, transform.position.z)){
							return;
						}
						

					}
					if(Input.GetAxis("Horizontal")>0){
						if(!checkRightRoom(transform.position.x, transform.position.z)){
							return;
						}
						

					}
					targetPos = transform.position + (Vector3.right * Input.GetAxis("Horizontal"));

				}

				if(transform.position == targetPos){
					//Check if the button is pressed and the side pressed is reachable
					if(Input.GetAxis("Vertical")<0){
						if(!checkBottomRoom(transform.position.x, transform.position.z)){
							return;
						}
						
					}
					if(Input.GetAxis("Vertical")>0){
						if(!checkUpRoom(transform.position.x, transform.position.z)){
							return;
						}
						;
					}
					targetPos = transform.position + Vector3.forward * Input.GetAxis("Vertical");

				}else{
					moveTo(targetPos,Speed);
					
				}
			}
		}
	}
	void moveTo(Vector3 pos,float speed){
		//Change postion to moveToward target pos
		transform.position = Vector3.Lerp(transform.position,pos,speed*Time.deltaTime);
		SE_footStep.playSound();
		if(!isLockedToRoom){
		//If is in moveset 2
		//Stop when close and reveal wall around
		//Check around with the oracle power
		//then activate the current room event
			if(Vector3.Distance(transform.position,pos)<=speed*Time.deltaTime){
				transform.position = pos;
				currentRoom = _GM.getRoomAt(pos.x,pos.z);
				revealWall();
				checkAround();
				_GM.activateEvent(currentRoom.getEventId(),pos.x,pos.z,currentRoom);
			}
		}
	}

	//CheckRoom in X Direction and return whether you can or not
	bool checkLeftRoom(float x, float y){
		int currentRoomId = _GM.getRoomIdAt(x,y);
		int leftRoom = _GM.getRoomIdAt(x-1f,y);
		return (leftRoom==1 && currentRoomId==1);
	}
	bool checkUpRoom(float x, float y){
		int currentRoomId = _GM.getRoomIdAt(x,y);
		int upRoom = _GM.getRoomIdAt(x,y+1f);
		return (upRoom==1 && currentRoomId==1);//(_GM.PossibleSidesFromId[upRoom][3] && _GM.PossibleSidesFromId[currentRoomId][1]);
	}
	bool checkRightRoom(float x, float y){
		int currentRoomId = _GM.getRoomIdAt(x,y);
		int rightRoom = _GM.getRoomIdAt(x+1f,y);
		return (rightRoom==1 && currentRoomId==1);//(_GM.PossibleSidesFromId[rightRoom][0] && _GM.PossibleSidesFromId[currentRoomId][2]);
	}
	bool checkBottomRoom(float x, float y){
		int currentRoomId = _GM.getRoomIdAt(x,y);
		int bottomRoom = _GM.getRoomIdAt(x,y-1f);
		return (bottomRoom==1 && currentRoomId==1);//(_GM.PossibleSidesFromId[bottomRoom][1] && _GM.PossibleSidesFromId[currentRoomId][3]);
	}
	bool checkDiagRoom(float x, float y,bool isLeft,bool isDown){
	//Check from pos(x,y) to a diagonal chosen with the bools
		int dirX = 1;
		int dirY = 1;
		if(isLeft){
			dirX = -1;
		}
		if(isDown){
			dirY = -1;
		}
		int[] rooms = {_GM.getRoomIdAt(x,y),_GM.getRoomIdAt(x+dirX,y),_GM.getRoomIdAt(x,y+dirY),_GM.getRoomIdAt(x+dirX,y+dirY)};
		bool[] isRoom = new bool[rooms.Length];
		for (int i = 0; i < isRoom.Length; i++)
		{
			isRoom[i] = rooms[i]==1;
		}
		return ((isRoom[0] && isRoom[1] && isRoom[3])||(isRoom[0] && isRoom[2] && isRoom[3]));//(_GM.PossibleSidesFromId[bottomRoom][1] && _GM.PossibleSidesFromId[currentRoomId][3]);
	}
	public void switchControl(bool isRoomLocked){
	//Set up player when switching between control
		isLockedToRoom = isRoomLocked;
		targetPos = new Vector3(Mathf.Round(transform.position.x),0.3f,Mathf.Round(transform.position.z));
		this.transform.position = targetPos;
	}
	public void setCanMove(bool b){
		canMove = b;
	}
	public bool getIsLockedToRoom(){
	//return if is locked to room
		return isLockedToRoom;
	}
	public void revealWall(){
	//Reveal walls in all cardinal direction
		float posX = transform.position.x;
		float posY = transform.position.z;
		Room currentRoom = _GM.getRoomAt(posX,posY);
		currentRoom.show(true);

		//Check every diag ajacent to the player
		if(checkDiagRoom(posX, posY, true,true)){
			Room room = _GM.getRoomAt(posX-1f,posY-1f);
			room.show(true);
		}
		if(checkDiagRoom(posX, posY, false,true)){
			Room room = _GM.getRoomAt(posX+1f,posY-1f);
			room.show(true);
		}
		if(checkDiagRoom(posX, posY, true,false)){
			Room room = _GM.getRoomAt(posX-1f,posY+1f);
			room.show(true);
		}
		if(checkDiagRoom(posX, posY, false,false)){
			Room room = _GM.getRoomAt(posX+1f,posY+1f);
			room.show(true);
		}

		//Loop until there is a wall then break
		//1 loop for every direction;
		for (int i = 0; i < DiscoveryRange; i++)
		{
			if(checkLeftRoom(posX-i, posY)){
				Room room = _GM.getRoomAt(posX-i-1f,posY);
				room.show(true);
			}else{
				break;
			}
		}
		for (int i = 0; i < DiscoveryRange; i++)
		{
				if(checkRightRoom(posX+i, posY)){
					Room room = _GM.getRoomAt(posX+i+1f,posY);
					room.show(true);
				}else{
					break;
				}
		}
		for (int i = 0; i < DiscoveryRange; i++)
		{
			if(checkUpRoom(posX, posY+i)){
				Room room = _GM.getRoomAt(posX,posY+i+1f);
				room.show(true);					
			}else{
				break;
			}
		}
		for (int i = 0; i < DiscoveryRange; i++)
		{
			if(checkBottomRoom(posX, posY-i)){
				Room room = _GM.getRoomAt(posX,posY-i-1f);
				room.show(true);		
			}else{
				break;
			}
		}
	}

	private void enableOracleLight(){
		//StartCoroutine to light up the oracle effect;
		if(oracleLightCoroutine!= null){
			StopCoroutine(oracleLightCoroutine);
		}
		oracleLightCoroutine = StartCoroutine("lightUp");
	}

	private void disableOracleLight(){
		//StartCoroutine to turn off the oracle effect;
		if(oracleLightCoroutine!= null){
			StopCoroutine(oracleLightCoroutine);
		}
		oracleLightCoroutine = StartCoroutine("lightOut");
	}
	public void disableTrail(){
		trailRenderer.enabled = false;
	}
	public void enableTrail(){
		trailRenderer.enabled = true;

	}
	public void clearTrail(){
		trailRenderer.Clear();

	}


	IEnumerator lightUp(){
		//Couroutine to turn on Oracle effect
		float target = 10f;
		float speed = 0.5f;
		while (oracleLight.intensity < target)
		{
			oracleLight.intensity += speed;
			yield return new WaitForSeconds (0.01f);
		}
		oracleLight.intensity = target;
	}

	IEnumerator lightOut(){
		//Couroutine to turn off Oracle effect
		float target = 0f;
		float speed = 0.5f;
		while (oracleLight.intensity > target)
		{
			oracleLight.intensity -= speed;
			yield return new WaitForSeconds (0.01f);
		}
		oracleLight.intensity = target;
	}

	public void checkAround(){
		//Analyse adjacent room to enable oracle effect
		if(checkAroundForEvent){
			Room[] roomsAround = new Room[4];
			float posX = transform.position.x;
			float posY = transform.position.z;
			roomsAround[0]= _GM.getRoomAt(posX-1f,posY);
			roomsAround[1]= _GM.getRoomAt(posX,posY+1f);
			roomsAround[2]= _GM.getRoomAt(posX+1f,posY);
			roomsAround[3]= _GM.getRoomAt(posX,posY-1f);
			for (int i = 0; i < roomsAround.Length; i++)
			{
				if(roomsAround[i].getEventId()!=0){
					enableOracleLight();
					return;
				}
			}
			disableOracleLight();
		}
	}

	public void addDiscoveryRange(int range){
		//Increase discovery range
		DiscoveryRange += range;
	}
	public void setCheckAroundForEvent(bool b = true){
		//Enable Oracle effect for the game
		checkAroundForEvent = b;
	}
}
