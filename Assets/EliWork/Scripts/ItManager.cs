using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class ItManager : Singleton<ItManager>
{
    
    private PlayerControl _tagger;
    public PlayerControl Tagger {
        get {
            return _tagger;
        }
    }private List<PlayerControl> allPlayers;
    [SerializeField] private List<Color> PlayerColors;//List of the colors of players
    [SerializeField] private List<RuntimeAnimatorController> playerAnimators;

    [SerializeField] private Color TagColor;//Color of the player who is the tagger

    [SerializeField] private float startTimer;
    private float curTime;
    [SerializeField] private TMP_Text timerText;

    [Header("Scoring")]
    private float[] PlayerScores;//How many points each player has
    [SerializeField] private float scorePerSec;//How many points each player who isn't It gets per frame
    [SerializeField] private float scorePerDist;//How many points a player gets depending on how far away they are (furthest player gets 0, everyone else gets points relative to that)
    [SerializeField] private TMP_Text[] scoreText;

    private PlayerControl[] players;//An array of all players
    void Start()
    {
        StartCoroutine("StartGame");
        curTime = startTimer;
        players = FindObjectsOfType<PlayerControl>();
        PlayerScores = new float[players.Length];
    }

    void Update()
    {
        curTime -= Time.deltaTime;
        if(curTime <= 0) {
            //WHen the timer reaches zero, the game ends
            EndGame();
        }
        string timeString = (Mathf.FloorToInt(curTime / 60)).ToString() + ":";
        float secs = Mathf.FloorToInt(curTime%60);
        if(secs < 10) {
            timeString += "0" + secs.ToString();
        }
        else {
            timeString += secs.ToString();
        }
        timerText.text = timeString;
        //Calculate each player's score
        if(Tagger != null) {
            float maxDistFromIt = 0;
            foreach(PlayerControl player in players) {
                float distFromIt = (player.transform.position - Tagger.transform.position).magnitude;
                maxDistFromIt = Mathf.Max(maxDistFromIt, distFromIt);
                //Gains a static num of points if not it
                if(player != Tagger) {
                    PlayerScores[player.PlayerNumber] += scorePerSec * Time.deltaTime;
                }
            }
            foreach(PlayerControl player in players) {
                //Gains points if closer to the tagger
                    //Currently only works w/ 3+ players, because it gives points to the closest player
                if(player != Tagger) {
                    PlayerScores[player.PlayerNumber] += scorePerDist * (1 - (player.transform.position - Tagger.transform.position).magnitude/maxDistFromIt);
                }
                float displayScore = Mathf.Round(PlayerScores[player.PlayerNumber]);
                scoreText[player.PlayerNumber].text = displayScore.ToString();
            }
        }
    }

    void ChooseTagger() {
        List<PlayerInput> playersUnorder = new List<PlayerInput>(FindObjectsOfType<PlayerInput>());
        allPlayers = new List<PlayerControl>();
        //A haphazard way to get all the players, in the correct order
        foreach(PlayerInput player in playersUnorder) {
            //THIS IS A TEMPORARY MEASURE BC SOMETHING's GOING WRONG W/ THE CAMERA
            player.transform.position = Vector3.up * 20;
            if(player.currentControlScheme == "KeyboardLeft") {
                allPlayers.Add(player.GetComponent<PlayerControl>());
            }
        }
        foreach(PlayerInput player in playersUnorder) {
            if(player.currentControlScheme == "KeyboardRight") {
                allPlayers.Add(player.GetComponent<PlayerControl>());
            }
        }
        foreach(PlayerInput player in playersUnorder) {
            if(player.currentControlScheme == "Gamepad") {
                allPlayers.Add(player.GetComponent<PlayerControl>());
            }
        }
        for(int i = 0; i < allPlayers.Count; i++) {
            allPlayers[i].MySprite.color = PlayerColors[i];
            allPlayers[i].PlayerNumber = i;
            allPlayers[i].MyAnimator.runtimeAnimatorController = playerAnimators[i];
        }
        //Makes invisible all scores of players not playing
        for(int i = allPlayers.Count; i < 4; i++) {
            scoreText[i].transform.parent.gameObject.SetActive(false);
        }
        int rnd = Random.Range(0, allPlayers.Count);
        _tagger = allPlayers[rnd];
        _tagger.MySprite.color = TagColor;
    }

    public void SetTagger(GameObject taggedPlayer) {
        _tagger.newColorTag = PlayerColors[_tagger.PlayerNumber];
        _tagger.currentColorTag = _tagger.MySprite.color;
        _tagger.StartCoroutine("UnTagged");
        _tagger = taggedPlayer.GetComponentInParent<PlayerControl>();
        _tagger.newColorTag = TagColor;
        _tagger.currentColorTag = _tagger.MySprite.color;
        _tagger.StartCoroutine("WhenTagged");
    }

    private IEnumerator StartGame() {
        yield return new WaitForSeconds(1f);
        ChooseTagger();
        yield return null;
    }

    //Is called when the timer reaches 0: ends the game at the moment
    [SerializeField] private Image EndGameScreen;
    [SerializeField] private TMP_Text EndGameText;
    void EndGame() {
        int winningPlayer = 0;
        float winningScore = 0;
        foreach(PlayerControl player in allPlayers) {
            if(PlayerScores[player.PlayerNumber] > winningScore) {
                winningPlayer = player.PlayerNumber;
                winningScore = PlayerScores[player.PlayerNumber];
            }
            player.gameObject.SetActive(false);
        }
        EndGameText.text = "Player " + (winningPlayer + 1).ToString() + " won, with " + Mathf.RoundToInt(winningScore).ToString() + " points!";
        EndGameScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
