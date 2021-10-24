using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AssignStartingPlayers : MonoBehaviour
{
    public PlayerInputManager InputManager;
    [SerializeField] private int maxPlayers;
    void Awake()
    {
        CreatePlayer("KeyboardLeft", Keyboard.current);
        CreatePlayer("KeyboardRight", Keyboard.current);
        var gamepads = Gamepad.all;
        foreach(Gamepad pad in gamepads) {
            CreatePlayer("Gamepad", pad);
        }
        Debug.Log(gamepads);
        //Ignores collisions between the player when they jump and any objects they can jump over
        Physics2D.IgnoreLayerCollision(3, 6, true);
    }
    void CreatePlayer(string controlScheme = null, InputDevice newDevice = null) {
        if(InputManager.playerCount < maxPlayers) {
            InputManager.JoinPlayer(InputManager.playerCount, InputManager.playerCount, controlScheme, newDevice);
        }
    }

    public void JoinSecondKeyboard(PlayerInput joinedPlayer) {
        List<PlayerInput> currentPlayers = new List<PlayerInput>(FindObjectsOfType<PlayerInput>());
        bool secondKeyboardExists = false;
        foreach(PlayerInput player in currentPlayers) {
            if(player.currentControlScheme == "KeyboardRight") {
                secondKeyboardExists = true;
            }
        }
        if(!secondKeyboardExists) {
            CreatePlayer("KeyboardRight", Keyboard.current);
        }
    }

}
