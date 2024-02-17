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

    private void Start()
    {
        GameManager.Instance.OnGameOver += StartDeathScreen;
    }

    private void StartDeathScreen() => StartCoroutine(IDeathScreen());

    private IEnumerator IDeathScreen()
    {
        GameManager.Instance.SetTimeScaleInstant(0.0f);

        Player p = GameManager.Instance.LocalPlayer;

        _deathWipe.transform.position = p.transform.position;
        _deathWipe.SetActive(true);

        yield return new WaitForSecondsRealtime(2.0f);

        p.GetComponent<Animator>().enabled = false;
        _playerSpriteRenderer.sprite = _playerDeathSprite;
    }
}
