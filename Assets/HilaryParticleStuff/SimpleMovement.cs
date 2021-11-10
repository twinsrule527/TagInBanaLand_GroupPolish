using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float speed;
   Rigidbody2D rb;
   Vector2 movement;
   public KeyCode leftKey;
   public KeyCode rightKey;
   public KeyCode upKey;
   public KeyCode downKey;
    // public GameObject dustParticles;   //this variable has to be implement in player script
    // public GameObject emitSpot;   //this variable has to be implement in player script
 
   void Start()
   {
         rb = GetComponent<Rigidbody2D>();
   }
 
    void FixedUpdate()
   {
       Vector2 velocity = rb.velocity;
       rb.velocity = new Vector2(movement.x * speed, movement.y* speed); 
 
   }
   void Update()
   {
       movement = Vector2.zero;
       if (Input.GetKey(rightKey))
       {
            movement += Vector2.right;
            Particles.Instance.EmitDust(Particles.Instance.transform.position);  
            Particles.Instance.EmitTagGrass(); 
       }
       if (Input.GetKey(leftKey))
       {
           movement += Vector2.left;
            Particles.Instance.EmitDust(Particles.Instance.transform.position); 
            Particles.Instance.EmitTagGrass(); 
       }
        if (Input.GetKey(upKey))
       {
           movement += Vector2.up;
            Particles.Instance.EmitDust(Particles.Instance.transform.position); 
            Particles.Instance.EmitTagGrass(); 
       }
        if (Input.GetKey(downKey))
       {
           movement += Vector2.down;
           Particles.Instance.EmitDust(Particles.Instance.transform.position); 
           Particles.Instance.EmitTagGrass(); 
       }
   }
//    public void EmitDust(){
//        if(Time.frameCount%4==0){
//        GameObject a = Instantiate(dustParticles, emitSpot.transform.position, Quaternion.identity) as GameObject;
//        }
//    }
}

