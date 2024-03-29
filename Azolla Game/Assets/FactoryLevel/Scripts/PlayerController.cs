using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Private Fields
    // Move and jump
    private float horizontal;
    private float speed = 5f;
    private float jumpingPower = 12f;
    private bool isFacingRight = true;
    // Dash
    private bool canDash = false;
    private bool isDashing;
    private float dashingPower = 30.0f;
    private float dashingTime = 0.25f;
    private float dashingCooldown = 1f;
    // Double Jump
    private bool doubleJump = false;
    // Wall Slide
    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    // Wall Jump
    private bool isWallJumping = false;
    private float wallJumpingDirection;
    private float WallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(4f, 8f);
    // Coyote time - can still jump 0.2f after leaving the ground
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    // Bounce
    private float bounceSpeed = 7f;
    // Collectibles
    public int woodCount = 0;
    public int ironCount = 0;
    public int copperCount = 0;
    public int goldCount = 0;
    // Collectible values - CONST
    private const int WOODVALUE = 2;
    private const int COPPERVALUE = 6;
    private const int IRONVALUE = 4;
    private const int GOLDVALUE = 8;

    // Classes
    private WinPanel winPanel;
    private LosePanel losePanel;
    private Animator anim;
    private Timer timer;

    // Serialized Fields
    [Header ("Rigidbody, ground check/layer")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header ("Trail effect")]
    [SerializeField] private TrailRenderer tr;

    [Header ("Wall checks")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    [Header ("Audio sources")]
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioSource jumpEffect;
    [SerializeField] private AudioSource oreCollectEffect;
    [SerializeField] private AudioSource woodCollectEffect;
    [SerializeField] private AudioSource damageEffect;
    [SerializeField] private AudioSource dashEffect;
    [SerializeField] private AudioSource powerUpEffect;

    [Header ("UI")]
    [SerializeField] private Text woodText;
    [SerializeField] private Text ironText;
    [SerializeField] private Text copperText;
    [SerializeField] private Text goldText;

    [Header ("win-lose-panel")]
    public GameObject winLosePanels;

    [Header ("Dust particle")]
    public ParticleSystem dust;

    private void Start()
    {
        timer = FindObjectOfType<Timer>();
        winPanel = FindAnyObjectByType<WinPanel>();
        losePanel = FindObjectOfType<LosePanel>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Character animation bool
        anim.SetBool("Run", horizontal != 0);
        anim.SetBool("IsGrounded", IsGrounded());

        // Character movement input
        horizontal = Input.GetAxisRaw("Horizontal");

        if (isDashing)
        {
            return;
        }

        // Coyote time counter
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Disable double jump if player is on the ground
        if(IsGrounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                doubleJump = !doubleJump;
                jumpEffect.Play();
                CreateDust();
            }
        }

        // Jump and reset coyote time counter
        if (Input.GetButtonDown("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            jumpEffect.Play();

            coyoteTimeCounter = 0f;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
            dashEffect.Play();
        }

        // Wall slide - jump methods.
        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
    }

    // Game Over screen
    public void PlayerDied()
    {
        AreaManager.instance.GameOver();
        gameObject.SetActive(false);
        losePanel.Lose();
    }
    // Player Win Screen
    public void PlayerWin()
    {
        AreaManager.instance.WinScreen();
        gameObject.SetActive(false);
        winPanel.Win();
        timer.timeOn = false;
    }

    private void FixedUpdate()
    {
        // Dash
        if (isDashing)
        {
            return;
        }

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    // Check player isGrounded
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    // Check player isWalled
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    // Wall slide
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    // Wall jump
    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallSliding = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = WallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    
    // Flip player sprite
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            CreateDust();
        }
    }

    // Dash settings
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

    // Create player dust effect
    void CreateDust()
    {
        dust.Play();
    }

    // Player collision with obstacles
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SpikesDanger"))
        {
            // Knockback effect
            rb.velocity += Vector2.up * bounceSpeed;

            // Reduce Health by 1 and check if hearts are above 0
            HeartsSystem.life--;
            if(HeartsSystem.life == 0 )
            {
                PlayerDied();
                bgMusic.Stop();
            }
            damageEffect.Play();
        }

        if  (collision.gameObject.CompareTag("Danger"))
        {
            // Reduce Health by 1 and check if hearts are above 0
            HeartsSystem.life--;
            if (HeartsSystem.life == 0)
            {
                PlayerDied();
                bgMusic.Stop();
            }
            damageEffect.Play();
        }
    }

    // Player collision with collectibles
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Wood
        if (collision.gameObject.CompareTag("Wood"))
        {
            woodCollectEffect.Play();
            Destroy(collision.gameObject);
            woodCount += WOODVALUE;
            woodText.text = "Wood: " + woodCount;
        }

        // Iron
        if (collision.gameObject.CompareTag("IronOre"))
        {
            oreCollectEffect.Play();
            Destroy(collision.gameObject);
            ironCount += IRONVALUE;
            ironText.text = "Iron: " + ironCount;
        }

        // Gold
        if (collision.gameObject.CompareTag("GoldOre"))
        {
            oreCollectEffect.Play();
            Destroy(collision.gameObject);
            goldCount += GOLDVALUE;
            goldText.text = "Gold: " + goldCount;
        }

        // Copper
        if (collision.gameObject.CompareTag("CopperOre"))
        {
            oreCollectEffect.Play();
            Destroy(collision.gameObject);
            copperCount += COPPERVALUE;
            copperText.text = "Copper: " + copperCount;
        }

        // Dash Bubble
        if (collision.gameObject.CompareTag("dashBubble"))
        {
            Destroy(collision.gameObject);
            canDash = true;
            powerUpEffect.Play();
        }

        // Win Game Trigger
        if (collision.gameObject.CompareTag("Win"))
        {
            PlayerWin();
            bgMusic.Stop();
        }
    }

    
}
