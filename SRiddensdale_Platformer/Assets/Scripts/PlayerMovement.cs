using UnityEngine;
using System.Collections;
using static PlayerMovement;

public class PlayerMovement : MonoBehaviour
{
    public enum Direction
    {
        Left = -1,
        Right = 1
    }

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

    [Header("Gravity")]
    [SerializeField]
    private AnimationCurve _gravityOverTime;
    [SerializeField]
    private float _initialGravity = 1.0f;
    [SerializeField]
    private float _targetGravity = 5.0f;
    [SerializeField]
    private float _evaluationTime = 2.5f;

    [Header("Ground Check")]
    [SerializeField]
    private Vector2 _groundCheckOffset = new Vector2(0, -0.5f);
    [SerializeField]
    private float _groundCheckRadius = 0.25f;
    [SerializeField]
    private LayerMask _groundCheckMask;

    [Header("RPG")]
    [SerializeField]
    private float _rpgCooldownTime = 0.5f;

    // private variables
    private float xInput;
    private bool jumpQueued = false;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float airTime = 0.0f;
    private float gravityFactor = 1.0f;

    private Coroutine activeGravityCoroutine;
    private bool enteredAirFlag;
    private bool gravityCoroutineFinished;
    private bool stunned = false;

    // enums
    private Direction direction;

    // events
    public delegate void Jump();
    public Jump OnJump;

    public delegate void Land();
    public Land OnLand;

    // accessors
    public bool IsMoving { get; private set; }
    public Rigidbody2D MyBody { get { return rb; } }
    public float TopSpeed { get { return _playerSpeed; } }
    public bool IsRPGReady { get { return inRpgState; } }

    bool inputFrozen = false;
    bool playerFrozen = false;
    bool inRpgState = false;
    bool rpgQueued = false;
    bool canUseRPG = true;

    public delegate void RPGReady();
    public RPGReady OnRPGReady;
    private Timer rpgCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        TryQueueJump();
        UpdateGravity();

        if (IsGrounded()) stunned = false;
        if (rpgQueued) EnterRPGState();

        if (rpgCooldown != null) rpgCooldown.Tick(Time.deltaTime);

        // Set moving variable
        IsMoving = xInput != 0;
    }

    RPGIndicator indicator;

    private void EnterRPGState()
    {
        if (!IsGrounded() && !inRpgState) return;

        inRpgState = true;
        xInput = 0.0f;
        rpgQueued = false;

        if (indicator == null) indicator = FindObjectOfType<RPGIndicator>(true);
        OnRPGReady?.Invoke();

        indicator.gameObject.SetActive(true);
    }

    public void ExitRPGState()
    {
        rpgQueued = false;
        inRpgState = false;
        canUseRPG = false;

        rpgCooldown = new Timer(_rpgCooldownTime);
        rpgCooldown.OnTimerEnd += () => canUseRPG = true;
    }

    public void StunPlayer() => stunned = true;

    private void FixedUpdate()
    {
        // handle jump
        if (jumpQueued) HandleJump();
        if (stunned) return;
        
        rb.velocity = new Vector2(xInput * _playerSpeed, rb.velocity.y);
    }

    private void HandleJump()
    {
        // directly set player velocity
        rb.velocity = new Vector2(rb.velocity.x, _playerJumpHeight);
        OnJump?.Invoke();

        // set flag
        jumpQueued = false;
    }

    float lastXInput = 0.0f;

    /// <summary>
    /// Retrieves necessary inputs for movement calculations
    /// </summary>
    private void GetInput()
    {
        if (inputFrozen || stunned) return;

        // get axis inputs

        if(!inRpgState) xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0) lastXInput = xInput;
        if (Input.GetKeyDown(KeyCode.X) && canUseRPG && !inRpgState) {
            rpgQueued = true;
        }
        else {
            rpgQueued = false;
        }

        // set direction based on the non 0 last x input
        direction = (Direction)((int)Mathf.Sign(lastXInput));
    }

    private void TryQueueJump()
    {
        if (inRpgState) return;

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

    bool groundFlag = false;

    /// <summary>
    /// Returns whether or not the player is grounded
    /// </summary>
    public bool IsGrounded()
    {
        Collider2D col = Physics2D.OverlapCircle((Vector2)transform.position + _groundCheckOffset, _groundCheckRadius, _groundCheckMask);

        // return true if a collider was found, otherwise, return false.
        if (col != null) {

            // the ground flag handles calling the land event only once.
            if (!groundFlag) OnLand?.Invoke();
            groundFlag = true;

            return true;
        }
        else {
            groundFlag = false;
            return false;
        }

    }

    /// <summary>
    /// Updates the players gravity based on a coroutine
    /// </summary>
    private void UpdateGravity()
    {
        if (IsGrounded())
        {
            enteredAirFlag = true;
            gravityCoroutineFinished = false;
        }

        // add to airtime while not grounded
        if (!IsGrounded() && activeGravityCoroutine == null && !gravityCoroutineFinished || !IsGrounded() && enteredAirFlag && !gravityCoroutineFinished)
        {
            // ensure any active coroutine is stopped
            StopAllCoroutines();
            activeGravityCoroutine = StartCoroutine(EvaluateGravity());

            enteredAirFlag = false;
        }

        // override gravity with new gravity factor
        rb.gravityScale = gravityFactor;
    }

    private IEnumerator EvaluateGravity()
    {
        // set to initial gravity
        gravityFactor = _initialGravity;

        float elapsed = 0.0f;
        float curveValue = _gravityOverTime.Evaluate(0.0f);

        while (elapsed < _evaluationTime)
        {
            // increment timer and continue evaluating the curve
            elapsed += Time.deltaTime;
            curveValue = _gravityOverTime.Evaluate(elapsed / _evaluationTime);

            // use an exponential equation rather than just lerping normally bc frame dependency
            gravityFactor = Mathf.Lerp(gravityFactor, _targetGravity, 1 - Mathf.Pow(1 - curveValue, Time.deltaTime));

            yield return null;
        }

        activeGravityCoroutine = null;
        gravityCoroutineFinished = true;
    }

    public Direction GetDirection() => direction;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // wont be able to set to platform if normal is incorrect
        if (collision.gameObject.tag != "Platform" || collision.contacts[0].normal != Vector2.up) return;

        transform.SetParent(collision.transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Platform") return;

        transform.SetParent(null);
    }

    public void FreezeInput(bool frozen) => inputFrozen = frozen;
    public void FreezePlayer(bool frozen)
    {
        playerFrozen = frozen;

        if (playerFrozen) rb.isKinematic = true;
        else  rb.isKinematic = false;
    }
}

