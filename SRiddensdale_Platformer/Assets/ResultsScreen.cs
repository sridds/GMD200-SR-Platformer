using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsScreen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI _youCollectedText;
    [SerializeField]
    private TextMeshProUGUI _applesText;
    [SerializeField]
    private TextMeshProUGUI _thatsEnoughText;
    [SerializeField]
    private TextMeshProUGUI _itemNameText;

    [Header("VFX")]
    [SerializeField]
    private ParticleSystem _confetti;

    [Header("Meal")]
    [SerializeField]
    private SpriteRenderer _mealSprite;
    [SerializeField]
    private Sprite _wings;
    [SerializeField]
    private Sprite _onionRing;
    [SerializeField]
    private Sprite _mozzStick;
    [SerializeField]
    private Sprite _chicken;
    [SerializeField]
    private Sprite _fourCheeseMac;

    [Header("Audio")]
    [SerializeField]
    private AudioData _textAppearSound;

    [SerializeField]
    private AudioData _crowdGaspSound;

    [SerializeField]
    private AudioData _crowdAhSound;

    [SerializeField]
    private AudioData _drumRollSound;

    [SerializeField]
    private AudioData _clapSound;

    // Start is called before the first frame update
    void Start()
    {
        _youCollectedText.gameObject.SetActive(false);
        _applesText.gameObject.SetActive(false);
        _thatsEnoughText.gameObject.SetActive(false);
        _itemNameText.gameObject.SetActive(false);
        _mealSprite.gameObject.SetActive(false);

        StartCoroutine(ShowResultsScreen());
    }

    private IEnumerator ShowResultsScreen()
    {
        Debug.Log(PersistentData.lastLevelID);
        PersistentData.LevelData data = PersistentData.GetSceneData(PersistentData.lastLevelID);

        yield return new WaitForSeconds(0.5f);
        _youCollectedText.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_textAppearSound);
        yield return new WaitForSeconds(0.5f);
        AudioHandler.instance.ProcessAudioData(_drumRollSound);

        yield return new WaitForSeconds(2f);
        AudioHandler.instance.ProcessAudioData(_textAppearSound);
        _confetti.Play();

        _applesText.gameObject.SetActive(true);
        _applesText.text = $"{data.coins} APPLES";
        yield return new WaitForSeconds(0.5f);
        AudioHandler.instance.ProcessAudioData(_clapSound);
        yield return new WaitForSeconds(2f);
        AudioHandler.instance.ProcessAudioData(_textAppearSound);
        _thatsEnoughText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        AudioHandler.instance.ProcessAudioData(_textAppearSound);

        AudioData audio = data.coins == 0 ? _crowdGaspSound : _crowdAhSound;

        _mealSprite.gameObject.SetActive(true);
        if (data.coins == 0)
        {
            _itemNameText.text = "NOTHING.";
        }
        else if (data.coins <= 5)
        {
            _itemNameText.text = "WINGS";
            _mealSprite.sprite = _wings;
        }
        else if (data.coins <= 10)
        {
            _itemNameText.text = "ONION RINGS";
            _mealSprite.sprite = _onionRing;
        }
        else if (data.coins <= 15)
        {
            _itemNameText.text = "MOZZ STICKS";
            _mealSprite.sprite = _mozzStick;
        }
        else if (data.coins <= 19)
        {
            _itemNameText.text = "CHICKEN TENDERS";
            _mealSprite.sprite = _chicken;
        }
        else
        {
            _itemNameText.text = "FOUR CHEESE MAC AND CHEESE WITH HONEY PEPPER CHICKEN TENDERS BABY";
            _mealSprite.sprite = _fourCheeseMac;
        }

        yield return new WaitForSeconds(0.5f);
        _itemNameText.gameObject.SetActive(true);
        AudioHandler.instance.ProcessAudioData(_textAppearSound);
        AudioHandler.instance.ProcessAudioData(audio);

        yield return new WaitForSeconds(4.0f);
        AudioHandler.instance.FadeMusic(2.0f, 0.0f);
        yield return new WaitForSeconds(2.0f);

        // load next scene
        SceneManager.LoadScene(data.levelID + 1);
    }
}
