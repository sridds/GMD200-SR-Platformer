using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private int _damage = 25;

    [SerializeField]
    private GameObject _explosionEffect;

    [SerializeField]
    private AudioData _hitSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bubble")
        {
            Bubble b = collision.gameObject.GetComponent<Bubble>();

            // pop bubble if the rocket hit the bubble
            b.MyHankling.ExitBubble();
        }

        CameraShake.instance.Shake(0.8f, 0.45f);

        // any other damageable will just take damage
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damage)) damage.TakeDamage(_damage);

        // process the hit sound and destroy
        AudioHandler.instance.ProcessAudioData(_hitSound);
        Instantiate(_explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
