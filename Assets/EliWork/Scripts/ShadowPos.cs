using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Used to get the shadow of an object's position displayed accurately
public class ShadowPos : MonoBehaviour
{
    [SerializeField] private float y_offset;
    [SerializeField] private Transform characterTransform;
    void LateUpdate()
    {
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y + y_offset, characterTransform.position.z +1f);
        transform.rotation = Quaternion.identity;
    }
}
