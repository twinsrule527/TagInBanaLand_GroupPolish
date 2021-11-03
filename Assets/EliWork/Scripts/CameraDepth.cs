using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraDepth : MonoBehaviour
{
    private CinemachineBrain brain;

    void Start() {
        brain = GetComponent<CinemachineBrain>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        brain.ManualUpdate();
        transform.position += Vector3.back * 100f;
    }
}
