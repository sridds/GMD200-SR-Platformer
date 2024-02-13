using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float _moveTime;

    [SerializeField]
    private Transform[] _movePoints;

    private int pointIndex = 0;
    private Transform currentPoint;

    private Coroutine activeMoveCoroutine;

    void Start() => currentPoint = _movePoints[pointIndex];

    private void Update()
    {
        if (activeMoveCoroutine == null) activeMoveCoroutine = StartCoroutine(HandlePoint());
    }

    private IEnumerator HandlePoint()
    {
        float elapsed = 0.0f;
        float duration = _moveTime;

        Vector2 initial = transform.position;
        Vector2 target = currentPoint.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector2.Lerp(initial, target, elapsed / duration);

            yield return null;
        }

        transform.position = target;

        pointIndex++;
        pointIndex %= _movePoints.Length;
        currentPoint = _movePoints[pointIndex];

        activeMoveCoroutine = null;
    }
}
