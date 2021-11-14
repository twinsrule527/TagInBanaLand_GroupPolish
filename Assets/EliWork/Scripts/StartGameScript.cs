using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//Attach to an empty manager object in the start room
//Starts the game when the correct key is pressed
//Also, helps with determining how many players on keyboard there are
public class StartGameScript : MonoBehaviour
{
    private float numOnKeyboard = 2;//How many players are on the keyboard
    [SerializeField] private KeyCode startKey;
    [SerializeField] private List<Button> keyboardButtons;
    void Update()
    {
        if(Input.GetKeyDown(startKey)) {
            AssignStartingPlayers.numPlayersOnKeyboard = numOnKeyboard;
            SceneManager.LoadScene(1);
        }
        //Keyboard numbers can also be set by pressing Number keys
        if(Input.GetKeyDown(KeyCode.Alpha0)) {
            keyboardButtons[0].interactable = false;
            keyboardButtons[1].interactable = true;
            keyboardButtons[2].interactable = true;
            SetKeyboardNum(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            keyboardButtons[0].interactable = true;
            keyboardButtons[1].interactable = false;
            keyboardButtons[2].interactable = true;
            SetKeyboardNum(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            keyboardButtons[0].interactable = true;
            keyboardButtons[1].interactable = true;
            keyboardButtons[2].interactable = false;
            SetKeyboardNum(2);
        }
    }

    public void SetKeyboardNum(int num) {
        numOnKeyboard = num;
    }
}
