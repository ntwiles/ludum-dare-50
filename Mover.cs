using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    // Dependencies
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject ballPickupTrigger;

    // Components
    private Rigidbody2D rigidBody;
    private Animator animator;

    // Settings
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float groundDistance;
    [SerializeField] private float ballThrowSpeed;
    [SerializeField] private float ballThrowUpshotSpeed;
    [SerializeField] private float ballThrowRise;
    [SerializeField] private float ballThrowUpshotRise;
    [SerializeField] private Vector3 ballOffset;
    [SerializeField] private float runningThreshold;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBoostDuration;
    [SerializeField] private float jumpBoostForce;
    [SerializeField] private float jumpVelocity;

    // State
    private Vector3 startScale;
    private bool prevGrounded;
    private bool isRunning;

    private bool hasBall = false;
    private bool hasControl = true;
    private float timeSinceGrounded = 0;
    private bool jump = false;
    private bool canJump = true;
    private bool jumpHeld = false;

    // Events
    public UnityEvent BallPickedUp;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startScale = transform.localScale;
    }

    void Update() 
    {
        if (!hasControl) return;

        bool isGrounded = checkGrounded();

        if (isGrounded && !prevGrounded) {
            canJump = true;
        }

        prevGrounded = isGrounded;

        if (!isGrounded) {
            timeSinceGrounded += Time.deltaTime;
        } else {
            timeSinceGrounded = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && timeSinceGrounded < coyoteTime && canJump) { 
            jump = true;
            canJump = false;
            jumpHeld = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpHeld = false;
        }

        if (Input.GetKeyDown(KeyCode.RightShift) && hasBall) 
        {
            throwBall();
        }
        
        if (isGrounded) 
        {
            if (isRunning) 
            {
                if (rigidBody.velocity.magnitude < runningThreshold) 
                {
                    isRunning = false;
                    animator.SetBool("isRunning", false);
                }
            } else if (rigidBody.velocity.magnitude >= runningThreshold) 
            {
                isRunning = true;
                animator.SetBool("isRunning", true);
            }
        }

    }

    void FixedUpdate()
    {
        if (!hasControl) return;

        if (jump == true) {
            var vel = rigidBody.velocity;
            rigidBody.velocity = new Vector2(vel.x, jumpVelocity);
            //rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jump = false;
        }

        if (jumpHeld && timeSinceGrounded < jumpBoostDuration) {
            var vel = rigidBody.velocity;
            rigidBody.velocity = new Vector2(vel.x, jumpVelocity);
            //rigidBody.AddForce(Vector2.up * jumpBoostForce);
        }

        if (Input.GetKey(KeyCode.A)) {
            rigidBody.AddForce(Vector2.left * walkSpeed);
            transform.localScale = new Vector3(startScale.x * -1, startScale.y, startScale.z);
        }

        if (Input.GetKey(KeyCode.D)) {
            rigidBody.AddForce(Vector2.right * walkSpeed);
            transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
        }
    }

    public void DisableControl() {
        if (hasBall) {
            dropBall();
        }
            
        hasControl = false;

        animator.SetBool("isRunning", false);
    }

    private bool checkGrounded() {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;

        LayerMask wallLayer = LayerMask.GetMask("World");
        
        RaycastHit2D hit = Physics2D.Raycast(position, direction, groundDistance, wallLayer);

        if (hit.collider != null) {
            return true;
        }
        
        return false;
    }

    private void throwBall() 
    {
        bool isUpshot = Input.GetKey(KeyCode.W);

        float rise = isUpshot ? ballThrowUpshotRise : ballThrowRise;
        float speed = isUpshot ? ballThrowUpshotSpeed : ballThrowSpeed;

        StartCoroutine(enablePickupTrigger());

        ball.GetComponent<Ball>().ResetBounces();

        var ballBody = ball.GetComponent<Rigidbody2D>();
        ball.transform.SetParent(null);
        ballBody.velocity = Vector3.zero;

        var throwDirection = (Vector3.right * transform.localScale.x) + (Vector3.up * rise);

        ballBody.simulated = true;
        ballBody.AddForce(throwDirection * speed, ForceMode2D.Impulse);

        hasBall = false;
    }

    private void dropBall()
    {
        StartCoroutine(enablePickupTrigger());

        var ballBody = ball.GetComponent<Rigidbody2D>();

        ball.GetComponent<Ball>().ResetBounces();

        ball.transform.SetParent(null);
        ballBody.velocity = Vector3.zero;
        ballBody.simulated = true;

        var throwDirection = (Vector3.right * transform.localScale.x) + (Vector3.up * ballThrowRise);

        ballBody.AddForce(throwDirection * ballThrowSpeed * 0.1f, ForceMode2D.Impulse);

        hasBall = false;
    }

    private IEnumerator enablePickupTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        ballPickupTrigger.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (!hasControl) return;

        if (other.gameObject.tag == "BallPickup")
        {
            ball.transform.SetParent(transform);
            ball.transform.localPosition = ballOffset;
            
            var ballBody = ball.GetComponent<Rigidbody2D>();

            ballBody.simulated = false;
            ballPickupTrigger.SetActive(false);

            hasBall = true;
            BallPickedUp.Invoke();
        }

        var goal = other.gameObject.GetComponent<Goal>();

        if (goal != null && hasBall) 
        {
            if (goal.SinkGoal(0)) {
                dropBall();
            }
        }
    }
}
