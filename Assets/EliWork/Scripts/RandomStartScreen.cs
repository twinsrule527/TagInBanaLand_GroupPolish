using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomStartScreen : MonoBehaviour
{
    [SerializeField] private float chanceOfWrongText;
    [SerializeField] private List<Sprite> WrongTitles;
    [SerializeField] private Sprite RightTitle;
    [SerializeField] private Image TagTitle;
    void Start()
    {
        float rnd = Random.Range(0f, 1f);
        if(rnd < chanceOfWrongText) {
            int rnd2 = Random.Range(0, WrongTitles.Count);
            TagTitle.sprite = WrongTitles[rnd2];
        }
        else {
            TagTitle.sprite = RightTitle;
        }
    }
}

