using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject _deathWipe;
    [SerializeField]
    private Sprite _playerDeathSprite;
    [SerializeField]
    private SpriteRenderer _playerSpriteRenderer;

    [Header("Audio")]
    [SerializeField]
    private AudioData _deathScreenStartSound;
    [SerializeField]
    private AudioData _flyAwaySound;
    [SerializeField]
    private AudioData _hankSadSound;

    private void Start()
    {
        GameManager.Instance.OnGameOver += StartDeathScreen;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameOver -= StartDeathScreen;
    }

    private void StartDeathScreen() => StartCoroutine(IDeathScreen());

    private IEnumerator IDeathScreen()
    {
        // ensure time is frozen when dead
        GameManager.Instance.SetTimeScaleInstant(0.0f);
        AudioHandler.instance.ProcessAudioData(_deathScreenStartSound);

        // get references
        Player p = GameManager.Instance.LocalPlayer;
        GameObject bubble = FindObjectOfType<BubbleMovement>().gameObject;

        // set the death wipe to hide the screen entirely
        _deathWipe.transform.position = p.transform.position;
        _deathWipe.SetActive(true);

        yield return new WaitForSecondsRealtime(2.0f);
        AudioHandler.instance.ProcessAudioData(_flyAwaySound);

        float elapsed = 0.0f;
        float duration = 3.0f;
        float initial = bubble.transform.position.y;
        float targetY = p.transform.position.y + 20.0f;

        // lerp the bubble upwards into the sky
        while(elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            bubble.transform.position = new Vector2(bubble.transform.position.x, Mathf.Lerp(initial, targetY, elapsed / duration));

            yield return null;
        }

        AudioHandler.instance.ProcessAudioData(_hankSadSound);

        // disable the animator to allow for overriding the sprite
        p.GetComponent<Animator>().enabled = false;
        _playerSpriteRenderer.sprite = _playerDeathSprite;

        yield return new WaitForSecondsRealtime(2);
        // restart level
        GameManager.Instance.RestartLevel();
    }
}
