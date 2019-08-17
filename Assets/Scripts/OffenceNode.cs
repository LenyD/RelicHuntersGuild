using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffenceNode : Node
{
    // Start is called before the first frame update
    
    void Start()
    {
        effect = 3;
        image = GetComponent<Image>();
    }
    public override void applyEffect(){
        //Deal normal damage
        _m.receiveDamage(effect,0);
    }
    
    // Update is called once per frame
    void Update()
    {
        reduceCooldown();
    }
}
