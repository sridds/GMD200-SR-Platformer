using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField]
    private string[] _lines;
    [SerializeField]
    private float _cooldown = 0.5f;

    public delegate void SignHover();
    public static SignHover OnSignHover;

    public delegate void SignInteract(string[] lines);
    public static SignInteract OnSignInteract;

    public delegate void SignExit();
    public static SignExit OnSignExit;

    private Timer cooldownTimer = null;
    private bool canInteract = false;

    private void Update()
    {
        if (cooldownTimer != null) cooldownTimer.Tick(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Z) && canInteract)
        {
            OnSignInteract?.Invoke(_lines);
            OnSignExit?.Invoke();
            canInteract = false;

            cooldownTimer = new Timer(_cooldown);
            cooldownTimer.OnTimerEnd += () => { cooldownTimer = null; };
        }
    }

    private bool CanInteract()
    {
        if (!canInteract || !CanInteractAtAll()) return false;
        if (!Input.GetKeyDown(KeyCode.Z)) return false;

        return true;
    }

    private bool CanInteractAtAll()
    {
        if (GameManager.Instance.IsGameOver || GameManager.Instance.IsLevelComplete || GameManager.Instance.LocalPlayer.IsHanklingBubbled) return false;
        return true;
    }

    /// <summary>
    /// Call the hover enter event
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanInteractAtAll()) return;

        if (collision.gameObject.tag != "Player") return;
        if (cooldownTimer != null) return;

        OnSignHover?.Invoke();
        canInteract = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        OnSignExit?.Invoke();
        canInteract = false;
    }
}
