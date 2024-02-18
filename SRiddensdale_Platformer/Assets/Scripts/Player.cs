using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Player : MonoBehaviour
{
    [Header("Bubble")]
    [SerializeField]
    private float hanklingBubbleTimer = 10.0f;

    [Header("RPG")]
    [SerializeField]
    private Rocket rocketPrefab;
    [SerializeField]
    private float rocketShootForce = 10.0f;
    [SerializeField]
    private AudioData fireRPGSound;

    private Hankling myHankling;
    private Health myHealth;
    private PlayerMovement movement;

    // accessors
    public Hankling MyHankling { get { if (myHankling == null) myHankling = FindObjectOfType<Hankling>(); return myHankling; } }
    public bool IsHanklingBubbled { get; private set; }

    Timer activeHanklingTimer;
    Timer rpgPrimedTimer;

    private bool fireReady = false;
    private bool firing = false;
    private RPGIndicator indicator;

    public delegate void FireRPG();
    public FireRPG OnFireRPG;

    private void Start()
    {
        myHealth = GetComponent<Health>();
        movement = GetComponent<PlayerMovement>();

        MyHankling.OnBubbleEnter += BubbleEntered;
        MyHankling.OnBubblePopped += BubbleExit;
        movement.OnRPGReady += ReadyRPG;

        myHealth.OnIFramesTaken += PlayerHit;
    }

    private void ReadyRPG()
    {
        // wait a little bit before allowing the player to fire the rpg
        rpgPrimedTimer = new Timer(0.2f);
        rpgPrimedTimer.OnTimerEnd += () => fireReady = true;
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
        if (rpgPrimedTimer != null) rpgPrimedTimer.Tick(Time.deltaTime);

        if (fireReady && Input.GetKeyDown(KeyCode.X) && !firing) {
            StartCoroutine(FireBullet());
        }
    }

    private IEnumerator FireBullet()
    {
        if (indicator == null) indicator = FindObjectOfType<RPGIndicator>();
        firing = true;

        fireReady = false;
        OnFireRPG?.Invoke();
        CameraShake.instance.Shake(0.5f, 0.3f);
        AudioHandler.instance.ProcessAudioData(fireRPGSound);

        Rocket r = Instantiate(rocketPrefab, indicator.transform.position, Quaternion.identity);
        Rigidbody2D rb = r.gameObject.GetComponent<Rigidbody2D>();

        Vector2 dir = indicator.transform.position - transform.position;
        rb.velocity = new Vector2(dir.x, dir.y).normalized * rocketShootForce;

        Vector2 rot = transform.position - indicator.transform.position;
        float rotAmt = Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg;

        r.transform.rotation = Quaternion.Euler(0, 0, rotAmt + 90);
        indicator.Flash();

        yield return new WaitForSeconds(0.5f);
        indicator.Exit();

        yield return new WaitForSeconds(0.5f);
        firing = false;
        fireReady = false;
        rpgPrimedTimer = null;

        movement.ExitRPGState();
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
