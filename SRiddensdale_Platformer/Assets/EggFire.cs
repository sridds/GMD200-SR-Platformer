using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFire : MonoBehaviour
{
    [SerializeField]
    private Health _health;
    [SerializeField]
    private float _fireInterval;
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private Vector2 _randomX;
    [SerializeField]
    private Vector2 _randomY;
    [SerializeField]
    private AudioData _fireSound;
    [SerializeField]
    private float _distToThrow = 15.0f;

    private Timer _fireTimer;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_health.IsHealthDepleted) return;
        if (Vector2.Distance(GameManager.Instance.LocalPlayer.transform.position, transform.position) > _distToThrow) return;

        // Handle timer
        if (_fireTimer == null) {
            _fireTimer = new Timer(_fireInterval);
            _fireTimer.OnTimerEnd += StartAnimation;
        }
        else _fireTimer.Tick(Time.deltaTime); 
    }

    private void StartAnimation()
    {
        animator.SetTrigger("Flip");
    }

    public void Fire()
    {
        GameObject go = Instantiate(_prefab, _firePoint.position, Quaternion.identity);
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        AudioHandler.instance.ProcessAudioData(_fireSound);

        rb.velocity = new Vector2(Random.Range(_randomX.x, _randomX.y), Random.Range(_randomY.x, _randomY.y));

        _fireTimer = null;
    }
}
