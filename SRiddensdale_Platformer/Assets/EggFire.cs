using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFire : MonoBehaviour
{
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

    private Timer _fireTimer;

    private void Update()
    {
        // Handle timer
        if (_fireTimer == null) {
            _fireTimer = new Timer(_fireInterval);
            _fireTimer.OnTimerEnd += Fire;
        }
        else _fireTimer.Tick(Time.deltaTime); 
    }

    private void Fire()
    {
        GameObject go = Instantiate(_prefab, _firePoint.position, Quaternion.identity);
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(Random.Range(_randomX.x, _randomX.y), Random.Range(_randomY.x, _randomY.y));

        _fireTimer = null;
    }
}
