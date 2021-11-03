using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Manages the depth of all objects
public class DepthManagement : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> allObjects;
    void Start() {
        allObjects = new List<SpriteRenderer>(FindObjectsOfType<SpriteRenderer>());
        for(int i = allObjects.Count - 1; i >= 0; i--) {
            if(allObjects[i].gameObject.layer == 7 || allObjects[i].CompareTag("Ground")) {
                //allObjects[i].transform.position += new Vector3(0, 0, 100);
                allObjects.RemoveAt(i);
            }
            else if(allObjects[i].GetComponent<DepthSortStart>() != null) {
                allObjects.RemoveAt(i);
            }
        }
    }

    void Update() {
        foreach(SpriteRenderer obj in allObjects) {
            float y_offset = obj.size.y / 2f;
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.y - y_offset);
            if(obj.CompareTag("Shadow")) {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.parent.position.z + 0.1f);
            }
        }
    }
}
