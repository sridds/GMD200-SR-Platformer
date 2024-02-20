using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    [SerializeField]
    private AudioData _appleSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.GetComponent<Player>()) return;

        AudioHandler.instance.ProcessAudioData(_appleSound);
        GameManager.Instance.AddCoin();
        Destroy(gameObject);
    }
}
