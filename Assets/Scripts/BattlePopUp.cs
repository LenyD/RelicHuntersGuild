using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BattlePopUp : MonoBehaviour
{
    int duration =2;
    public Image signImg;
    public Image effectImg;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject,duration);
    }
    public void setUp(Sprite[] images,Vector2 pos){
        GetComponent<RectTransform>().anchoredPosition = pos;
        signImg.sprite = images[0];
        effectImg.sprite = images[1];
    }
}
