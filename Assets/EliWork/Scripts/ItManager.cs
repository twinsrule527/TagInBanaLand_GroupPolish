using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItManager : Singleton<ItManager>
{
    
    private PlayerControl _tagger;
    public PlayerControl Tagger {
        get {
            return _tagger;
        }
    }private List<PlayerControl> allPlayers;
    [SerializeField] private List<Color> PlayerColors;//List of the colors of players

    [SerializeField] private Color TagColor;//Color of the player who is the tagger

    [SerializeField] private float startTimer;
    private float curTime;
    [SerializeField] private TMP_Text timerText;

    void Start()
    {
        StartCoroutine("StartGame");
        curTime = startTimer;
    }

    void Update()
    {
        curTime -= Time.deltaTime;
        if(curTime <= 0) {
            //WHen the timer reaches zero, the game ends
            Debug.Log("ENDGAME");
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
    }

    void ChooseTagger() {
        allPlayers = new List<PlayerControl>(FindObjectsOfType<PlayerControl>());
        for(int i = 0; i < allPlayers.Count; i++) {
            allPlayers[i].MySprite.color = PlayerColors[i];
            allPlayers[i].PlayerNumber = i;
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

}
