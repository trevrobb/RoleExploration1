using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horizontal;
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpingPower = 16f;
    bool isFacingRight = true;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;


    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 16f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    [SerializeField] TrailRenderer tr;


    [SerializeField] ParticleSystem dust;
    [SerializeField] ParticleSystem dust2;

    bool gameNotStarted = true;
    bool isJumping = false;
    bool jumped = false;

    bool dustPlaying = false;

    int timesPlayed = 0;

    Vector3 scaleVector;

    bool jumpingNotStarted = false;
    void Start()
    {
        scaleVector = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isDashing)
        {
            return;
        }

        
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;

        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter >= 0f && coyoteTimeCounter >= 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            CreateDust();
            isJumping = true;
            timesPlayed = 0;
            jumpBufferCounter = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
            
            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        
        Flip();
    }

    private void FixedUpdate()
    {

        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        if (IsGrounded())
        {
            if (timesPlayed < 1)
            {
                CreateDust2();
                timesPlayed++;
            }

        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            scaleVector = transform.localScale; 
            dust.Play();
        }
    }

    private IEnumerator Dash()
    {

       
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    void CreateDust()
    {
        dust.Play();
    }
    void CreateDust2()
    {
        dust2.Play();   
    }

    
    
  
}
