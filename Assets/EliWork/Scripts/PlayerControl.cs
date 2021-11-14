using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Controls the players inputs, using the Unity Controller System
//Attach to each Player
public class PlayerControl : MonoBehaviour
{
    public int PlayerNumber;
    public InputDevice myInput;//Declares the type of Input th
    [HideInInspector] public bool frozen;//When true, player input doesn't do anything

    //Movement-related Variables
    [Header("Movement")]
    [SerializeField] private float maxSpeed;//The fastest speed a player can run at
    [SerializeField] private float taggerMaxSpeed;//The tagger can move faster than other players
    [SerializeField] private float acceleration;//How fast the player speeds up
    private Vector2 movementInput;//Whatever input is being given throught the controller regarding player movement
    private Vector2 _curVelocity;//Player's velocity at the moment
    public Vector2 CurVelocity {
        get {
            return _curVelocity;
        }
        set {
            _curVelocity = value;
        }
    }
    private Vector2 moveSpeed;//Player's current speed
    public Vector2 MoveSpeed {
        get {
            return moveSpeed;
        }
        set {
            moveSpeed = value;
        }
    }
    private bool onDifficultTerrain;//Whether the player is on terrain that's harder to move on
    private bool onWater;//Water is extremely difficult terain
    [SerializeField] private float difficultTerrainSpeedModifier;
    [SerializeField] private float waterTerrainSpeedModifier;
    private bool onSlipperyTerrain;//WHether the player is standing on slippery terrain
    [SerializeField] private float slipFriction;//The friction multiplier when on slippery terrain
    [SerializeField] private float normalFriction;
    private float prevTaggerTime;//How long since last been tagger - used to get speed boost
    [SerializeField] private float prevTaggerSpeedBonusTime;

    
    //Jumping-related Variables
    [Header("Jumping")]

    private bool _isJumping;
    public bool IsJumping {
        get {
            return _isJumping;
        }
        set {
            _isJumping = value;
        }
    }
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float maxFallTime;//How long the player is on the ground for when they fall
    private bool tripActive;//Whether the player will trip when they hit the ground
    //These two variables below are used to determine the player's sprite's jump arc when they jump
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float jumpGravityAcc;
    [SerializeField] private float spriteBaseY;

    //tagging-related variables
    [Header("Tagging")]
    [SerializeField] private float tagDelay;
    private float tagDelayCurTime;//Currently how long until the tag resets
    [SerializeField] private float tagDist;//How far away the player can tag
    private Vector2 tagDirection;//The player will tag in the direction they most recently moved
    [SerializeField] private float tagFreezeBaseTime;
    [SerializeField] private float tagFreezeTime;//how long a player is frozen for when tagged
    [HideInInspector] public Color currentColorTag;//When the player tags or is tagged, they change color
    [HideInInspector] public Color newColorTag;

    //Throwing-related variables
    [Header("Throwing")]
    [SerializeField] private float throwDelay;//Time between throws, whether or not succesful
    [SerializeField] private float throwDelaySuccesful;//time between throws, if succesful
    private float throwDelayCurTime;//How long until the throw resets
    [SerializeField] private float thrownPlayerFreezeTime;//How long a player who is thrown can't control their movement
    [SerializeField] private float thrownPlayerUnfrozenTime;//How long a player slides back but is able to have some control
    [SerializeField] private float throwPlayerInitialSpeed;//How fast the person is initially pushed back when they're thrown
    [SerializeField] private float throwObjectInitialSpeed;//The speed at which an object is initially thrown at
    [SerializeField] private float thrownObjMassMultiplier;
    [SerializeField] private float thrownObjTime;
    [SerializeField] private float throwNPCInitialSpeed;
    [SerializeField] private float thrownNPCTime;
    private bool _isThrown;
    public bool IsThrown {
        get {
            return _isThrown;
        }
        set {
            _isThrown = value;
        }
    }
    [SerializeField] private float thrownFriction;//Amt of friction while the person is being thrown, once they hit the ground
    //General references variables
    private Rigidbody2D myBody;
    private Animator myAnimator;
    public Animator MyAnimator {
        get {
            return myAnimator;
        }
        set {
            myAnimator = value;
        }
    }
    private SpriteRenderer mySprite;
    public SpriteRenderer MySprite {
        get {
            return mySprite;
        }
    }
    private BoxCollider2D myTagCollider;
    private Particles myParticles;//The particle emitter attached to this player's child
    public Particles MyParticles {
        get {
            return myParticles;
        }
    }
    [Header("ParticleStuff")]
    [SerializeField] private float dustParticleOffset;//How much the dust particles are offset from the direction you're moving
    [SerializeField] private float starParticleYOffset;//How much above the emitSpot vertically will stars spawn
    //Audio sources attached to the player
    [Header("Audio")]
    [SerializeField] private AudioSource TagSound;
    [SerializeField] private AudioSource JumpSound;
    [SerializeField] private AudioSource FallSound;
    [SerializeField] private AudioSource ThrowSound;

    [Header("PitterPatter Audio")]//The way the pitter patter audio works is that the corresponding audio clip is switched in, and played as long as the player is moving
    [SerializeField] private AudioSource PitterPatter;//Might need to be played at a certain point, depending on how the pitter patter works
    [SerializeField] private AudioClip basePitterPatter;
    [SerializeField] private AudioClip grassPitterPatter;
    [SerializeField] private AudioClip waterPitterPatter;
    [SerializeField] private AudioClip slimePitterPatter;
    //If a gamepad player, they have a gamepad
    public Gamepad myGamepad;
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
        mySprite = GetComponentInChildren<SpriteRenderer>();
        myTagCollider = GetComponentInChildren<BoxCollider2D>();
        spriteBaseY = mySprite.transform.position.y - transform.position.y;
        myParticles = GetComponentInChildren<Particles>();
    }
    void Update()
    {
        //Player can only control their movement when not frozen
        if(!frozen) {
            //Player can only control their movement when not jumping
            if(!IsJumping && !IsThrown) {
                //If the player has changed directions or stopped moving in 1 direction, their speed in that direction is reversed
                if(!GeneralFunctions.SameSign(movementInput.x, moveSpeed.x)) {
                    if(!onSlipperyTerrain) {
                        moveSpeed.x *= normalFriction;
                    }
                    else {
                        moveSpeed.x *= slipFriction;
                    }
                }
                if(!GeneralFunctions.SameSign(movementInput.y, moveSpeed.y)) {
                    if(!onSlipperyTerrain) {
                        moveSpeed.y *= normalFriction;
                    }
                    else {
                        moveSpeed.y *= slipFriction;
                    }
                }
                //Increases the player's movement by the direction they're moving
                moveSpeed += movementInput * acceleration * Time.deltaTime;
                //The tagger can move faster than other players
                if(ItManager.Instance.Tagger == this || prevTaggerTime > 0) {
                    if(moveSpeed.magnitude > taggerMaxSpeed) {
                        moveSpeed = moveSpeed.normalized * taggerMaxSpeed;
                    }
                    if(prevTaggerTime > 0) {
                        prevTaggerTime -= Time.deltaTime;
                    }
                }
                else {
                    if(moveSpeed.magnitude > maxSpeed) {//If the player would be moving too fast, their movement is clamped
                        moveSpeed = moveSpeed.normalized * maxSpeed;
                    }
                }
                
                _curVelocity = moveSpeed;
                if(onDifficultTerrain) {
                    _curVelocity *= difficultTerrainSpeedModifier;
                    if(movementInput != Vector2.zero) {
                        myParticles.EmitTagGrass();
                    }
                }
                if(onWater) {
                    _curVelocity *= waterTerrainSpeedModifier;
                    if(movementInput != Vector2.zero) {
                        myParticles.EmitTagWater();
                    }
                }
                if(onSlipperyTerrain) {
                    if(moveSpeed != Vector2.zero) {
                        myParticles.EmitTagSlim();
                    }
                }
                //Sets their animation depending on the direction they're facing in
                if(movementInput == Vector2.zero) {
                    myAnimator.SetInteger("direction", 0);
                    //Pauses the pitterpatter sound while its playing
                    if(PitterPatter.isPlaying) {
                        PitterPatter.Pause();
                    }
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
                    //Currently, dust is emitted as long as you are moving
                    Vector3 dustSpawnPos = myParticles.transform.position - (Vector3)tagDirection * dustParticleOffset;
                    myParticles.EmitDust(dustSpawnPos);
                    //PitterPatter sound is also played as long as the player is moving
                    if(!PitterPatter.isPlaying) {
                        PitterPatter.Play();
                    }
                }
            }
            //Once the person is able to move again after having been thrown, they have only a little bit of control at first
            else if(IsThrown && !IsJumping) {
                moveSpeed *= thrownFriction;
                if(moveSpeed.magnitude < maxSpeed) {
                    moveSpeed += movementInput * acceleration * Time.deltaTime;
                }
                _curVelocity = moveSpeed;
                if(onDifficultTerrain) {
                    _curVelocity *= difficultTerrainSpeedModifier;
                }
                if(onWater) {
                    _curVelocity *= waterTerrainSpeedModifier;
                }
                if(movementInput == Vector2.zero) {
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
            //There's a delay between when you can attempt to tag/throw something
            if(tagDelayCurTime > 0) {
                tagDelayCurTime-=Time.deltaTime;
                if(tagDelayCurTime <= 0) {
                    tagDelayCurTime = 0;
                }
            }
            if(throwDelayCurTime > 0) {
                throwDelayCurTime-=Time.deltaTime;
                if(throwDelayCurTime <= 0) {
                    throwDelayCurTime = 0;
                }
            }
        }

    }
    //Fixes its depth at late update
    [Header("Other Variables")]
    [SerializeField] private float depthYOffset;
    void LateUpdate() {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + depthYOffset);
    }
    void FixedUpdate() {
        myBody.velocity = _curVelocity;
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
                if(!IsJumping) {
                    //jumpTime = maxJumpTime;
                    //myAnimator.SetBool("jumping", true);
                    JumpSound.Play();//SOUND
                    StartCoroutine("JumpCoroutine");
                }
            }
        }
    }

    //[SerializeField] private GameObject tagPress;    
    public void OnTag(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            //Can't do anything when frozen
            if(!frozen) {
                //Only can tag if their tag delay has been reset
                if(tagDelayCurTime <= 0) {
                    //It always makes the tag action - even if it fails
                    myAnimator.Play("TagState");
                    //Instantiate(tagPress, transform.position + Vector3.forward, Quaternion.identity);
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
                            //IMPORTANT: Currently the player who tags is the one who emits stars - do we want it to be the other way around?
                            //Gets a position half-way between you and the thrown player, to emit particles
                            Vector3 starSpawnPos = Vector3.Lerp(transform.position, TagBox[0].collider.gameObject.transform.parent.position, 0.75f) + Vector3.up * starParticleYOffset;
                            myParticles.EmitTagStars(starSpawnPos);
                            //Makes a tag sound
                            TagSound.Play(); //SOUND
                            ItManager.Instance.SetTagger(TagBox[0].collider.gameObject);
                            //Stays fast for a short while
                            prevTaggerTime = prevTaggerSpeedBonusTime;
                            tagDelayCurTime = tagFreezeBaseTime;
                        }
                    }

                    //Otherwise, they try to push/throw nearby objects/people
                    else {
                        if(throwDelayCurTime <= 0) {
                            throwDelayCurTime = throwDelay;
                            //finds the closest throwable object
                            List<RaycastHit2D> TagBox = new List<RaycastHit2D>(Physics2D.BoxCastAll((Vector2)transform.position, myTagCollider.size, 0, tagDirection, tagDist));
                            //Removes all non-players, non-throwables from the list
                            for(int i = TagBox.Count - 1; i >= 0; i--) {
                                if(!TagBox[i].collider.CompareTag("PlayerTag") && !TagBox[i].collider.CompareTag("Throwable") && !TagBox[i].collider.CompareTag("NPC")) {
                                    TagBox.RemoveAt(i);
                                }
                                //Also removes itself from the list
                                else if(TagBox[i].collider.gameObject.transform.parent == this.transform) {
                                    TagBox.RemoveAt(i);
                                }
                            }
                            //Then, if the list isn't empty, it attempts to throw the nearest object
                            if(TagBox.Count > 0) {
                                //IMPORTANT: Currently the player who throws is the one who emits stars - do we want it to be the other way around?
                                //Gets a position half-way between you and the thrown player, to emit particles
                                if(TagBox[0].collider.CompareTag("PlayerTag")) {
                                    Vector3 starSpawnPos = Vector3.Lerp(transform.position, TagBox[0].collider.gameObject.transform.parent.position, 0.8f) + Vector3.up * starParticleYOffset;
                                    myParticles.EmitTagStars(starSpawnPos);
                                }
                                else {
                                    Vector3 starSpawnPos = Vector3.Lerp(transform.position, TagBox[0].collider.gameObject.transform.position, 0.8f) + Vector3.up * starParticleYOffset;
                                    myParticles.EmitTagStars(starSpawnPos);
                                }
                                ThrowSound.Play();//SOUND
                                throwDelayCurTime = throwDelaySuccesful;
                                IEnumerator ThrowCoroutine;
                                if(TagBox[0].collider.CompareTag("PlayerTag")) {
                                    Vector2 throwDirection = (TagBox[0].collider.gameObject.transform.parent.position - transform.position).normalized;
                                    ThrowCoroutine = ThrowPlayer(TagBox[0].collider.gameObject.transform.parent.gameObject, throwDirection);
                                }   
                                else if(TagBox[0].collider.CompareTag("Throwable")) {
                                    Vector2 throwDirection = (TagBox[0].collider.gameObject.transform.position - transform.position).normalized;
                                    ThrowCoroutine = ThrowObject(TagBox[0].collider.gameObject.transform.parent.gameObject, throwDirection);
                                }
                                else {
                                    Vector2 throwDirection = (TagBox[0].collider.gameObject.transform.position - transform.position).normalized;
                                    ThrowCoroutine = ThrowNPC(TagBox[0].collider.gameObject.transform.parent.gameObject, throwDirection);
                                }
                                StartCoroutine(ThrowCoroutine);
                            }
                        }
                    }
                }
            }
        }
    }

    //The player can enter various triggers
    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.CompareTag("DifficultTerrain")) {
            onDifficultTerrain = true;
            PitterPatter.clip = grassPitterPatter;
        }
        else if(collider.CompareTag("TripTrigger")) {
            if(IsJumping) {
                tripActive = true;
            }
        }
        else if(collider.CompareTag("Water")) {
            onWater = true;
            PitterPatter.clip = waterPitterPatter;
        }
        else if(collider.CompareTag("Slippery")) {
            onSlipperyTerrain = true;
            PitterPatter.clip = slimePitterPatter;
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        if(collider.CompareTag("DifficultTerrain")) {
            onDifficultTerrain = false;
            //Sets their speed to be equal to whatever their speed is supposed to be when leaving the grass
            moveSpeed *= difficultTerrainSpeedModifier;
            PitterPatter.clip = basePitterPatter;
        }
        else if(collider.CompareTag("TripTrigger")) {
            tripActive = false;
        }
        else if(collider.CompareTag("Water")) {
            onWater = false;
            moveSpeed *= waterTerrainSpeedModifier;
            PitterPatter.clip = basePitterPatter;
        }
        else if(collider.CompareTag("Slippery")) {
            onSlipperyTerrain = false;
            PitterPatter.clip = basePitterPatter;
        }
    }


    //When a player is tagged, they become locked and unable to move a portion of time
    public IEnumerator WhenTagged() {
        //When tagged, if you are a gamepad user, you get some haptic feedback (trying this out)
        if(myGamepad != null) {
            IEnumerator buzz = AssignStartingPlayers.BuzzController(myGamepad);
            StartCoroutine(buzz);
        }
        _curVelocity = Vector2.zero;
        frozen = true;
        myAnimator.Play("FallState", 0);
        yield return null;
        float curTime = tagFreezeBaseTime;
        while(curTime < tagFreezeTime) {
            yield return null;
            mySprite.color = Color.Lerp(currentColorTag, newColorTag, curTime / tagFreezeTime);
            curTime += Time.deltaTime;
        }
        //Ends the haptic feedback
        /*if(myGamepad != null) {
            myGamepad.PauseHaptics();
        }*/
        mySprite.color = newColorTag;
        frozen = false;
        if(moveSpeed == Vector2.zero) {
            myAnimator.Play("IdleState");
        }
        else {
            myAnimator.Play("RunSideState");
        }
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
        _isJumping = true;
        gameObject.layer = 3;
        myAnimator.Play("JumpState", 0);
        float halfJumpTime = maxJumpTime / 2f;
        float baseJumpVel = maxJumpHeight / (halfJumpTime) - jumpGravityAcc * halfJumpTime/ 2f;
        float curTime = 0;
        float jumpYPos = spriteBaseY;
        while(curTime < maxJumpTime) {
            jumpYPos += baseJumpVel * Time.deltaTime;
            baseJumpVel += jumpGravityAcc * Time.deltaTime;
            mySprite.transform.position = new Vector3(mySprite.transform.position.x, transform.position.y + jumpYPos, mySprite.transform.position.z);
            curTime+=Time.deltaTime;
            yield return null;
        }
        mySprite.transform.position = transform.position + Vector3.up * spriteBaseY;
        gameObject.layer = 0;
        _isJumping = false;
        if(tripActive) {
            moveSpeed = Vector2.zero;
            myBody.velocity = moveSpeed;
            frozen = true;
            myAnimator.Play("FallState", 0);
            FallSound.Play();//SOUND
            yield return new WaitForSeconds(maxFallTime);
            frozen = false;
        }
        if(moveSpeed == Vector2.zero) {
            myAnimator.Play("IdleState");
        }
        else {
            myAnimator.Play("RunSideState");
        }
    }

    //Two coroutines: 1 to throw other players, and 1 to throw objects
    IEnumerator ThrowPlayer(GameObject player, Vector2 direction) {
        PlayerControl thrownControl = player.GetComponent<PlayerControl>();
        //When you throw a player, there is a short period of time where they are just pushed back unable to move
        thrownControl.CurVelocity = direction * throwPlayerInitialSpeed;
        thrownControl.myAnimator.Play("Thrown", 0);
        if(direction.x > 0) {
            thrownControl.MySprite.flipX = true;
        }
        else {
            thrownControl.mySprite.flipX = false;
        }
        thrownControl.IsJumping = true;
        thrownControl.IsThrown = true;
        player.layer = 3;//They're put on the jumping layer
        yield return new WaitForSeconds(thrownPlayerFreezeTime);
        //After that, they gain the ability to move again, but are still pushed back 
        thrownControl.IsJumping = false;
        player.layer = 0;
        thrownControl.MoveSpeed = thrownControl.CurVelocity;
        thrownControl.myAnimator.Play("IdleState", 0);
        yield return new WaitForSeconds(thrownPlayerUnfrozenTime);
        //Eventually, they gain full control again
        thrownControl.IsThrown = false;
        yield return null;
    }
    IEnumerator ThrowObject(GameObject thrownObject, Vector2 direction) {
        Rigidbody2D thrownRB = thrownObject.GetComponent<Rigidbody2D>();
        //The object is given a force to be thrown at
        thrownRB.angularVelocity = 0;
        thrownRB.velocity = direction * throwObjectInitialSpeed / thrownRB.mass;
        //The object has its mass multiplied while it's being thrown, increasing its impact w/ players/other objects
        thrownRB.mass *= thrownObjMassMultiplier;
        int thrownObjBaseLayer = thrownObject.layer;
        thrownObject.layer = 3;
        yield return new WaitForSeconds(thrownObjTime);
        thrownObject.layer = thrownObjBaseLayer;
        //if something wrong happens, it reverts to layer 0
        if(thrownObject.layer == 3) {
            thrownObject.layer = 0;
        }
        thrownRB.mass /= thrownObjMassMultiplier;
        int check = 0;
        while(thrownRB.velocity.magnitude > 1f && check < 100) {
            thrownRB.velocity *= 0.9f;
            check++;
            yield return null;
        }
    }

    IEnumerator ThrowNPC(GameObject thrownNPC, Vector2 direction) {
        Rigidbody2D thrownRB = thrownNPC.GetComponent<Rigidbody2D>();
        NPC_Movement thrownCharacter = thrownNPC.GetComponent<NPC_Movement>();
        thrownRB.velocity = direction * throwNPCInitialSpeed;
        thrownCharacter.IsThrown = true;
        thrownCharacter.MyAnimator.Play("StateThrown", 0);
        if(direction.x > 0) {
            thrownCharacter.MySprite.flipX = true;
        }
        else {
            thrownCharacter.MySprite.flipX = false;
        }
        int thrownNPCBaseLayer = thrownNPC.layer;
        thrownNPC.layer = 3;
        yield return new WaitForSeconds(thrownNPCTime);
        thrownCharacter.MyAnimator.Play("StateIdle", 0);
        if(thrownCharacter.Positive > 0) {
            thrownCharacter.MySprite.flipX = false;
        }
        else {
            thrownCharacter.MySprite.flipX = true;
        }
        thrownNPC.layer = thrownNPCBaseLayer;
        thrownCharacter.IsThrown = false;
    }

}
