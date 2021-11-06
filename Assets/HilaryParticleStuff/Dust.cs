using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//the dust particles and other particles
public class Dust : MonoBehaviour
{
    //public GameObject Dust;  //the prefab
    //public static Particles Instance;
    Animator dustAnim;
    void Awake(){
        //Instance = this;
    }
    void Start()
    {
        dustAnim = this.GetComponent<Animator>();
        //play animation
    }

    void Update()
    {
        if(this.gameObject.tag=="Dust"){
            Destroy(this.gameObject,.5f);
        }
        if(this.gameObject.tag=="OtherParticles"){
            Destroy(this.gameObject,1f);
        }
    }
//     public void EmitDust(){
//        if(Time.frameCount%4==0){
//        GameObject a = Instantiate(dustParticles, emitSpot.transform.position, Quaternion.identity) as GameObject;
//        }
//    }
}
