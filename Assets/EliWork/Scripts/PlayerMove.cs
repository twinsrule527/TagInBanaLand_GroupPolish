
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public bool player1;
    int dir;
    int chaseTime;
    int musicTime = 3;
    int musicMaxTime = 8;
    bool musicing;
    bool switching;
    public bool chaser;
    public GameObject otherPlayer;
    //public GameObject rendObj;
    SpriteRenderer sprRenderer;
    private Vector3 selfPos;
    private Rigidbody2D selfBody;
    private Vector3 currVelocity;
    private float moveSpdMax = 2.8f; //Add soft speed 
    private float moveAcc = 0.3f;
    private float moveHSpd;
    private float moveVSpd;
    public bool lockMovement = false;
    float moveSmoothRate = 0.1f;
    bool onGrass;
    bool onFence;
    public bool jumping;
    public bool floating;
    bool tripping;
    PlayerInput myInput;
    float waitTime = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        selfBody = gameObject.GetComponent<Rigidbody2D>();
        selfPos = gameObject.transform.position;
        sprRenderer = GetComponent<SpriteRenderer>();
        currVelocity = selfBody.velocity;
        if (player1)
        {
            StartCoroutine(ChaseMusic());
        }
        myInput = GetComponent<PlayerInput>();
    }

    IEnumerator ChaseMusic()
    {

        
        yield return new WaitForSeconds(musicTime);
        musicing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!chaser && !switching)
        {
        }
        if (!lockMovement)
        {
            if (player1)
            {
                /*
                if (!jumping)
                {
                    SetAniDirection();
                    if (Input.GetKey(KeyCode.W))
                    {
                        if (moveVSpd < moveSpdMax)
                            moveVSpd += moveAcc;
                        //spriteMan.SetDir(1);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        if (moveVSpd > -moveSpdMax)
                            moveVSpd -= moveAcc;
                        //spriteMan.SetDir(2);
                    }
                    else
                    {
                        moveVSpd = 0;
                    }
                    if (Input.GetKey(KeyCode.A))
                    {
                        if (moveHSpd > -moveSpdMax)
                            moveHSpd -= moveAcc;
                        //spriteMan.SetDir(3);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        if (moveHSpd < moveSpdMax)
                            moveHSpd += moveAcc;
                        //spriteMan.SetDir(3);
                    }
                    else
                    {
                        moveHSpd = 0;
                    }
                    SetAniDirection();
                }
                currVelocity.x = moveHSpd;
                currVelocity.y = moveVSpd;
                if (currVelocity.x < 0)
                    transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
                else
                    transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);

                if (!jumping)
                {
                    if (Input.GetKeyDown(KeyCode.Space))//LeftAlt)))
                    {
                        //floating = true;
                        gameObject.layer = 6;
                        
                        jumping = true;
                    }
                    else if (currVelocity.x != 0 || currVelocity.y != 0)
                    {
                        
                    }
                    else
                    {
                    }
                }
                */
            }
            else
            {
                if (!jumping)
                {
                    SetAniDirection();
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        if (moveVSpd < moveSpdMax)
                            moveVSpd += moveAcc;
                        //spriteMan.SetDir(1);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow))
                    {
                        if (moveVSpd > -moveSpdMax)
                            moveVSpd -= moveAcc;
                        //spriteMan.SetDir(2);
                    }
                    else
                    {
                        moveVSpd = 0;
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        if (moveHSpd > -moveSpdMax)
                            moveHSpd -= moveAcc;
                        //spriteMan.SetDir(3);
                    }
                    else if (Input.GetKey(KeyCode.RightArrow))
                    {
                        if (moveHSpd < moveSpdMax)
                            moveHSpd += moveAcc;
                        //spriteMan.SetDir(3);
                    }
                    else
                    {
                        moveHSpd = 0;
                    }
                    SetAniDirection();
                }
                currVelocity.x = moveHSpd;
                currVelocity.y = moveVSpd;
                if (currVelocity.x < 0)
                    transform.localScale = new Vector3(-1.0f, transform.localScale.y, transform.localScale.z);
                else
                    transform.localScale = new Vector3(1.0f, transform.localScale.y, transform.localScale.z);

                if (!jumping)
                {
                    if (Input.GetKeyDown(KeyCode.RightShift))
                    {
                        gameObject.layer = 6;
                        jumping = true;
                    }
                    else if (currVelocity.x != 0 || currVelocity.y != 0)
                    {
                        if (Mathf.Abs(moveHSpd) > Mathf.Abs(moveVSpd))
                        {
                        }
                    }
                }
            }
            if (chaser)
            {
                currVelocity.x *= 1.1f;
                currVelocity.y *= 1.1f;
            }
            if (onGrass&&!jumping)
            {
                currVelocity.x *= 0.65f;
                currVelocity.y *= 0.65f;
            }
        }
        HandleMovement();
        /*
        if (Input.GetKeyUp(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            */
    }
    void HandleMovement()
    {
        //selfBody.velocity = Vector3.Lerp(selfBody.velocity, currVelocity, moveSmoothRate);
        selfBody.velocity = currVelocity;
        currVelocity = new Vector3(0, 0, 0);
    }
    void SetAniDirection()
    {
        if (player1)
        {/*
            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
                    dir = 3;
                else
                    dir = 1;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
                dir = 3;
            else if (Input.GetKey(KeyCode.S))
                dir = 2;
            else if (Input.GetKey(KeyCode.W))
                dir = 1;
            else
                dir = 0;
            
        */
        }
        else 
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                    dir = 3;
                else
                    dir = 1;
            }
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                dir = 3;
            else if (Input.GetKey(KeyCode.DownArrow))
                dir = 2;
            else if (Input.GetKey(KeyCode.UpArrow))
                dir = 1;
            else
                dir = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (chaser && other.tag == "Catch")
        {
            otherPlayer.transform.GetChild(1).SetParent(transform);
            transform.GetChild(1).gameObject.SetActive(false);
            StartCoroutine(RunAway());
            otherPlayer.GetComponent<PlayerMove>().lockMovement = true;
            otherPlayer.GetComponent<PlayerMove>().chaser = true;
            chaser = false;
        }
        if (other.tag == "Grass")
        {
            onGrass = true;
        }
    }

    public bool LandTripping()
    {
        jumping = false;
        gameObject.layer = 0;
        if (onFence)
            return true;
        return false;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Grass")
        {
            onGrass = true;
        }
        if (other.tag == "Fence")
        {
            onFence = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Grass")
        {
            onGrass = false;
        }
        if (other.tag == "Fence")
        {
            onFence = false;
        }
    }
    IEnumerator RunAway()
    {
        if (player1)
        {
            StopCoroutine(ChaseMusic());
            
            musicing = false;
        }
        switching = true;
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(waitTime);
        if (player1) { StartCoroutine(ChaseMusic()); }
        otherPlayer.GetComponent<PlayerMove>().lockMovement = false;
        switching = false;
        otherPlayer.GetComponent<PlayerMove>().sprRenderer.color = new Color(1.0f, 0.7f, 0.7f);
        sprRenderer.color = Color.white;
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(1).position = transform.position;
    }
    bool up;
    public void OnUp() {
        up = true;
        Debug.Log("UP");
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMove : MonoBehaviour
{
    public bool player1;
    public TextMeshProUGUI scoreText;
    float score;
    bool switching;

    public CameraManager cameraMan;

    public bool chaser;
    public GameObject otherPlayer;
    public SpriteRenderer sprRenderer;
    private Vector3 selfPos;
    private Rigidbody selfBody;
    private Vector3 currVelocity;
    private float moveSpd = 1.8f; //Add soft speed 
    public bool lockMovement = false;
    float moveSmoothRate = 0.3f;
    bool onGrass;
    bool jumping;
    bool tripping;

    float waitTime = 1.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        selfBody = gameObject.GetComponent<Rigidbody>();
        selfPos = gameObject.transform.position;
        sprRenderer = gameObject.GetComponent<SpriteRenderer>();
        currVelocity = selfBody.velocity;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
            currVelocity.y = 1.0f * moveSpd;
        if (Input.GetKey(KeyCode.S))


            if (!chaser&&!switching)
        {
            score+=Vector3.Distance(transform.position, otherPlayer.transform.position)/75;
            scoreText.text = score.ToString("0.0");
        }
        if (!lockMovement)
        {
            if (player1)
            {
                //currVelocity.y = Input.GetAxis("VerticalP1");
                //currVelocity.x = Input.GetAxis("HorizontalP1");

                /*Debug.Log("P1H");
                Debug.Log(Input.GetAxis("P1H"));
                Debug.Log("P1V");
                Debug.Log(Input.GetAxis("P1V"));
                

                if (Input.GetKey(KeyCode.W))
                    currVelocity.y = 1.0f * moveSpd;
                else if (Input.GetKey(KeyCode.S))
                    currVelocity.y = -1.0f * moveSpd;
                if (Input.GetKey(KeyCode.A))
                    currVelocity.x = -1.0f * moveSpd;
                else if (Input.GetKey(KeyCode.D))
                    currVelocity.x = 1.0f * moveSpd;
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow))
                    currVelocity.y = 1.0f * moveSpd;
                else if (Input.GetKey(KeyCode.DownArrow))
                    currVelocity.y = -1.0f * moveSpd;
                if (Input.GetKey(KeyCode.LeftArrow))
                    currVelocity.x = -1.0f * moveSpd;
                else if (Input.GetKey(KeyCode.RightArrow))
                    currVelocity.x = 1.0f * moveSpd;
            }
            if (chaser)
            {
                currVelocity.x *= 1.1f;
                currVelocity.y *= 1.1f;
            }
            if (onGrass)
            {
                currVelocity.x *= 0.65f;
                currVelocity.y *= 0.65f;
            }
        }
        HandleMovement();
        if (Input.GetKeyUp(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void HandleMovement()
    {
        selfBody.velocity = Vector3.Lerp(selfBody.velocity, currVelocity, moveSmoothRate);
        currVelocity = new Vector3(0, 0, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (chaser && other.tag == "Catch")
        {
            otherPlayer.transform.GetChild(0).SetParent(transform);
            transform.GetChild(0).gameObject.SetActive(false);
            StartCoroutine(RunAway());
            otherPlayer.GetComponent<PlayerMove>().lockMovement = true;
            otherPlayer.GetComponent<PlayerMove>().chaser = true;            
            chaser = false;
        }
        if (other.tag == "Grass")
        {
            onGrass = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Grass")
        {
            onGrass = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grass")
        {
            onGrass = false;
        }
    }
    IEnumerator RunAway()
    {
        switching = true;
        yield return new WaitForSeconds(1);
        cameraMan.setRad = true;
        yield return new WaitForSeconds(waitTime);
        otherPlayer.GetComponent<PlayerMove>().lockMovement = false;
        switching = false;
        otherPlayer.GetComponent<PlayerMove>().sprRenderer.color = Color.red;
        sprRenderer.color = Color.white;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).position = transform.position;
    }
}
*/
