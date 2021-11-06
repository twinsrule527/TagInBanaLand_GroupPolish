using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NPCs move up-down or side-to-side
public class NPC_Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 direction;
    private int positive;//Whether the NPC is moving in a positive or negative direction
    [SerializeField] private float timeInDirection;//How long it goes in one direction
    private float curTime;
    void Start()
    {
        positive = 1;
    }

    void Update()
    {
        curTime+=Time.deltaTime;
        transform.position += direction * positive * Time.deltaTime;
        if(curTime >= timeInDirection) {
            curTime = 0;
            positive = -positive;
        }
    }
}
