using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damage)) return;
        collision.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement movement);

        movement.StunPlayer();

        damage.TakeDamage(5);

        float xFactor = 12.0f;
        if (collision.gameObject.transform.position.x < transform.position.x) xFactor *= -1;

        collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(xFactor, 12.0f);
    }
}
