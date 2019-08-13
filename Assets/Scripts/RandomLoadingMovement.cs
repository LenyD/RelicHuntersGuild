using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLoadingMovement : MonoBehaviour
{
    int min = -15;
    int max = 15;
    Vector2 targetPos;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
        targetPos = rectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,targetPos,0.03f);
        if( Vector2.Distance(rectTransform.anchoredPosition,targetPos)<=1){
            targetPos = randomVector2();
        }
    }

    Vector2 randomVector2(){
        Vector2 v2 = new Vector2(rectTransform.anchoredPosition.x,Random.Range(min,max+1));
        return v2;
    }

}
