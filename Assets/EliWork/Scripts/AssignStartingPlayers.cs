using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
public class AssignStartingPlayers : MonoBehaviour
{
    public static float numPlayersOnKeyboard = 2;//How many players are using a keyboard (upwards of 2)
    public PlayerInputManager InputManager;
    [SerializeField] private int maxPlayers;
    [SerializeField] private CinemachineTargetGroup group;
    [SerializeField] private float cameraCharacterRadius;
    [SerializeField] private float cameraCharacterWeight;
    [SerializeField] private Vector3 playerStartPos;//The starting position of the players
    void Awake()
    {
        //Sets the players on the keyboard depending on the number of keyboards set to be used
        PlayerInput newPlayer;
        switch (numPlayersOnKeyboard) {
            case 2 :
                newPlayer = CreatePlayer("KeyboardLeft", Keyboard.current);
                newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                newPlayer.transform.position = playerStartPos;
                newPlayer = CreatePlayer("KeyboardRight2", Keyboard.current);
                newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                newPlayer.transform.position = playerStartPos;
                break;
            case 1 :
                newPlayer = CreatePlayer("KeyboardLeft", Keyboard.current);
                newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                newPlayer.transform.position = playerStartPos;
                break;
            case 0 :
                break;
        }
        
        var gamepads = Gamepad.all;
        foreach(Gamepad pad in gamepads) {
            newPlayer = CreatePlayer("Gamepad", pad);
            if(newPlayer != null) {
                newPlayer.GetComponent<PlayerControl>().myInput = pad;
                newPlayer.GetComponent<PlayerControl>().myGamepad = pad;
                pad.SetMotorSpeeds(0.75f, 0.25f);
                pad.PauseHaptics();
                IEnumerator buzz = BuzzController(pad);
                StartCoroutine(buzz);
                newPlayer.transform.position = playerStartPos;
            }
            //newPlayer.GetComponent<PlayerControl>().myInput = pad;
        }
        PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl>();
        //Then, if there are fewer then 2 players, it adds keyboard players
        switch(allPlayers.Length) {
            case 0 : 
                newPlayer = CreatePlayer("KeyboardLeft", Keyboard.current);
                newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                newPlayer.transform.position = playerStartPos;
                newPlayer = CreatePlayer("KeyboardRight2", Keyboard.current);
                newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                newPlayer.transform.position = playerStartPos;
                break;
            case 1 :
                if(allPlayers[0].GetComponent<PlayerInput>().currentControlScheme == "KeyboardLeft") {
                    newPlayer = CreatePlayer("KeyboardRight2", Keyboard.current);
                    newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                    newPlayer.transform.position = playerStartPos;
                }
                else {
                    newPlayer = CreatePlayer("KeyboardLeft", Keyboard.current);
                    newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
                    newPlayer.transform.position = playerStartPos;
                }
                break;
        }
        allPlayers = FindObjectsOfType<PlayerControl>();
        foreach(PlayerControl player in allPlayers) {
            group.AddMember(player.transform, cameraCharacterWeight, cameraCharacterRadius);
        }
        //Ignores collisions between the player when they jump and any objects they can jump over
        Physics2D.IgnoreLayerCollision(3, 6, true);
    }
    PlayerInput CreatePlayer(string controlScheme = null, InputDevice newDevice = null) {
        if(InputManager.playerCount < maxPlayers) {
            return InputManager.JoinPlayer(InputManager.playerCount, InputManager.playerCount, controlScheme, newDevice);
        }
        else {
            return null;
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
            PlayerInput newPlayer = CreatePlayer("KeyboardRight2", Keyboard.current);
            newPlayer.GetComponent<PlayerControl>().myInput = Keyboard.current;
        }
    }

    void Update() {

        //Restart when R is pressed
        if(Input.GetKeyDown(KeyCode.R)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    private static float timeToBuzz = 0.25f;
    public static IEnumerator BuzzController(Gamepad pad) {
        pad.ResumeHaptics();
        yield return new WaitForSeconds(timeToBuzz);
        pad.PauseHaptics();
    }

}
