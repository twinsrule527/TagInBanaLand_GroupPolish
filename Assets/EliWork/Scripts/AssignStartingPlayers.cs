using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
public class AssignStartingPlayers : MonoBehaviour
{
    public PlayerInputManager InputManager;
    [SerializeField] private int maxPlayers;
    [SerializeField] private CinemachineTargetGroup group;
    [SerializeField] private float cameraCharacterRadius;
    [SerializeField] private float cameraCharacterWeight;
    void Awake()
    {
        CreatePlayer("KeyboardLeft", Keyboard.current);
        CreatePlayer("KeyboardRight2", Keyboard.current);
        var gamepads = Gamepad.all;
        foreach(Gamepad pad in gamepads) {
            CreatePlayer("Gamepad", pad);
        }
        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        foreach(PlayerControl player in allPlayers) {
            group.AddMember(player.transform, cameraCharacterWeight, cameraCharacterRadius);
        }
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
            if(player.currentControlScheme == "KeyboardRight2") {
                secondKeyboardExists = true;
            }
        }
        if(!secondKeyboardExists) {
            CreatePlayer("KeyboardRight2", Keyboard.current);
        }
    }

    void Update() {

        //Restart when P is pressed
        if(Input.GetKeyDown(KeyCode.P)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

}
