using UnityEngine;
using TMPro;

public class SignInteractionHint : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private string _hintText = "PRESS Z TO INTERACT";

    void Start()
    {
        Sign.OnSignHover += ShowHint;
        Sign.OnSignExit += HideHint;

        _text.text = "";
    }

    private void ShowHint() => _text.text = _hintText;
    private void HideHint() => _text.text = "";
}
