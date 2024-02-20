using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField]
    private AudioData _appleSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ensure the player was hit
        if (!collision.gameObject.GetComponent<Player>()) return;

        // Handle the destruction of the object, along with recording the value in the game manager
        AudioHandler.instance.ProcessAudioData(_appleSound);
        GameManager.Instance.AddCoin();

        Destroy(gameObject);
    }
}
