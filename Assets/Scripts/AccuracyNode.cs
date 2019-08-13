using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccuracyNode : Node
{
    // Start is called before the first frame update
    
    void Start()
    {
        effect = 1;
        image = GetComponent<Image>();
    }
    public override void applyEffect(){
        _m.receiveDamage(effect,effect);
    }
    
    // Update is called once per frame
    void Update()
    {
        reduceCooldown();
    }
}
