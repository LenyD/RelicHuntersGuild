using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSpoiler : MonoBehaviour
{

    GameMaster _GM;
    public Material trapMat;
    public Material monsterMat;
    public Material curseMat;
    Light _l;
    MeshRenderer _r;
    // Start is called before the first frame update
    void Start()
    {
        //Get nescessary data
		_GM = FindObjectOfType<GameMaster>();
        _r = GetComponent<MeshRenderer>();
        _l = GetComponent<Light>();
        Room room = _GM.getRoomAt(transform.position.x,transform.position.z);
        setMat(room.getEventId());    
    }

    void setMat(int id){
        //Set color of the light & sphere
        switch(id){
			case 1:_r.material = trapMat;break;
			case 2:_r.material = monsterMat;break;
			case 3:_r.material = curseMat;break;
		}
        _l.color = _r.material.color;
    }
}
