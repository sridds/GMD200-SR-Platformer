using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField]
    private string[] _lines;

    public delegate void SignHover();
    public static SignHover OnSignHover;

    public delegate void SignInteract(string[] lines);
    public static SignInteract OnSignInteract;

    public delegate void SignExit();
    public static SignExit OnSignExit;

    /// <summary>
    /// Call the hover enter event
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        OnSignHover?.Invoke();
    }

    /// <summary>
    /// Allow the player to interact with the sign
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        if (Input.GetKeyDown(KeyCode.Z)) OnSignInteract?.Invoke(_lines);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        OnSignExit?.Invoke();
    }
}
