using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        Hit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Hit(collision.gameObject);
    }

    private void Hit(GameObject go)
    {
        if (!go.TryGetComponent<Health>(out Health damage)) return;

        if (damage.IFramesActive) return; // dont take damage more than once

        go.TryGetComponent<PlayerMovement>(out PlayerMovement movement);

        movement.StunPlayer();
        damage.TakeDamage(5);

        float xFactor = 12.0f;
        if (go.transform.position.x < transform.position.x) xFactor *= -1;

        go.GetComponent<Rigidbody2D>().velocity = new Vector2(xFactor, 12.0f);
    }
}
