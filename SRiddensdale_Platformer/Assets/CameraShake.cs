using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private Camera target;

    private float shakePower = 0;
    private float shakeTimeRemaining = 0;
    private float shakeFadeTime = 0;

    private Vector3 origin;

    private void Awake()
    {
        instance = this;
        origin = target.transform.localPosition;
    }

    private void LateUpdate()
    {
        // don't shake while paused
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused) return;

        if (shakeTimeRemaining > 0.0f)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmt = Random.Range(-1f, 1f) * shakePower;
            float yAmt = Random.Range(-1f, 1f) * shakePower;

            target.transform.localPosition = new Vector3(origin.x + xAmt, origin.y + yAmt, origin.z);

            shakePower = Mathf.MoveTowards(shakePower, 0.0f, shakeFadeTime * Time.deltaTime);
        }
    }

    public void Shake(float power, float length)
    {
        shakePower = power;
        shakeTimeRemaining = length;

        shakeFadeTime = power / length;
    }
}
