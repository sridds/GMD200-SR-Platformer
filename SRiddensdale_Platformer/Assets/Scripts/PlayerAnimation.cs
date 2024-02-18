using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerMovement movement;
    private Player player;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private bool flipX;
    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        movement = FindObjectOfType<PlayerMovement>();
        player = FindObjectOfType<Player>();

        movement.OnJump += CallJump;
        movement.OnLand += CallLand;
        movement.OnRPGReady += () => animator.SetTrigger("ReadyRPG");
        player.OnFireRPG += () => animator.SetTrigger("FireRPG");
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        PlayerMovement.Direction dir = movement.GetDirection();

        if(dir == PlayerMovement.Direction.Left) spriteRenderer.flipX = flipX ? true : false;
        else spriteRenderer.flipX = flipX ? false : true;

        if(movement.IsGrounded()) {
            animator.SetFloat("MoveSpeedX", Mathf.Abs(movement.MyBody.velocity.x) / movement.TopSpeed);
        }
        else {
            animator.SetFloat("MoveSpeedX", 0.0f);
        }

        if (player.IsHanklingBubbled) animator.SetBool("Carry", false);
        else animator.SetBool("Carry", true);
    }

    private void CallJump() => animator.SetTrigger("Jump");

    private void CallLand()
    {
        CameraShake.instance.Shake(0.15f, 0.2f);
        animator.SetTrigger("Land");
    }
}
