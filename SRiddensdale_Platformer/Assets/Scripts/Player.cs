using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField]
    private float hanklingBubbleTimer = 10.0f;

    private Hankling myHankling;
    private Health myHealth;
    private PlayerMovement movement;

    // accessors
    public Hankling MyHankling { get { if (myHankling == null) myHankling = FindObjectOfType<Hankling>(); return myHankling; } }
    public bool IsHanklingBubbled { get; private set; }

    Timer activeHanklingTimer;

    private void Start()
    {
        myHealth = GetComponent<Health>();

        MyHankling.OnBubbleEnter += BubbleEntered;
        MyHankling.OnBubblePopped += BubbleExit;
        myHealth.OnIFramesTaken += PlayerHit;
    }

    private void PlayerHit()
    {
        // enter the bubble
        myHankling.EnterBubble();
    }

    private void Update()
    {
        // ensure the timer updates
        if (activeHanklingTimer != null) activeHanklingTimer.Tick(Time.deltaTime);
    }

    private void BubbleEntered()
    {
        // create a timer for when the hankling enters the bubble
        activeHanklingTimer = new Timer(hanklingBubbleTimer);
        activeHanklingTimer.OnTimerEnd += Lose;

        IsHanklingBubbled = true;
        AudioHandler.instance.SwitchMusic(AudioHandler.FadeMode.CrossFade, 0.5f, 0.5f, 0.62f, AudioHandler.instance.BubbleTrack, false);
    }

    private void BubbleExit()
    {
        IsHanklingBubbled = false;

        if (activeHanklingTimer == null) return;

        // unsubscribe and set null
        activeHanklingTimer.OnTimerEnd -= Lose;
        activeHanklingTimer = null;

        AudioHandler.instance.SwitchMusic(AudioHandler.FadeMode.None, 0.0f, 0.0f, 1.0f, AudioHandler.instance.StageTheme);
    }

    /// <summary>
    /// Called when the hankling timer expires
    /// </summary>
    private void Lose()
    {
        GameManager.Instance.CallGameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // restart level if hit hazard
        if (collision.gameObject.tag == "Hazard") GameManager.Instance.RestartLevel();
    }

    
}
