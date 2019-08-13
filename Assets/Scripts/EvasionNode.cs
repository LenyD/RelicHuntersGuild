using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvasionNode : Node
{
    // Start is called before the first frame update
    
    void Start()
    {
        effect = 25;
        image = GetComponent<Image>();
    }
    public override void applyEffect(){
        _s.addDodge(effect);
    }
    
    // Update is called once per frame
    void Update()
    {
        reduceCooldown();
    }
}
