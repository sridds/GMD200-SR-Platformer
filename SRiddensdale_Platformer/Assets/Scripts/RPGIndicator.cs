using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGIndicator : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _amplitude = 4.0f;
    [SerializeField]
    private AudioData _tickAudio;
    [SerializeField]
    private float _tickFrequency = 0.1f;

    private Animator animator;

    private float _maxAngle = 90;
    private float _minAngle = 0;

    bool flipped = false;
    bool flashing = false;

    float timer = 0.0f;

    private PlayerMovement movement;
    private Timer tickTimer;

    private void Start()
    {
        movement = FindObjectOfType<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    PlayerMovement.Direction lastDir;

    void Update()
    {
        if (tickTimer != null)
        {
            if(!flashing) tickTimer.Tick(Time.deltaTime);
        }
        else {
            tickTimer = new Timer(_tickFrequency);
            tickTimer.OnTimerEnd += () => { AudioHandler.instance.ProcessAudioData(_tickAudio); tickTimer = null; };
        }

        if(movement.GetDirection() != lastDir) {
            timer = 0.0f;
        }
        lastDir = movement.GetDirection();

        float speedMultiplier = 1.0f;
        if(movement.GetDirection() == PlayerMovement.Direction.Left)
        {
            _maxAngle = 90;
            _minAngle = 0;

            speedMultiplier = -1.0f;
        }
        else
        {
            _maxAngle = 180;
            _minAngle = 90;

            speedMultiplier = 1.0f;
        }

        Vector2 circleCenter = GameManager.Instance.LocalPlayer.transform.position;

        Vector2 dir = (circleCenter - (Vector2)transform.position).normalized;
        float angle = Vector2.Angle(Vector2.right, dir);

        if (angle > _maxAngle)
        {
            flipped = !flipped;

            if (flipped) timer -= Time.deltaTime * 2;
            else timer += Time.deltaTime * 2;
        }
        else if (angle < _minAngle)
        {
            flipped = !flipped;

            if (flipped) timer -= Time.deltaTime * 2;
            else timer += Time.deltaTime * 2;
        }

        if (flipped) timer -= Time.deltaTime;
        else timer += Time.deltaTime;

        float sin = Mathf.Sin(timer * _amplitude) * _speed * speedMultiplier;
        float cos = Mathf.Cos(timer * _amplitude) * _speed * speedMultiplier;

        if(!flashing) transform.position = new Vector2(circleCenter.x + cos, circleCenter.y + sin);
    }

    private void OnEnable()
    {
        timer = 0.0f;
        flashing = false;
    }

    public void Flash()
    {
        animator.SetTrigger("Flash");
        flashing = true;
    }

    public void Exit()
    {
        animator.SetTrigger("Exit");
    }

    public void Deactivate() => gameObject.SetActive(false);
}
