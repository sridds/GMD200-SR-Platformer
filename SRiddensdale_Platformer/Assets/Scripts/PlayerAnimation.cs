using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerMovement movement;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private bool flipX;
    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        movement = FindObjectOfType<PlayerMovement>();

        movement.OnJump += CallJump;
        movement.OnLand += CallLand;
    }

    private void Update()
    {
        PlayerMovement.Direction dir = movement.GetDirection();

        if(dir == PlayerMovement.Direction.Left) spriteRenderer.flipX = flipX ? true : false;
        else spriteRenderer.flipX = flipX ? false : true;

        if(movement.IsGrounded()) {
            animator.SetFloat("MoveSpeedX", Mathf.Abs(movement.MyBody.velocity.x) / movement.TopSpeed);
        }
        else {
            animator.SetFloat("MoveSpeedX", 0.0f);
        }
    }

    private void CallJump() => animator.SetTrigger("Jump");

    private void CallLand() => animator.SetTrigger("Land");
}