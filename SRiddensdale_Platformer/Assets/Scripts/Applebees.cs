using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Applebees : MonoBehaviour
{
    [SerializeField]
    private AudioData _fanfare;
    [SerializeField]
    private Sprite _playerProudSprite;
    [SerializeField]
    private Sprite _playerSuperProudSprite;
    [SerializeField]
    private Transform _enterence;
    [SerializeField]
    private AudioData _enterApplebeesSound;
    [SerializeField]
    private AudioData _yippie;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.IsLevelComplete) return;
        // must be the player
        if (!collision.gameObject.TryGetComponent<Player>(out Player player)) return;
        if (player.IsHanklingBubbled) return; // cannot cheat!

        // ensure the game manager knows the level was completed
        GameManager.Instance.CallLevelComplete();
        StartCoroutine(IPlayWinAnimation());
    }

    private IEnumerator IPlayWinAnimation()
    {
        Player player = GameManager.Instance.LocalPlayer;
        //Time.timeScale = 0.0f;
        AudioHandler.instance.PauseMusic();

        player.Movement.FreezeInputAndVelocity();
        CameraMovement cam = FindObjectOfType<CameraMovement>();
        cam.FreezeCamera(true);

        AudioHandler.instance.ProcessAudioData(_fanfare);

        for(int i = 0; i < 4; i++)
        {
            AudioHandler.instance.ProcessAudioData(_yippie);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        StartCoroutine(ILerpObject(cam.transform, new Vector2(transform.position.x, cam.transform.position.y), 4.0f, false, true));

        yield return new WaitForSecondsRealtime(1.0f);
        player.GetComponent<Animator>().enabled = false;
        player.MyRenderer.sprite = _playerProudSprite;

        Hankling hankling = player.MyHankling;
        hankling.SetFollowPlayer(false);

        StartCoroutine(ILerpObject(hankling.transform, new Vector2(player.transform.position.x + 1.0f, player.transform.position.y), 1.0f, false, true));
        yield return new WaitForSecondsRealtime(1.0f);
        StartCoroutine(ILerpObject(hankling.transform, _enterence.transform.position, 2.0f, false, true));
        player.MyRenderer.sprite = _playerSuperProudSprite;

        yield return new WaitForSecondsRealtime(2.5f);

        AudioHandler.instance.ProcessAudioData(_enterApplebeesSound);
        hankling.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(2.0f);
        GameManager.Instance.ResultsScreen();
    }

    private IEnumerator ILerpObject(Transform transform, Vector2 lerpPos, float time, bool addToCurrent, bool useUnscaledTime = false)
    {
        float elapsed = 0.0f;
        Vector2 initial = transform.position;
        Vector2 final = addToCurrent ? initial + lerpPos : lerpPos;

        while(elapsed < time)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            transform.position = Vector2.Lerp(initial, final, elapsed / time);

            yield return null;
        }

        transform.position = final;
    }
}
