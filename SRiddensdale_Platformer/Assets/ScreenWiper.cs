using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenWiper : MonoBehaviour
{    
    public void WipeOut(float time)
    {
        transform.localPosition = Vector2.zero;
        StartCoroutine(Wipe(time, 17));
    }
    public void WipeIn(float time)
    {
        transform.localPosition = new Vector2(-17, transform.localPosition.y);
        StartCoroutine(Wipe(time, 0));
    }

    private void Start()
    {
        transform.localPosition = Vector2.zero;
        StartCoroutine(Wipe(0.5f, 17));
    }

    IEnumerator Wipe(float time, float xPos)
    {
        yield return null;

        float elapsed = 0.0f;
        float initX = transform.localPosition.x;

        while(elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            transform.localPosition = new Vector2(Mathf.Lerp(initX, xPos, elapsed / time), transform.localPosition.y);

            yield return null;
        }

        transform.localPosition = new Vector2(xPos, transform.localPosition.y);
    }
}
