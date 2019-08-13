using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseNode : Node
{
    // Start is called before the first frame update
    
    void Start()
    {
        effect = 2;
        image = GetComponent<Image>();
    }
    public override void applyEffect(){
        _s.addBlock(effect);
    }
    
    // Update is called once per frame
    void Update()
    {
        reduceCooldown();
    }
}
