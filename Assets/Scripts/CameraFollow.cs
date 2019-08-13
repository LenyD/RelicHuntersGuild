using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 offset;
    public float speed = 8;
    public float rotSpeed = 0.1f;
    GameObject target;
    // Start is called before the first frame update
    void Awake(){
        offset = transform.position;
    }
    void Start()
    {
        target = GameObject.Find("Player");
        transform.position = target.transform.position;
        //transform.LookAt(target.transform);
        //this.GetComponent<Camera>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion q1 = transform.rotation;
        transform.LookAt(target.transform);
        Quaternion q2 = transform.rotation;
        transform.rotation = q1;
        transform.rotation = Quaternion.Slerp(q1,q2,rotSpeed*Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset,speed*Time.deltaTime);
    }
}
