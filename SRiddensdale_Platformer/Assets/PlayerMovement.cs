using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Modifiers")]
    [SerializeField]
    private float _playerSpeed;

    [Header("Jump")]
    [SerializeField]
    private float _playerJumpHeight;
    [SerializeField]
    private float _maxJumpHoldTime;
    [SerializeField]
    private float _jumpBufferTime;
    [SerializeField]
    private float _coyoteTime;
    [SerializeField]
    private AnimationCurve _gravityOverTime;

    [Header("Ground Check")]
    [SerializeField]
    private Vector2 _groundCheckOffset = new Vector2(0, -0.5f);
    [SerializeField]
    private float _groundCheckRadius = 0.25f;
    [SerializeField]
    private LayerMask _groundCheckMask;

    // private variables
    private float xInput;
    private bool jumpQueued = false;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float airTime = 0.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        TryQueueJump();

        // add to airtime while not grounded
        if (!IsGrounded()) EvaluateGravity();
        else airTime = 0.0f;
    }

    private void FixedUpdate()
    {
        // handle jump
        if (jumpQueued) HandleJump();

        rb.velocity = new Vector2(xInput * _playerSpeed, rb.velocity.y);
    }

    private void HandleJump()
    {
        // directly set player velocity
        rb.velocity = new Vector2(rb.velocity.x, _playerJumpHeight);

        // set flag
        jumpQueued = false;
    }

    private void EvaluateGravity()
    {
        airTime += Time.deltaTime;


    }

    /// <summary>
    /// Retrieves necessary inputs for movement calculations
    /// </summary>
    private void GetInput()
    {
        // get axis inputs
        xInput = Input.GetAxisRaw("Horizontal");
    }

    private void TryQueueJump()
    {
        // reset coyoteTime
        if (IsGrounded()) coyoteTimeCounter = _coyoteTime;
        // tick down coyote time while not grounded
        else coyoteTimeCounter -= Time.deltaTime;

        // start jump buffer
        if (Input.GetKeyDown(KeyCode.Z)) jumpBufferCounter = _jumpBufferTime;
        else jumpBufferCounter -= Time.deltaTime;

        // Queue jump
        if(jumpBufferCounter > 0 && coyoteTimeCounter > 0) {
            jumpQueued = true;
            jumpBufferCounter = 0f;
        }
        if(Input.GetKeyUp(KeyCode.Z) && rb.velocity.y > 0f) coyoteTimeCounter = 0f;
    }

    /// <summary>
    /// Returns whether or not the player is grounded
    /// </summary>
    public bool IsGrounded()
    {
        Collider2D col = Physics2D.OverlapCircle((Vector2)transform.position + _groundCheckOffset, _groundCheckRadius, _groundCheckMask);

        // return true if a collider was found, otherwise, return false.
        if (col != null) return true;
        return false;
    }
}
