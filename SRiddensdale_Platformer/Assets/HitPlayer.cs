using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<Health>(out Health damage)) return;
        if (damage.IFramesActive) return; // dont take damage more than once

        collision.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement movement);

        movement.StunPlayer();

        damage.TakeDamage(5);

        float xFactor = 12.0f;
        if (collision.gameObject.transform.position.x < transform.position.x) xFactor *= -1;

        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(xFactor, 12.0f);
    }
}
