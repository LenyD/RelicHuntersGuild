using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Wall {
	//string directory = "tiles/";

	// Use this for initialization
	int roomEventId;
	GameObject chest;
	GameObject alchTable;
	GameObject dummy;
	GameObject adventurer;
	public Sprite openChestSprite;
	public Sprite emptyAlchTable;
	Material _m;

	public Tile(){

	}
	void Awake () {
		float x = transform.position.x;
		float y = transform.position.z;
		_GM = FindObjectOfType<GameMaster>();
		
			
		_m = new Material(Shader.Find("Custom/Sprites/Dissolve"));
		int texRng=Random.Range(0,disolveTextures.Length);
		_m.SetFloat("_Alpha",targetAlpha);
		_m.SetColor("_Color",_r.color);
		_m.SetColor("_BurnColor",burnColor);
		_m.SetTexture("_SliceGuide",disolveTextures[texRng]);
		_m.SetTexture("_BurnRamp",disolveTextures[texRng]);
		_r.material = _m;


		name = strName+"["+x+","+y+"]";
		int NumberOfTiles = Resources.LoadAll(directory).Length;
		int rng = Random.Range(0,NumberOfTiles/2);
		Sprite s = Resources.Load<Sprite>(directory+strName+rng);
		_r.sprite = s;
		addTilesToList(x,y);
		showWalls(false);
	}

	void Start(){
		
		float x = transform.position.x;
		float y = transform.position.z;
		roomEventId = _GM.getRoomAt(x,y).getEventId();;
	}

	public override void showWalls(bool b){
		if(_r.enabled != b){
		_r.enabled = b;
			if(b){
				switch(roomEventId){
					case 4:
						chest = (GameObject)Instantiate(Resources.Load("Loot"),new Vector3(0,0,0),Quaternion.Euler(-65,0,0));
						chest.transform.SetParent(transform,false);break;
					case 6:
						alchTable = (GameObject)Instantiate(Resources.Load("AlchemyTable"),new Vector3(0,0.05f,-0.35f),Quaternion.Euler(0,0,0));
						alchTable.transform.SetParent(transform,false);break;
					case 7:
						GameObject campFire = (GameObject)Instantiate(Resources.Load("CampSite"),new Vector3(0,0f,-0.05f),Quaternion.Euler(0,0,0));
						campFire.transform.SetParent(transform,false);break;
					case 9: 
						dummy = (GameObject)Instantiate(Resources.Load("TrainingStrength"),new Vector3(0f,0f,0f),Quaternion.Euler(-65,0,0));
						dummy.transform.SetParent(transform,false);break;
					case 10: 
						dummy = (GameObject)Instantiate(Resources.Load("TrainingBlock"),new Vector3(0f,0f,0f),Quaternion.Euler(-65,0,0));
						dummy.transform.SetParent(transform,false);break;
					case 11: 
						dummy = (GameObject)Instantiate(Resources.Load("TrainingEvasion"),new Vector3(0f,0f,0f),Quaternion.Euler(-65,0,0));
						dummy.transform.SetParent(transform,false);break;
					case 12: 
						dummy = (GameObject)Instantiate(Resources.Load("TrainingAccuracy"),new Vector3(0f,0f,0f),Quaternion.Euler(-65,0,0));
						dummy.transform.SetParent(transform,false);break;
					case 8: 
						adventurer = (GameObject)Instantiate(Resources.Load("LostAdventurer"),new Vector3(0f,0f,-0.05f),Quaternion.Euler(-25,0,0));
						adventurer.transform.SetParent(transform,false);break;
				}
				this.gameObject.SetActive(true);
				StartCoroutine(FadeIn());
			}else{
				/*
				if(chest != null){
					Destroy(chest);
				}
				*/
				StartCoroutine(FadeOut());
			}
		}
		
	}
	public void openChest(){
		chest.GetComponent<SpriteRenderer>().sprite = openChestSprite;
	}
	public void useAlchTable(){
		alchTable.GetComponent<SpriteRenderer>().sprite = emptyAlchTable;
	}
	public void removeAlchTable(){
		Destroy(alchTable);
	}
	public void removeDummy(){
		Destroy(dummy);
	}
	public void removeAdventurer(){
		Destroy(adventurer);
	}
	

	void addTilesToList(float x,float y){
		_GM.addWallsToList(Mathf.FloorToInt(x),Mathf.FloorToInt(y), this);
	}
	
}
