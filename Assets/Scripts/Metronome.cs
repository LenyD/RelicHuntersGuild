using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    float rotationLimit = 90f;
    public float speed = 1.5f;
    float currentAngle = 0;
    float damageScaler = 1f;
    float cooldownLength = 0.5f;
    float cooldown = 0f;
    Color onCDColor = new Color(.35f,.35f,.35f,1f);
    Color offCDColor = new Color(1f,1f,1f,1f);
    public Image image;

    public Node[] nodes;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void init(){
        currentAngle=0;
        transform.rotation = Quaternion.Euler(0,0,currentAngle);
        image.color = onCDColor;
    }    
    public void startCombat(){
        enabled=true;
    }
    public void endCombat(){
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        cooldown -= Time.deltaTime;
        currentAngle =Mathf.Sin(Mathf.PI/2 * speed*Time.time)*rotationLimit;
        transform.rotation = Quaternion.Euler(0,0,currentAngle);
        if(cooldown>0){
            image.color = onCDColor;
        }else{
            image.color = offCDColor;
        }
        
        if(Input.GetButtonDown("Attack")){
            buttonPressed(0,currentAngle);
        }
        if(Input.GetButtonDown("Block")){
            buttonPressed(1,currentAngle);
        }
        if(Input.GetButtonDown("Evade")){
            buttonPressed(2,currentAngle);
        }
        if(Input.GetButtonDown("Shoot")){
            buttonPressed(3,currentAngle);
        }
    }
    void buttonPressed(int nodeId,float angle){
        if(cooldown<=0f){
            //Reduce current angle between 0-9 instead of -90 - 90;
            angle = Mathf.Sqrt(angle*angle)/10;
            if(nodes[nodeId].increaseSize(((damageScaler) / (angle+(damageScaler)/2)))){
                cooldown = cooldownLength;
            }
        }
    }
    public void setDamageScale(float number=1){
        damageScaler = number;
    }
    public void setSpeed(float number = 1){
        speed = number;
    }
}

