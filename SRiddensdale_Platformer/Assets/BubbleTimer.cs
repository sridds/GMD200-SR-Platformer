using UnityEngine;
using TMPro;

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
        if (GameManager.Instance.IsGameOver)
        {
            _timerText.text = "";
            return;
        }

        if (GameManager.Instance.LocalPlayer.IsHanklingBubbled) {
            _timerText.text = (Mathf.CeilToInt(GameManager.Instance.LocalPlayer.HanklingTimer.RemainingTime)).ToString();

            _animator.speed += _speedIncreaseMultiplier * Time.deltaTime;
        }
        else {
            _timerText.text = "";
            _animator.speed = 1.0f;
        }
    }
}
