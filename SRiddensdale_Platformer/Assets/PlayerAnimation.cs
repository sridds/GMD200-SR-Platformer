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

    private void Awake()
    {
        movement = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        PlayerMovement.Direction dir = movement.GetDirection();

        if(dir == PlayerMovement.Direction.Left) spriteRenderer.flipX = flipX ? true : false;
        else spriteRenderer.flipX = flipX ? false : true;
    }
}
