using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public Transform playerTransform;
	public float speed = 1f;
	float duration = 4;
	GameMaster _GM;
	Stats _S;
	// Use this for initialization
	void Start () {
		_GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
		_S = GameObject.Find("Party stats").GetComponent<Stats>();
		playerTransform = GameObject.Find("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward*speed*Time.deltaTime);
		duration -= Time.deltaTime;
		if(duration<=0){
			Destroy(gameObject);
		}
	}
	void OnTriggerEnter(Collider col){
		if(col.name == "Player"){
			_S.reduceHp(1,true);
			Destroy(gameObject);			
		}
	}
	void OnDestroy()
    {
        _GM.removeProjectileInstance();
    }
}
