using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject _logo;
    [SerializeField]
    private GameObject _startText;
    [SerializeField]
    private GameObject _creditText;

    [SerializeField]
    private AudioData _logoAppearSound;
    [SerializeField]
    private AudioData _startGameSound;
    [SerializeField]
    private AudioData _selectSound;
    [SerializeField]
    private AudioData _textAppear;

    [SerializeField]
    private Animator _startTextAnimator;

    private bool canStartGame;

    void Start()
    {
        _logo.SetActive(false);
        _startText.SetActive(false);
        _creditText.SetActive(false);

        StartCoroutine(ShowTitle());
    }

    private void Update()
    {
        if (!canStartGame) return;

        if (Input.GetKeyDown(KeyCode.Z)) {
            canStartGame = false;

            AudioHandler.instance.ProcessAudioData(_startGameSound);
            AudioHandler.instance.ProcessAudioData(_selectSound);

            Time.timeScale = 0.0f;
            _startTextAnimator.SetTrigger("Start");

            StartCoroutine(StartGame());
        }
    }

    private IEnumerator ShowTitle()
    {
        yield return new WaitForSeconds(1);
        _logo.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_logoAppearSound);
        yield return new WaitForSeconds(2.0f);

        AudioHandler.instance.ProcessAudioData(_textAppear);
        _startText.SetActive(true);
        _creditText.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        canStartGame = true;
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        FindObjectOfType<ScreenWiper>().WipeIn(0.7f);

        yield return new WaitForSecondsRealtime(0.7f);
        SceneManager.LoadScene(1);
    }
}
