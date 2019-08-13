using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public string strName;
	public SpriteRenderer _r;
	Material _m;
	public Texture[] disolveTextures;
	public GameMaster _GM;
	public Color burnColor;
	public string directory = "WallSprites/";
	public GameObject torch;
	public Transform torchSpawnTransform;
	int chanceToSpawnTorch = 5;
	public float targetAlpha = 1f;
	void Awake(){
		_GM = FindObjectOfType<GameMaster>();

		_m = new Material(Shader.Find("Custom/Sprites/Dissolve"));
		int texRng=Random.Range(0,disolveTextures.Length);
		_m.SetFloat("_Alpha",targetAlpha);
		_m.SetColor("_Color",_r.color);
		_m.SetColor("_BurnColor",burnColor);
		_m.SetTexture("_SliceGuide",disolveTextures[texRng]);
		_m.SetTexture("_BurnRamp",disolveTextures[texRng]);
		_r.material = _m;
		
		//int NumberOfWalls = Resources.LoadAll("walls").Length;
		//int wallRng = Random.Range(0,NumberOfWalls);
	}

	void Start () {
		float x = transform.position.x;
		float y = transform.position.z;	
		name = strName+"["+x+","+y+"]";
		int NumberOfTiles = Resources.LoadAll(directory).Length;
		int rng = Random.Range(0,NumberOfTiles/2);
		Sprite s = Resources.Load<Sprite>(directory+strName+rng);
		_r.sprite = s;
		if(torch!=null){
			if(Random.Range(0,100)<chanceToSpawnTorch){
				torch = Instantiate(torch,new Vector3(),new Quaternion());
				torch.transform.SetParent(torchSpawnTransform,false);
			}
		}
		addWallsToList(x,y);
		showWalls(false);
	}
	

	void addWallsToList(float x,float y){
		_GM.addWallsToList(Mathf.FloorToInt(x),Mathf.FloorToInt(y), this);
		_GM.addWallsToList(Mathf.CeilToInt(x),Mathf.CeilToInt(y), this);
	}
	
	public virtual void showWalls(bool b){
		if(_r.enabled != b){
		_r.enabled = b;
			if(!b){
				StartCoroutine(FadeOut());

			}else{
				this.gameObject.SetActive(true);
				StartCoroutine(FadeIn());
			}
		}
	}

	public IEnumerator FadeOut(){
			//_r.color= new Color(c.r,c.g,c.b,i);
			_r.material.SetFloat("_SliceAmount",1f);
			yield return new WaitForSeconds (0.01f);
		this.gameObject.SetActive(false);
	}
	public IEnumerator FadeIn(){
		while(_r.material.GetFloat("_SliceAmount")>0){
			_r.material.SetFloat("_SliceAmount",_r.material.GetFloat("_SliceAmount")-0.04f);
			yield return new WaitForSeconds (0.01f);
		}
		
		/*
		for (float i = _r.material.GetFloat("_Threshold"); i > 0; i -= 0.05f)
		{
			//_r.color = new Color(c.r,c.g,c.b,i);
			_r.material.SetFloat("_Threshold",i);
			yield return new WaitForSeconds (0.01f);
		}
		 */
		
	}
	
}
