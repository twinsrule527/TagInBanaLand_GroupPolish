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
    [SerializeField] private Animator timerAnimator;//The timer animator shouldn't actually start until the game really start

    [Header("Scoring")]
    private float[] PlayerScores;//How many points each player has
    [SerializeField] private float scorePerSec;//How many points each player who isn't It gets per frame
    [SerializeField] private float scorePerDist;//How many points a player gets depending on how far away they are (furthest player gets 0, everyone else gets points relative to that)
    [SerializeField] private TMP_Text[] scoreText;

    //Icons that appear above players heads
    [SerializeField] private HeadIcon tagIcon;
    [SerializeField] private List<HeadIcon> scoreIcons;//Each score icon is a multiplier - only included w/ 3-4 players

    private PlayerControl[] players;//An array of all players

    [SerializeField] private AudioSource startSound;//A sound which plays when the game starts
    [SerializeField] private AudioSource fanfareSound;
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private float musicStartVolume;//When the music starts, it grows over time to reach its proper volume
    [SerializeField] private float musicBaseVolume;
    [SerializeField] private float musicTimeToReachVolume;
    [SerializeField] private float fanfareLength;//The length of the fanfare, in seconds
    [SerializeField] private Vector3 playerStartPos;//The starting position of the players
    
    //List of end game objects for the UI
    [SerializeField] private List<Image> endGameImages;
    [SerializeField] private List<TMP_Text> endGameScores;
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject normalCanvas;
    
    void Start()
    {
        StartCoroutine("StartGame");
        curTime = startTimer;
        players = FindObjectsOfType<PlayerControl>();
        PlayerScores = new float[players.Length];
        //Disables score multipliers
        for(int i = players.Length - 2; i < scoreIcons.Count; i++) {
            scoreIcons[i].gameObject.SetActive(false);
        }
        timerAnimator.enabled = false;
    }

    void Update()
    {
        //Calculate each player's score
        if(Tagger != null) {
            //Each player gets points - you have a score multiplier if you're closer to it
            List<PlayerControl> nonItPlayers = new List<PlayerControl>(players);
            nonItPlayers.Remove(Tagger);
            nonItPlayers = OrderByDistance(nonItPlayers, Tagger.transform.position);
            for(int i = 0; i < nonItPlayers.Count - 1; i++) {
                scoreIcons[i].ChangeParent(nonItPlayers[i].transform);
            }
            //Add/Multiply scores
            switch(nonItPlayers.Count) {
                case 1 :
                    PlayerScores[nonItPlayers[0].PlayerNumber] += scorePerSec * Time.deltaTime;
                    break;
                case 2 :
                    //Closest player gets double points
                    PlayerScores[nonItPlayers[0].PlayerNumber] += scorePerSec * Time.deltaTime * 2;
                    PlayerScores[nonItPlayers[1].PlayerNumber] += scorePerSec * Time.deltaTime;
                    break;
                case 3 :
                    PlayerScores[nonItPlayers[0].PlayerNumber] += scorePerSec * Time.deltaTime * 2;
                    PlayerScores[nonItPlayers[1].PlayerNumber] += scorePerSec * Time.deltaTime * 1.5f;
                    PlayerScores[nonItPlayers[2].PlayerNumber] += scorePerSec * Time.deltaTime;
                    break;
            }
            foreach(PlayerControl player in players) {
                float displayScore = Mathf.Round(PlayerScores[player.PlayerNumber]);
                scoreText[player.PlayerNumber].text = displayScore.ToString();
            }
            /*float maxDistFromIt = 0;
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
            }*/
        //And then subtract from the timer
        curTime -= Time.deltaTime;
        if(curTime <= 0) {
            //WHen the timer reaches zero, the game ends
            EndGame();
        }
        string timeString = (Mathf.FloorToInt(curTime).ToString());
        timerText.text = timeString;
        }
    }

    //After the beginning fanfare, chooses a tagger
    private IEnumerator ChooseTagger() {
        List<PlayerInput> playersUnorder = new List<PlayerInput>(FindObjectsOfType<PlayerInput>());
        allPlayers = new List<PlayerControl>();
        //A haphazard way to get all the players, in the correct order
        foreach(PlayerInput player in playersUnorder) {
            //THIS IS A TEMPORARY MEASURE BC SOMETHING's GOING WRONG W/ THE CAMERA
            player.transform.position = playerStartPos;
            if(player.currentControlScheme == "KeyboardLeft") {
                allPlayers.Add(player.GetComponent<PlayerControl>());
            }
        }
        foreach(PlayerInput player in playersUnorder) {
            if(player.currentControlScheme == "KeyboardRight2") {
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
        //Before tagger is chosen, a beginning fanfar occurs
        fanfareSound.Play();
        tagIcon.gameObject.SetActive(false);
        yield return new WaitForSeconds(fanfareLength);
        int rnd = Random.Range(0, allPlayers.Count);
        tagIcon.gameObject.SetActive(true);
        _tagger = allPlayers[rnd];
        _tagger.MySprite.color = TagColor;
        _tagger.MyParticles.EmitTagStars(_tagger.transform.position);
        _tagger.MyParticles.EmitTagStars(_tagger.transform.position);
        _tagger.MyParticles.EmitTagStars(_tagger.transform.position);
        tagIcon.ChangeParent(_tagger.transform);
        startSound.Play();
        mainMusic.Play();
        mainMusic.volume = musicStartVolume;
        //Sets the timer animator to work again
        timerAnimator.enabled = true;
        float curTime = 0;
        while(curTime < musicTimeToReachVolume) {
            mainMusic.volume = Mathf.Lerp(musicStartVolume, musicBaseVolume, curTime/musicTimeToReachVolume);
            curTime+=Time.deltaTime;
            yield return null;
        }
        mainMusic.volume = musicBaseVolume;
    }

    public void SetTagger(GameObject taggedPlayer) {
        _tagger.newColorTag = PlayerColors[_tagger.PlayerNumber];
        _tagger.currentColorTag = _tagger.MySprite.color;
        _tagger.StartCoroutine("UnTagged");
        _tagger = taggedPlayer.GetComponentInParent<PlayerControl>();
        _tagger.newColorTag = TagColor;
        _tagger.currentColorTag = _tagger.MySprite.color;
        _tagger.StartCoroutine("WhenTagged");
        tagIcon.ChangeParent(_tagger.transform);
        StopCoroutine("TagAppear");
        StartCoroutine("TagAppear");
    }

    [SerializeField] private Image TagWord;
    [SerializeField] private float startSize;
    [SerializeField] private float baseSize;
    [SerializeField] private float tagWordTimeToGrow;
    [SerializeField] private float tagWordTimeMaxSize;
    private IEnumerator TagAppear() {
        TagWord.gameObject.SetActive(true);
        Vector3 oneOneOne = new Vector3(1f, 1f, 1f);
        TagWord.transform.localScale = oneOneOne * startSize;
        float myTime = 0;
        while(myTime < tagWordTimeToGrow) {
            TagWord.transform.localScale = oneOneOne * Mathf.Lerp(startSize, baseSize, myTime / tagWordTimeToGrow);
            myTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(tagWordTimeMaxSize);
        TagWord.gameObject.SetActive(false);
    }

    private IEnumerator StartGame() {
        yield return null;
        StartCoroutine("ChooseTagger");
        yield return null;
    }

    //Is called when the timer reaches 0: ends the game at the moment
    [SerializeField] private Image EndGameScreen;
    [SerializeField] private TMP_Text EndGameText;
    void EndGame() {
        /*
        int winningPlayer = 0;
        float winningScore = 0;
        foreach(PlayerControl player in allPlayers) {
            if(PlayerScores[player.PlayerNumber] > winningScore) {
                winningPlayer = player.PlayerNumber;
                winningScore = PlayerScores[player.PlayerNumber];
            }
            player.gameObject.SetActive(false);
        }
        */
        normalCanvas.SetActive(false);
        endGameCanvas.SetActive(true);
        for(int i = 0; i < endGameImages.Count; i++) {
            if(i < allPlayers.Count) {
                endGameImages[i].gameObject.SetActive(true);
                endGameScores[i].gameObject.SetActive(true);
                endGameScores[i].text = Mathf.RoundToInt(PlayerScores[i]).ToString();
                allPlayers[i].gameObject.SetActive(false);
            }
            else {
                endGameImages[i].gameObject.SetActive(false);
                endGameScores[i].gameObject.SetActive(false);
            }
        }
        //EndGameText.text = "Player " + (winningPlayer + 1).ToString() + " won, with " + Mathf.RoundToInt(winningScore).ToString() + " points!";
        //EndGameScreen.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    //Reorders a list of players to be ordered by closest player to furthest player
    List<PlayerControl> OrderByDistance(List<PlayerControl> unorderedList, Vector3 pos) {
        List<PlayerControl> orderedList = new List<PlayerControl>();
        while(unorderedList.Count > 0) {
            float minDistFromTagger = 1000000;
            int num = 0;
            for(int i = 0; i < unorderedList.Count; i++) {
                float distFromTagger = (unorderedList[i].transform.position - pos).magnitude;
                if(distFromTagger < minDistFromTagger) {
                    minDistFromTagger = distFromTagger;
                    num = i;
                }
            }
            orderedList.Add(unorderedList[num]);
            unorderedList.RemoveAt(num);
        }
        return orderedList;
    }
}
