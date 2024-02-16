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

    [Header("Return Settings")]
    [SerializeField]
    private float _freezeTime = 0.5f;
    [SerializeField]
    private float _returnToPlayerTime = 1.0f;

    Vector2 vel;

    // events
    public delegate void BubbleEnter();
    public BubbleEnter OnBubbleEnter;

    public delegate void BubblePopped();
    public BubblePopped OnBubblePopped;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();

        // subscribe to events
        _bubble.OnBubbleTriggered += ExitBubble;
    }

    public void EnterBubble()
    {
        _bubble.gameObject.SetActive(true);
        myState = State.Bubble;

        OnBubbleEnter?.Invoke();
    }

    public void ExitBubble()
    {
        _bubble.gameObject.SetActive(false);

        OnBubblePopped?.Invoke();
        StartCoroutine(ReturnToPlayer());
    }

    private void Update()
    {
        // handle states
        if (myState == State.Carried) HandleCarriedState();
        else if (myState == State.Bubble) HandleBubbleState();

        // debug
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (myState == State.Carried) EnterBubble();
            else ExitBubble();
        }
    }

    private void HandleCarriedState()
    {
        Vector2 target = (Vector2)player.transform.position + _holdOffset;

        // hold above head
        transform.position = Vector2.SmoothDamp(transform.position, target, ref vel, _speed);
        transform.position = new Vector2(target.x, transform.position.y);
    }

    private void HandleBubbleState() { }

    private IEnumerator ReturnToPlayer()
    {
        yield return new WaitForSeconds(_freezeTime);

        float elapsed = 0.0f;
        float duration = _returnToPlayerTime;

        Vector2 current = transform.position;
        Vector2 target = (Vector2)player.transform.position + _holdOffset;

        while(elapsed < duration)
        {
            elapsed += Time.unscaledTime;
            transform.position = Vector2.Lerp(current, target, elapsed / duration);

            yield return null;
        }

        myState = State.Carried;
    }
}
