using UnityEngine;

public class ReturnAudioToPool : MonoBehaviour
{
    private AudioSource source;
    private bool canReturn = false;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        ReturnToPool();
    }

    private void ReturnToPool() => canReturn = true;

    private void Update()
    {
        if (!canReturn || source.isPlaying) return;

        ObjectPooler.ReturnObjectToPool(gameObject);
    }
}