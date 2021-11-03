using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//An icon that appears above the head of a player
public class HeadIcon : MonoBehaviour
{
    private Transform myParent;//The object which this is floating above
    [SerializeField] private float y_offset_from_parent;//How much above the player this icon appears
    private SpriteRenderer mySpriteRenderer;

    void Start() {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ChangeParent(Transform newParent)
    {
        //If being set to have no parent, it becomes invisible
        if(newParent == null) {
            mySpriteRenderer.enabled = false;
        }
        else {
            transform.parent = newParent;
            transform.position = transform.parent.position + Vector3.up * y_offset_from_parent;
            //Position is right in front of the camera
            transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z + 10);
            mySpriteRenderer.enabled = true;
        }
    }
}
