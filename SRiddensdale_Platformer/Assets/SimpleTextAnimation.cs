using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTextAnimation : MonoBehaviour
{
    [SerializeField]
    private Vector2 _waveAmplitude;
    [SerializeField]
    private Vector2 _waveFrequency;
    [SerializeField]
    private RectTransform _target;

    private Vector2 startPos;

    void Start()
    {
        startPos = _target.anchoredPosition;
    }
    void Update()
    {
        _target.anchoredPosition = new Vector2(
            startPos.x + Mathf.Cos(Time.time * _waveFrequency.x) * _waveAmplitude.x,
            startPos.y + Mathf.Sin(Time.time * _waveFrequency.y) * _waveAmplitude.y);
    }
}
