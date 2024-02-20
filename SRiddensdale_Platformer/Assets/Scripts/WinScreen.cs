using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _winText;
    [SerializeField]
    private TextMeshProUGUI _subtext;
    [SerializeField]
    private TextMeshProUGUI _musicText;
    [SerializeField]
    private TextMeshProUGUI _cantLeaveText;
    [SerializeField]
    private ParticleSystem _confetti;
    [SerializeField]
    private AudioData _textAppearSound;
    [SerializeField]
    private AudioData _clapSound;

    private void Start() => StartCoroutine(Win());

    private IEnumerator Win()
    {
        _winText.gameObject.SetActive(false);
        _subtext.gameObject.SetActive(false);
        _musicText.gameObject.SetActive(false);
        _cantLeaveText.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        _winText.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_textAppearSound);

        yield return new WaitForSeconds(0.5f);
        AudioHandler.instance.ProcessAudioData(_clapSound);
        _confetti.Play();

        yield return new WaitForSeconds(2.0f);
        _subtext.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);
        _musicText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);
        _cantLeaveText.gameObject.SetActive(true);

        yield return new WaitForSeconds(75);
        Debug.Log("music over");

        SceneManager.LoadScene(0);
    }
}
