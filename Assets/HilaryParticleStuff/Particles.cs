using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public static Particles Instance;
    public GameObject dustParticles;   //this variable has to be implement in player script
    public GameObject emitSpot;   //this variable has to be implement in player script
    public GameObject star;
    public GameObject grass;
    public GameObject water;
    public GameObject slim;
    void Awake() {
        Instance = this;
    }
        
    void Start()
    {
        
    }

    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            EmitTagStars();
            EmitTagGrass();
            EmitTagWater();
            EmitTagSlim();
        }
        
    }*/
    public void EmitDust(){
       if(Time.frameCount%4==0){
            GameObject a = Instantiate(dustParticles, emitSpot.transform.position, Quaternion.identity) as GameObject;
       }
   }
   public void EmitTagStars(){
       GameObject p = Instantiate(star, emitSpot.transform.position, Quaternion.identity) as GameObject;
   }
   public void EmitTagGrass(){
       if(Time.frameCount%4==0){
       GameObject p = Instantiate(grass, emitSpot.transform.position, Quaternion.identity) as GameObject;
       }
   }
   public void EmitTagWater(){
       if(Time.frameCount%4==0){
       GameObject p = Instantiate(water, emitSpot.transform.position, Quaternion.identity) as GameObject;
       }
   }
   public void EmitTagSlim(){
       if(Time.frameCount%4==0){
       GameObject p = Instantiate(slim, emitSpot.transform.position, Quaternion.identity) as GameObject;
       }
   }

}
