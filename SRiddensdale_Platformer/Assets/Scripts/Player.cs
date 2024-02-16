using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Hankling myHankling;

    // accessors
    public Hankling MyHankling { get { if (myHankling == null) myHankling = FindObjectOfType<Hankling>(); return myHankling; } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // restart level if hit hazard
        if (collision.gameObject.tag == "Hazard") GameManager.Instance.RestartLevel();
    }
}
