using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    bool checkpointActive = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        SetCheckpoint();
    }

    private void SetCheckpoint()
    {
        if (checkpointActive) return;

        // record the checkpoint in the game manager and set the active flag
        GameManager.Instance.SetCheckpoint(transform.position);
        checkpointActive = true;
    }
}
