using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A class that is called once at the very beginning to sort the depth of obstacles that stick out of the ground
public class DepthSortStart : MonoBehaviour
{
    
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

}
