using UnityEngine;
using TMPro;

/// <summary>
/// This class handles the bubble UI
/// </summary>
public class BubbleTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _timerText;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float _speedIncreaseMultiplier = 0.2f;

    void Update()
    {
        // ensure text is hidden when game over
        if (GameManager.Instance.IsGameOver)
        {
            _timerText.text = "";
            return;
        }

        if (GameManager.Instance.LocalPlayer.IsHanklingBubbled) {
            // set text to timer
            _timerText.text = (Mathf.CeilToInt(GameManager.Instance.LocalPlayer.HanklingTimer.RemainingTime)).ToString();

            // add to speed overtime
            _animator.speed += _speedIncreaseMultiplier * Time.deltaTime;
        }
        else {
            // hide and reset speed
            _timerText.text = "";
            _animator.speed = 1.0f;
        }
    }
}
