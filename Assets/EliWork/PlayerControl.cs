using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Controls the players inputs, using the Unity Controller System
//Attach to each Player
public class PlayerControl : MonoBehaviour
{
    //Movement-related Variables
    [SerializeField] private float maxSpeed;//The fastest speed a player can run at
    [SerializeField] private float acceleration;//How fast the player speeds up
    private Vector2 movementInput;//Whatever input is being given throught the controller regarding player movement
    private Vector2 curVelocity;//Player's velocity at the moment
    private Vector2 moveSpeed;//Player's current speed
    
    //Jumping-related Variables
    private bool isJumping;
    private float jumpTime;
    [SerializeField] private float maxJumpTime;
    private Rigidbody2D myBody;

    
    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();

    }
    void Update()
    {
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
        }
        else {
            jumpTime -= Time.deltaTime;
            if(jumpTime <= 0) {
                isJumping = false;
                gameObject.layer = 0;
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
            Debug.Log("PERFORMED");
            //Only does something if the player is not currently jumping
            if(!isJumping) {
                isJumping = true;
                gameObject.layer = 3;
                jumpTime = maxJumpTime;
            }
        }
    }

    public void OnTag(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            Debug.Log("Tagged");
        }
    }


}
