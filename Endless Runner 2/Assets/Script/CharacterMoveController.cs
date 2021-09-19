using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("GameOver")]
    public GameObject gameOverScreen;
    public float fallPositionY;

    [Header("Camera")]
    public CameraMoveController gameCamera;

    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayermask;

    
    [Header("Scoring")]
    public float scoringRatio;
    public ScoreController score;

    private float lastPositionX;

    private CharacterSoundController sound;

    private Animator anim;

    private Rigidbody2D rig;

    private bool isJumping;
    private bool isOnGround;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
        lastPositionX = transform.position.x;
    }

    private void Update()
    {
        //input
        if (Input.GetMouseButtonDown(0))
        {
            if (isOnGround) 
            {
                isJumping = true;

                sound.PlayJump();
            }
             
        }

        

        anim.SetBool("isOnGround", isOnGround);

        //calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
        lastPositionX += distancePassed;
        }

        //game over
        if (transform.position.y < fallPositionY){
            GameOver();
        }

    }

    private void FixedUpdate()
    {
        //ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,
            groundRaycastDistance, groundLayermask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }
        Vector2 velocityVector = rig.velocity;
        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }
        
        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;
    }

    private void GameOver()
    {
        //set high score
        score.FinishScoring();


        //stop camera movement
        gameCamera.enabled = false;

        //show game over
        gameOverScreen.SetActive(true);

        //disable this too
        this.enabled = false;



    }


    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.red);
    }

}
