using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Bubble Movement")]
    [SerializeField]
    private float _minDistanceAboveGround = 5.0f;
    [SerializeField]
    private float _maxDistanceAboveGround = 7.0f;
    [SerializeField]
    private float _sinFrequency = 2.0f;
    [SerializeField]
    private float _bubbleNoiseAmplitude = 0.5f;
    [SerializeField]
    private float _bubbleNoiseFrequency = 0.05f;
    [SerializeField]
    private float _ySmoothing = 0.1f;
    [SerializeField]
    private float _noiseSmoothing = 0.25f;
    [SerializeField]
    private float _xSpeed = 2f;
    [SerializeField]
    private LayerMask _groundLayer;

    private float timer = 0.0f;

    void Start() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        timer += Time.deltaTime;
    }

    float minY = 0.0f;
    float velY = 0.0f;
    Vector2 targetNoise;
    Vector2 noiseVel;
    Vector2 smoothedNoise;

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, _groundLayer);
        if (hit.collider != null) minY = hit.point.y;

        // calculate y
        float y = minY + _minDistanceAboveGround + Mathf.Sin(Time.time * (_maxDistanceAboveGround - _minDistanceAboveGround)) * _sinFrequency;
        smoothedNoise = Vector2.SmoothDamp(smoothedNoise, targetNoise, ref noiseVel, _noiseSmoothing);
        transform.position = new Vector2(transform.position.x + smoothedNoise.x, Mathf.SmoothDamp(transform.position.y, y, ref velY, _ySmoothing) + smoothedNoise.y);

        // choose new target noise
        if(timer >= _bubbleNoiseFrequency) {
            targetNoise = (Random.insideUnitCircle * _bubbleNoiseAmplitude) * Time.fixedDeltaTime;
            timer = 0.0f;
        }
    }
}
