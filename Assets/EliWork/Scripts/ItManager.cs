using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        StartCoroutine("StartGame");
    }

    void Update()
    {
        
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
