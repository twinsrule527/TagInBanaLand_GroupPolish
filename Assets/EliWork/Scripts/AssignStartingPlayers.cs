using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AssignStartingPlayers : MonoBehaviour
{
    public PlayerInputManager InputManager;
    void Awake()
    {
        InputManager.JoinPlayer(0, 0, "KeyboardLeft", Keyboard.current);
        InputManager.JoinPlayer(1, 1, "KeyboardRight", Keyboard.current);

        //Ignores collisions between the player when they jump and any objects they can jump over
        Physics2D.IgnoreLayerCollision(3, 6, true);
    }

}
