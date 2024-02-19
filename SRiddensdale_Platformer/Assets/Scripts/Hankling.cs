using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hankling : MonoBehaviour
{
    public enum State
    {
        Carried,
        Bubble,
    }

    private Player player;
    private State myState;

    [Header("Hold Settings")]
    [SerializeField]
    private Vector2 _holdOffset;
    [SerializeField]
    private float _speed = 0.1f;

    [Header("Bubble Settings")]
    [SerializeField]
    private Bubble _bubble;
    [SerializeField]
    private float _allowPopAfterSeconds = 1.0f;

    [Header("Return Settings")]
    [SerializeField]
    private float _freezeTime = 0.5f;
    [SerializeField]
    private float _returnToPlayerTime = 1.0f;

    [Header("SFX")]
    [SerializeField]
    private AudioData _bubblePopSound;
    [SerializeField]
    private AudioSource _cryingSound;

    [Header("VFX")]
    [SerializeField]
    private ParticleSystem _bubblePopEffect;

    Vector2 vel;
    Animator animator;

    // events
    public delegate void BubbleEnter();
    public BubbleEnter OnBubbleEnter;

    public delegate void BubblePopped();
    public BubblePopped OnBubblePopped;

    private float bubbleActiveTime;
    private bool followPlayer = true;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        // subscribe to events
        _bubble.OnBubbleTriggered += ExitBubble;
        GameManager.Instance.OnGameOver += EndCrying;

        animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameOver -= EndCrying;
    }

    public void EndCrying() => StartCoroutine(IEndCrying());

    private IEnumerator IEndCrying()
    {
        yield return new WaitForSecondsRealtime(2);

        float elapsed = 0.0f;
        float duration = 5.0f;
        float initial = _cryingSound.volume;
        float target = 0.0f;

        while(elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _cryingSound.volume = Mathf.Lerp(initial, target, elapsed / duration);

            yield return null;
        }

        _cryingSound.volume = target;

    }

    public void EnterBubble()
    {
        // return if already in bubble
        if (myState == State.Bubble) return;

        _bubble.transform.position = transform.position;
        _bubble.gameObject.SetActive(true);
        animator.SetBool("Sad", true);
        _cryingSound.Play();
        myState = State.Bubble;

        OnBubbleEnter?.Invoke();
    }

    public void ExitBubble()
    {
        // return if already carried
        if (myState == State.Carried) return;

        if (bubbleActiveTime < _allowPopAfterSeconds) return;
        _bubble.gameObject.SetActive(false);
        animator.SetBool("Sad", false);
        _cryingSound.Stop();

        AudioHandler.instance.ProcessAudioData(_bubblePopSound);
        Instantiate(_bubblePopEffect, transform.position, Quaternion.identity);

        OnBubblePopped?.Invoke();
        StartCoroutine(ReturnToPlayer());
    }

    private void Update()
    {
        // handle states
        if (myState == State.Carried) HandleCarriedState();
        else if (myState == State.Bubble) HandleBubbleState();

        // debug
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (myState == State.Carried) EnterBubble();
            else ExitBubble();
        }*/
    }

    /// <summary>
    /// Handles the state while being carried
    /// </summary>
    private void HandleCarriedState()
    {
        if (!followPlayer) return;

        bubbleActiveTime = 0.0f;
        Vector2 target = (Vector2)player.transform.position + _holdOffset;

        // hold above head
        transform.position = Vector2.SmoothDamp(transform.position, target, ref vel, _speed);
        transform.position = new Vector2(target.x, transform.position.y);
    }

    public bool SetFollowPlayer(bool doFollow) => followPlayer = doFollow;

    /// <summary>
    /// Handles the state while in a bubble
    /// </summary>
    private void HandleBubbleState()
    {
        bubbleActiveTime += Time.deltaTime;

        transform.position = _bubble.transform.position;
    }

    /// <summary>
    /// Returns the hankling to the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnToPlayer()
    {
        GameManager.Instance.SetTimeScale(0.0f, _freezeTime);
        yield return new WaitForSecondsRealtime(_freezeTime);

        float elapsed = 0.0f;
        float duration = _returnToPlayerTime;

        Vector2 current = transform.position;
        Vector2 target = (Vector2)player.transform.position + _holdOffset;

        while(elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.position = Vector2.Lerp(current, target, elapsed / duration);

            yield return null;
        }

        myState = State.Carried;
        transform.position = target;

        yield return null;
        GameManager.Instance.SetTimeScale(1.0f, 0.3f);
        transform.position = target;
    }
}
