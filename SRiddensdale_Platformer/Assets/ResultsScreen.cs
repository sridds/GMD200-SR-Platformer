using System.Collections;
using UnityEngine;
using TMPro;

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
        _itemNameText.gameObject.SetActive(true);

        _mealSprite.gameObject.SetActive(true);

        if (data.coins == 0) {
            _itemNameText.text = "NOTHING.";
            AudioHandler.instance.ProcessAudioData(_crowdGaspSound);
        } else if(data.coins <= 5) {
            _itemNameText.text = "WINGS";
            _mealSprite.sprite = _wings;
            AudioHandler.instance.ProcessAudioData(_crowdAhSound);
        }
        else if (data.coins <= 10)
        {
            _itemNameText.text = "ONION RINGS";
            _mealSprite.sprite = _onionRing;
            AudioHandler.instance.ProcessAudioData(_crowdAhSound);
        }
        else if (data.coins <= 15)
        {
            _itemNameText.text = "MOZZ STICKS";
            _mealSprite.sprite = _mozzStick;
            AudioHandler.instance.ProcessAudioData(_crowdAhSound);
        }
        else if (data.coins <= 20)
        {
            _itemNameText.text = "CHICKEN TENDERS";
            _mealSprite.sprite = _chicken;
            AudioHandler.instance.ProcessAudioData(_crowdAhSound);
        }
        else
        {
            _itemNameText.text = "FOUR CHEESE MAC AND CHEESE WITH HONEY PEPPER CHICKEN TENDERS BABY";
            _mealSprite.sprite = _fourCheeseMac;
            AudioHandler.instance.ProcessAudioData(_crowdAhSound);
        }
    }
}
