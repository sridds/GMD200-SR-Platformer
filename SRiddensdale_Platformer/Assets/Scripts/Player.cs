using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float hanklingBubbleTimer = 10.0f;

    private Hankling myHankling;

    // accessors
    public Hankling MyHankling { get { if (myHankling == null) myHankling = FindObjectOfType<Hankling>(); return myHankling; } }

    Timer activeHanklingTimer;

    private void Start()
    {
        MyHankling.OnBubbleEnter += BubbleEntered;
        MyHankling.OnBubblePopped += BubbleExit;
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
    }

    private void BubbleExit()
    {
        if (activeHanklingTimer == null) return;

        // unsubscribe and set null
        activeHanklingTimer.OnTimerEnd -= Lose;
        activeHanklingTimer = null;
    }

    /// <summary>
    /// Called when the hankling timer expires
    /// </summary>
    private void Lose()
    {
        Debug.Log("Hankling gone");

        GameManager.Instance.CallGameOver();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // restart level if hit hazard
        if (collision.gameObject.tag == "Hazard") GameManager.Instance.RestartLevel();
    }

    
}
