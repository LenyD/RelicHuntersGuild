using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CopyTargetCameraSize : MonoBehaviour
{
    public Camera targetCamera;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize = targetCamera.orthographicSize;
    }
}
