using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Manages the depth of all objects
public class DepthManagement : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> allObjects;
    void Start() {
        allObjects = new List<SpriteRenderer>(FindObjectsOfType<SpriteRenderer>());
    }

    void Update() {
        foreach(SpriteRenderer obj in allObjects) {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.y);
        }
    }
}
