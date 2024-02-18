using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpriteAnimation : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private Sprite[] _sprites;
    [SerializeField]
    private float _timeBetweenFrames = 0.2f;
    [SerializeField]
    private bool _destroyOnComplete;

    private Timer currentFrameTimer;
    private int index = 0;


    void Update()
    {
        if (currentFrameTimer != null) currentFrameTimer.Tick(Time.deltaTime);
        else {
            // set timer
            currentFrameTimer = new Timer(_timeBetweenFrames);
            currentFrameTimer.OnTimerEnd += NextFrame;
        }
    }

    void NextFrame()
    {
        index++;
        index %= _sprites.Length;

        // set sprite
        _renderer.sprite = _sprites[index];
        currentFrameTimer = null;

        if (index == _sprites.Length - 1 && _destroyOnComplete) Destroy(gameObject);
    }
}
