using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // objects atributes
    private PlayerControl playerControl;
    private BoxCollider2D footsCollider;
    private Rigidbody2D rb2d;
    private Animator animator;

    // constants
    private const int GROUNDLAYER = 8;
    private const int PLATFORMLAYER = 9;
    private const float FASTFALLSPEED = 7.0f;
    private const float FALLMULTIPLIER = 2.5f;
    private const float LOWJUMPMULTIPLIER = 2.1f;
    private const float FASTMOVESPEED = 12f;

    // adjustable atributes
    public LayerMask solidLayerMask;
    public float runSpeed = 4.0f;
    public float jumpSpeed = 6.0f;
    public int airJumps = 1;
    public float fastMoveRate = 3f;

    // auxiliary variables
    private bool isGrounded = true;
    private bool facingRight = true;
    private bool ignoreMovement = false;
    private int currAirJumps = 0;
    private float lastFastMove = 0f;


    void Awake()
    {
        // objects initialization
        this.playerControl = this.gameObject.GetComponent<PlayerControl>();
        this.footsCollider = this.gameObject.transform.Find("Foots").GetComponent<BoxCollider2D>();
        this.rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        this.animator = this.gameObject.GetComponent<Animator>();

        // variables initialization
        this.lastFastMove = -this.fastMoveRate;
    }
    

    void Update()
    {
        // ground handle
        this.isGrounded = this.GroundCheck();
        this.animator.SetBool("IsGrounded", this.isGrounded);
        if (this.isGrounded)
            this.currAirJumps = 0;
    }

    
    void FixedUpdate()
    {
        if (!this.playerControl.isAvailable)
            return;

        if (!this.ignoreMovement)
            HandleHorizontalMovement();
        HandleVerticalMovement();
        HandleFastMove();
    }

    // horizontal move
    private void HandleHorizontalMovement()
    {
        float x_velocity = this.playerControl.leftStickX * this.runSpeed;
        this.rb2d.velocity = new Vector2(x_velocity, rb2d.velocity.y);
        this.animator.SetFloat("Speed", Mathf.Abs(this.rb2d.velocity.x));
        if (x_velocity < 0)
        {
            if (this.facingRight)
                transform.Rotate(0f, 180f, 0f);
            this.facingRight = false;
        }
        else if (x_velocity > 0)
        {
            if (!this.facingRight)
                transform.Rotate(0f, 180f, 0f);
            this.facingRight = true;
        }
    }


    // vertical move
    private void HandleVerticalMovement()
    {
        // fall
        RaycastHit2D raycastHit = Physics2D.BoxCast(this.footsCollider.bounds.center, this.footsCollider.bounds.size, 0f, Vector2.down, 0.1f, 1 << GROUNDLAYER);
        if (this.playerControl.leftStickY == -1f && raycastHit.collider == null)
        {
            this.animator.SetTrigger("IsFalling");
            this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, -FASTFALLSPEED);
            raycastHit = Physics2D.BoxCast(this.footsCollider.bounds.center, this.footsCollider.bounds.size, 0f, Vector2.down, 0.1f, 1 << PLATFORMLAYER);
            if (raycastHit.collider != null)
                this.StartCoroutine(this.IgnorePlatform(raycastHit.collider));
        }

        // jump
        if (this.playerControl.buttonJumpDown)
        {
            if (this.isGrounded)
            {
                this.animator.SetTrigger("IsJumping");
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.jumpSpeed);
            }
            else if (this.currAirJumps < this.airJumps)
            {
                this.animator.SetTrigger("IsJumping");
                this.currAirJumps++;
                this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, this.jumpSpeed);
            }
            this.playerControl.buttonJumpDown = false;
        }

        // falling faster than jumping
        if (this.rb2d.velocity.y < 0)
            this.rb2d.velocity += Vector2.up * Physics2D.gravity.y * (FALLMULTIPLIER - 1) * Time.deltaTime;
        else if (rb2d.velocity.y > 0 && !this.playerControl.buttonJump)
            this.rb2d.velocity += Vector2.up * Physics2D.gravity.y * (LOWJUMPMULTIPLIER - 1) * Time.deltaTime;
    }


    private void HandleFastMove()
    {
        if (this.playerControl.buttonFastMoveDown && Time.time - lastFastMove >= fastMoveRate)
        {
            this.animator.SetTrigger("IsFastMove");
            this.rb2d.AddForce(this.gameObject.transform.right * FASTMOVESPEED, ForceMode2D.Impulse);
            this.lastFastMove = Time.time;
            this.StartCoroutine(this.IgnoreMovement());
        }
        this.playerControl.buttonFastMoveDown = false;
    }


    // ignoring platform and foots for 0.25sec
    private IEnumerator IgnorePlatform(Collider2D collider)
    {
        Physics2D.IgnoreCollision(this.footsCollider, collider, true);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(this.footsCollider, collider, false);
    }


    // ignoring movement for 0.1sec
    public IEnumerator IgnoreMovement()
    {
        this.ignoreMovement = true;
        yield return new WaitForSeconds(0.1f);
        this.ignoreMovement = false;
    }


    private bool GroundCheck()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(this.footsCollider.bounds.center, this.footsCollider.bounds.size, 0f, Vector2.down, 0.1f, solidLayerMask);
        return raycastHit.collider != null && this.rb2d.velocity.y < 3 && this.rb2d.velocity.y > -1.5;
    }

}
