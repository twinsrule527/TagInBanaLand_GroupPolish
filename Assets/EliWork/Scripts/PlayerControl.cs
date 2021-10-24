using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Controls the players inputs, using the Unity Controller System
//Attach to each Player
public class PlayerControl : MonoBehaviour
{
    public int PlayerNumber;
    private bool frozen;//When true, player input doesn't do anything

    //Movement-related Variables
    [SerializeField] private float maxSpeed;//The fastest speed a player can run at
    [SerializeField] private float acceleration;//How fast the player speeds up
    private Vector2 movementInput;//Whatever input is being given throught the controller regarding player movement
    private Vector2 curVelocity;//Player's velocity at the moment
    private Vector2 moveSpeed;//Player's current speed
    private bool onDifficultTerrain;//Whether the player is on terrain that's harder to move on
    [SerializeField] private float difficultTerrainSpeedModifier;
    
    //Jumping-related Variables
    private bool isJumping;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float maxFallTime;//How long the player is on the ground for when they fall
    private bool tripActive;//Whether the player will trip when they hit the ground
    //These two variables below are used to determine the player's sprite's jump arc when they jump
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float jumpGravityAcc;

    //General references variables
    private Rigidbody2D myBody;
    private Animator myAnimator;
    private SpriteRenderer mySprite;
    public SpriteRenderer MySprite {
        get {
            return mySprite;
        }
    }
    private BoxCollider2D myTagCollider;

    //tagging-related variables
    [SerializeField] private float tagDelay;
    private float tagDelayCurTime;//Currently how long until the tag resets
    [SerializeField] private float tagDist;//How far away the player can tag
    private Vector2 tagDirection;//The player will tag in the direction they most recently moved
    [SerializeField] private float tagFreezeBaseTime;
    [SerializeField] private float tagFreezeTime;//how long a player is frozen for when tagged
    [HideInInspector] public Color currentColorTag;//When the player tags or is tagged, they change color
    [HideInInspector] public Color newColorTag;
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        Debug.Log(myAnimator);
        mySprite = GetComponentInChildren<SpriteRenderer>();
        myTagCollider = GetComponentInChildren<BoxCollider2D>();

    }
    void Update()
    {
        //Player can only control their movement when not frozen
        if(!frozen) {
            //Player can only control their movement when not jumping
            if(!isJumping) {
                //If the player has changed directions or stopped moving in 1 direction, their speed in that direction is reversed
                if(!GeneralFunctions.SameSign(movementInput.x, moveSpeed.x)) {
                    moveSpeed.x = 0;
                }
                if(!GeneralFunctions.SameSign(movementInput.y, moveSpeed.y)) {
                    moveSpeed.y = 0;
                }
                //Increases the player's movement by the direction they're moving
                moveSpeed += movementInput * acceleration * Time.deltaTime;
                if(moveSpeed.magnitude > maxSpeed) {//If the player would be moving too fast, their movement is clamped
                    moveSpeed = moveSpeed.normalized * maxSpeed;
                }
                
                curVelocity = moveSpeed;
                if(onDifficultTerrain) {
                    curVelocity *= difficultTerrainSpeedModifier;
                }
                //Sets their animation depending on the direction they're facing in
                if(moveSpeed == Vector2.zero) {
                    myAnimator.SetInteger("direction", 0);
                }
                else {
                    //If the player is moving, their tagDirection becomes the direction they're pointing in
                    tagDirection = moveSpeed.normalized;
                    if(moveSpeed.x >= 0) {
                        myAnimator.SetInteger("direction", 1);
                        mySprite.flipX = false;
                    }
                    else if(moveSpeed.x < 0) {
                        myAnimator.SetInteger("direction", -1);
                        mySprite.flipX = true;
                    }
                }
            }
            else {
                //while jumping, can't move in any direction
                /*jumpTime -= Time.deltaTime;
                if(jumpTime <= 0) {
                    isJumping = false;
                    gameObject.layer = 0;
                    myAnimator.SetBool("jumping", false);
                    if(moveSpeed == Vector2.zero) {
                        myAnimator.SetInteger("direction", 0);
                    }
                    else {
                        tagDirection = moveSpeed.normalized;
                        if(moveSpeed.x >= 0) {
                            myAnimator.SetInteger("direction", 1);
                            mySprite.flipX = false;
                        }
                        else if(moveSpeed.x < 0) {
                            myAnimator.SetInteger("direction", -1);
                            mySprite.flipX = true;
                        }
                    }
                    
                }*/
            }
            //There's a delay between when you can attempt to tag/throw something
            if(tagDelayCurTime > 0) {
                tagDelayCurTime-=Time.deltaTime;
                if(tagDelayCurTime <= 0) {
                    tagDelayCurTime = 0;
                }
            }
        }

    }

    void FixedUpdate() {
        myBody.velocity = curVelocity;
    }

    //When the player moves their controller, this records the movement
    public void OnMove(InputAction.CallbackContext ctx) { 
        movementInput = ctx.ReadValue<Vector2>();
    }

    //When the player jumps, this records it so that the jump works
    public void OnJump(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            //Can only jump if not frozen
            if(!frozen) {
                //Only does something if the player is not currently jumping
                if(!isJumping) {
                    //jumpTime = maxJumpTime;
                    //myAnimator.SetBool("jumping", true);
                    StartCoroutine("JumpCoroutine");
                }
            }
        }
    }

    public void OnTag(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            //Can't do anything when frozen
            if(!frozen) {
                //Only can tag if their tag delay has been reset
                if(tagDelayCurTime <= 0) {
                    //If this character is the current tagger, they try to tag someone in front of them
                    if(ItManager.Instance.Tagger == this) {
                        tagDelayCurTime = tagDelay;
                        //Creates an overlap box with all possible characters you could tag
                        List<RaycastHit2D> TagBox = new List<RaycastHit2D>(Physics2D.BoxCastAll((Vector2)transform.position, myTagCollider.size, 0, tagDirection, tagDist));
                        //Removes all non-players from the list
                        for(int i = TagBox.Count - 1; i >= 0; i--) {
                            if(!TagBox[i].collider.CompareTag("PlayerTag")) {
                                TagBox.RemoveAt(i);
                            }
                            //Also removes itself from the list
                            else if(TagBox[i].collider.gameObject.transform.parent == this.transform) {
                                TagBox.RemoveAt(i);
                            }
                        }
                        //Then, if the tagbox still has a player in it, it chooses the closest one to tag
                        if(TagBox.Count > 0) {
                            ItManager.Instance.SetTagger(TagBox[0].collider.gameObject);
                        }
                    }

                    //Otherwise, they try to push/throw nearby objects/people
                }
            }
        }
    }

    //The player can enter various triggers
    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("DifficultTerrain")) {
            onDifficultTerrain = true;
        }
        else if(collider.CompareTag("TripTrigger")) {
            if(isJumping) {
                tripActive = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if(collider.CompareTag("DifficultTerrain")) {
            onDifficultTerrain = false;
            //Sets their speed to be equal to whatever their speed is supposed to be when leaving the grass
            moveSpeed *= difficultTerrainSpeedModifier;
        }
        else if(collider.CompareTag("TripTrigger")) {
            tripActive = false;
        }
    }


    //When a player is tagged, they become locked and unable to move a portion of time
    public IEnumerator WhenTagged() {
        curVelocity = Vector2.zero;
        frozen = true;
        yield return null;
        float curTime = tagFreezeBaseTime;
        while(curTime < tagFreezeTime) {
            yield return null;
            mySprite.color = Color.Lerp(currentColorTag, newColorTag, curTime / tagFreezeTime);
            curTime += Time.deltaTime;
        }
        mySprite.color = newColorTag;
        frozen = false;
    }

    //When a player tags another, their color changes back
    public IEnumerator UnTagged() {
        float curTime = tagFreezeBaseTime;
        yield return null;
        while(curTime < tagFreezeTime) {
            yield return null;
            mySprite.color = Color.Lerp(currentColorTag, newColorTag, curTime / tagFreezeTime);
            curTime += Time.deltaTime;
        }
    }

    //Coroutine for the player jumping
    public IEnumerator JumpCoroutine() {
        isJumping = true;
        gameObject.layer = 3;
        myAnimator.Play("JumpState", 0);
        float halfJumpTime = maxJumpTime / 2f;
        float baseJumpVel = maxJumpHeight / (halfJumpTime) - jumpGravityAcc * halfJumpTime/ 2f;
        Debug.Log(baseJumpVel);
        float curTime = 0;
        float jumpYPos = 0;
        while(curTime < maxJumpTime) {
            jumpYPos += baseJumpVel * Time.deltaTime;
            baseJumpVel += jumpGravityAcc * Time.deltaTime;
            mySprite.transform.position = new Vector3(mySprite.transform.position.x, transform.position.y + jumpYPos, mySprite.transform.position.z);
            curTime+=Time.deltaTime;
            yield return null;
        }
        mySprite.transform.position = transform.position;
        gameObject.layer = 0;
        isJumping = false;
        if(tripActive) {
            moveSpeed = Vector2.zero;
            myBody.velocity = moveSpeed;
            frozen = true;
            myAnimator.Play("FallState", 0);
            yield return new WaitForSeconds(maxFallTime);
            frozen = false;
        }
        if(moveSpeed == Vector2.zero) {
            myAnimator.Play("BaoIdle");
        }
        else {
            myAnimator.Play("BaoRunSide");
        }
    }

}
