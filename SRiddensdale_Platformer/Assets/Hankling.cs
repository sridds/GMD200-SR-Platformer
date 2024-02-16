using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hankling : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        // hankling collected!
        Debug.Log("Hankling Collected");
    }
}
