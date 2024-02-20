using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private Health _health;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private float _rayLength = 0.6f;
    [SerializeField]
    private float _rayDownLength = 1.0f;
    [SerializeField]
    private SpriteRenderer _sprite;

    private Rigidbody2D rb;
    private int dir = 1;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start() => rb = GetComponent<Rigidbody2D>();

    // Update is called once per frame
    void Update()
    {
        if (_health.IsHealthDepleted) return;

        // must be grounded to move
        if (isGrounded)
        {
            // create a ray in the direction currently moving
            Ray ray = new Ray(new Vector2(transform.position.x, transform.position.y), Vector2.right * dir);

            // check for wall. if wall, flip.
            if (Physics2D.Raycast(ray.origin, ray.direction * _rayLength, _rayLength, _groundLayer)) dir = -dir;
            // check for ground. if no ground, flip.
            else if (!Physics2D.Raycast(ray.origin + (ray.direction * _rayLength), Vector2.down * _rayDownLength, _rayDownLength, _groundLayer)) dir = -dir;

            // visualize ray
            Debug.DrawRay(ray.origin + (ray.direction * _rayLength), Vector2.down * _rayDownLength, Color.green);
        }

        // flip sprite
        if (_sprite != null) _sprite.flipX = dir == -1 ? false : true;

        // set velocity
        rb.velocity = new Vector2(_speed * dir, rb.velocity.y);
    }

    // Set grounded 
    private void OnCollisionStay2D(Collision2D collision) => isGrounded = true;

    private void OnCollisionExit2D(Collision2D collision) => isGrounded = false;
}
