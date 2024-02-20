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
        if (GameManager.Instance.IsLevelComplete) return; // guard clause to ensure the level isnt already complete
        if (!collision.gameObject.TryGetComponent<Player>(out Player player)) return; // must be the player
        if (player.IsHanklingBubbled) return; // cannot cheat!

        // ensure the game manager knows the level was completed
        GameManager.Instance.CallLevelComplete();
        StartCoroutine(IPlayWinAnimation());
    }

    /// <summary>
    /// Handles the win animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator IPlayWinAnimation()
    {
        Player player = GameManager.Instance.LocalPlayer;
        AudioHandler.instance.PauseMusic();

        // Freeze the player
        player.Movement.FreezeInputAndVelocity();

        // Freeze the camera
        CameraMovement cam = FindObjectOfType<CameraMovement>();
        cam.FreezeCamera(true);

        AudioHandler.instance.ProcessAudioData(_fanfare);

        // play the yippie sound effect four times
        for(int i = 0; i < 4; i++)
        {
            AudioHandler.instance.ProcessAudioData(_yippie);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        // Pan the camera over to applebees
        StartCoroutine(ILerpObject(cam.transform, new Vector2(transform.position.x, cam.transform.position.y), 4.0f, false, true));

        yield return new WaitForSecondsRealtime(1.0f);

        // disable the animator to allow for overriding sprites
        player.GetComponent<Animator>().enabled = false;
        player.MyRenderer.sprite = _playerProudSprite;

        // get the hankling and disable follow
        Hankling hankling = player.MyHankling;
        hankling.SetFollowPlayer(false);

        // manually move the hankling to the ground and to applebees entrence
        StartCoroutine(ILerpObject(hankling.transform, new Vector2(player.transform.position.x + 1.0f, player.transform.position.y), 1.0f, false, true));
        yield return new WaitForSecondsRealtime(1.0f);
        StartCoroutine(ILerpObject(hankling.transform, _enterence.transform.position, 2.0f, false, true));

        // set sprite of player
        player.MyRenderer.sprite = _playerSuperProudSprite;
        yield return new WaitForSecondsRealtime(2.5f);

        // enter applebees
        AudioHandler.instance.ProcessAudioData(_enterApplebeesSound);
        hankling.gameObject.SetActive(false);

        // display results screen
        yield return new WaitForSecondsRealtime(2.0f);
        GameManager.Instance.ResultsScreen();
    }

    /// <summary>
    /// Helper function to lerp a transform from its initial position to a specified lerpPos
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="lerpPos"></param>
    /// <param name="time"></param>
    /// <param name="addToCurrent"></param>
    /// <param name="useUnscaledTime"></param>
    /// <returns></returns>
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
