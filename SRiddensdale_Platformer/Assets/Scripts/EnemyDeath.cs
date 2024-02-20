using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the death animation of an enemy
/// </summary>
public class EnemyDeath : MonoBehaviour
{
    [SerializeField]
    private Health _health;
    [SerializeField]
    private Rigidbody2D _rb;
    [SerializeField]
    private Collider2D[] _collidersToDisable;
    [SerializeField]
    private AudioData _deathSound;
    [SerializeField]
    private float _destroyTime = 8.0f;
    [SerializeField]
    private Vector2 _randomAngularVelocity = new Vector2(-45, 45);

    void Start()
    {
        _health.OnHealthDepleted += Death;
    }

    private void Death()
    {
        // add random force to the enemy
        _rb.AddForce(new Vector2(Random.Range(-2, 2), 10), ForceMode2D.Impulse);
        _rb.freezeRotation = false;
        _rb.angularVelocity = Random.Range(_randomAngularVelocity.x, _randomAngularVelocity.y);

        // disable all colliders so the enemy can fall through the floor
        foreach(Collider2D c in _collidersToDisable) c.enabled = false;

        AudioHandler.instance.ProcessAudioData(_deathSound);

        // call destroy method overtime
        Destroy(gameObject, _destroyTime);
    }
}
