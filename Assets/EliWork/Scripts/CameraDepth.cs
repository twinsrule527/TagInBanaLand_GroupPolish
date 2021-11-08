using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraDepth : MonoBehaviour
{
    private CinemachineBrain brain;
    private Camera myCamera;
    //These variables are a test to see if I can cap the size of the screen 
    [SerializeField] private float maxCameraSize;
    [SerializeField] private Vector2 cameraOrigin;
    void Start() {
        brain = GetComponent<CinemachineBrain>();
        myCamera = GetComponent<Camera>();
    }
    
    void LateUpdate()
    {
        brain.ManualUpdate();
        transform.position += Vector3.back * 100f;
        //Possible implementation of limiting the max camera size
        if(myCamera.orthographicSize >= maxCameraSize) {
            myCamera.orthographicSize = maxCameraSize;
            transform.position = new Vector3(cameraOrigin.x, cameraOrigin.y, transform.position.z);
        }
    }
}
