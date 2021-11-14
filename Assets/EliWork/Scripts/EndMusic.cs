using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Lerps the music to the right volume, so it doesn't hit you all at once
public class EndMusic : MonoBehaviour
{
    private AudioSource myMusic;
    [SerializeField] private float musicMainVolume;
    [SerializeField] private float musicStartVolume;
    [SerializeField] private float musicTimeToLerp;
    void Start()
    {
        myMusic = GetComponent<AudioSource>();
        StartCoroutine("PlayMusic");
    }

    private IEnumerator PlayMusic() {
        myMusic.Play();
        myMusic.volume = musicStartVolume;
        float curTime = 0;
        while(curTime < musicTimeToLerp) {
            myMusic.volume = Mathf.Lerp(musicStartVolume, musicMainVolume, curTime / musicTimeToLerp);
            curTime+= Time.deltaTime;
            yield return null;
        }
        myMusic.volume = musicMainVolume;
    }
}
