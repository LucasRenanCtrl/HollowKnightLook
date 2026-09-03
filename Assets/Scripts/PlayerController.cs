using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    public float walkSpeed = 5f;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    public float jumpForce = 45f;
    public int jumpBufferFrames = 8;
    private int jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    public float coyoteTime = 0.1f;
    private int airJumpCounter = 0;
    public int maxAirJumps = 1;

    [Space(5)]
    [Header("Ground Check Settings")]
    public float groundCheckY = 0.2f;
    public float groundCheckX = 0.2f;
    public Transform groundCheckPoint;
    public LayerMask groundMask;

    [Space(5)]
    [Header("Dash Settings")]
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public bool dashed;
    public bool canDash = true;
    public GameObject dashEffect;

    private float xAxis;
    private float gravity;
    private PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public static PlayerController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();

        if (pState.dashing) return;

        Flip();
        Move();
        Jump();
        StartDash();
    }

    private void Flip()
    {
        if (xAxis < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (xAxis > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void Jump()
    {
        //Permite ao jogador parar o pulo no meio
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);

            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);

                pState.jumping = true;
            }
            else if (!IsGrounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
            }
        }

        anim.SetBool("Jumping", !IsGrounded());
    }

    void Move()
    {
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
        anim.SetBool("Walking", IsMoving());

    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(nameof(Dash));
            dashed = true;
        }

        if (IsGrounded())// Para permitir que o jogador só de 1 dash no ar
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;

        float direction;
        if (spriteRenderer.flipX)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        rb.velocity = new Vector2(dashSpeed * direction, 0);

        if (IsGrounded())
        {
            InstantiateDashEffect(direction);
        }

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void InstantiateDashEffect(float direction)
    {
        GameObject instantiatedDashEffect = Instantiate(dashEffect);
        
        Vector3 currentPosition = instantiatedDashEffect.transform.position;
        SpriteRenderer spriteDashEffect = instantiatedDashEffect.GetComponent<SpriteRenderer>();

        if (direction < 0)
        {
            spriteDashEffect.flipX = true;
            currentPosition.x *= direction; // Inverte o valor de X
        }
        else
        {
            spriteDashEffect.flipX = false;
        }

        //Adiciona a nova posição e faz o objeto ser filho do transform do jogador
        instantiatedDashEffect.transform.position = this.transform.position + currentPosition;
        instantiatedDashEffect.transform.parent = this.transform;
    }

    bool IsMoving()
    {
        if (rb.velocity.x != 0 && IsGrounded())
        {
            Flip();
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsGrounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundMask)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundMask)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateJumpVariables()
    {
        if (IsGrounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
