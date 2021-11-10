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
    private SpriteRenderer mySprite;
    private Animator myAnimator;
    [SerializeField] private float y_depth_offset;//How much its depth should be offset from its position (same as that of players)
    private Rigidbody2D myRB;
    public bool IsThrown;
    void Start()
    {
        positive = 1;
        mySprite = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("moving", true);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + y_depth_offset);
        myRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!IsThrown) {
            curTime+=Time.deltaTime;
            myRB.velocity= direction * positive* speed;
            if(curTime >= timeInDirection) {
                curTime = 0;
                positive = -positive;
                if(direction.x * positive > 0) {
                    mySprite.flipX = false;
                }
                else {
                    mySprite.flipX = true;
                }
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
    }
}
