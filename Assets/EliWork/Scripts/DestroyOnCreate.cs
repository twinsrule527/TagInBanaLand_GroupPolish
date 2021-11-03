using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Destroys this object a couple of frames after creation
public class DestroyOnCreate : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    private float curTime;
    void Start()
    {
        curTime = timeToDestroy;
    }

    void Update()
    {
        curTime-=Time.deltaTime;
        if(curTime < 0) {
            Destroy(gameObject);
        }
    }
}
