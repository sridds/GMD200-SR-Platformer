using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    [SerializeField]
    private float _yForce = 20;
    [SerializeField]
    private float _superYForce = 45;
    [SerializeField]
    private float _springBuffer = 0.4f;
    [SerializeField]
    private AudioData _springAudio;
    [SerializeField]
    private AudioData _superSpringAudio;

    private float bufferTime = 0.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) bufferTime = _springBuffer;

        if (bufferTime > 0) bufferTime -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player)) return;

        // spring
        if (bufferTime > 0.0f)
        {
            player.MyBody.velocity = new Vector2(player.MyBody.velocity.x, _superYForce);
            AudioHandler.instance.ProcessAudioData(_superSpringAudio);
            CameraShake.instance.Shake(0.7f, 0.35f);
        }
        else
        {
            player.MyBody.velocity = new Vector2(player.MyBody.velocity.x, _yForce);
            AudioHandler.instance.ProcessAudioData(_springAudio);
            CameraShake.instance.Shake(0.5f, 0.25f);
        }
        bufferTime = 0.0f;
        player.CallJumpFlags();
    }
}
