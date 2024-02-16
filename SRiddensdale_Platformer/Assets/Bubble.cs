using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public delegate void BubbleTriggered();
    public BubbleTriggered OnBubbleTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // must be the player
        if (collision.tag != "Player") return;

        OnBubbleTriggered?.Invoke();
    }
}
