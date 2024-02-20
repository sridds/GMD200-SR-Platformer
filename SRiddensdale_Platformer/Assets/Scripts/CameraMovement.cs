using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector2 timeOffset;
    public Vector3 offsetPos;

    public Vector3 boundsMin;
    public Vector3 boundsMax;

    private PlayerMovement player;
    private bool frozen = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void LateUpdate()
    {
        if (frozen) return;

        if (player != null)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = player.transform.position;

            Vector2 offset = new Vector2(offsetPos.x, offsetPos.y);

            targetPos.x += offset.x;
            targetPos.y += offset.y;
            targetPos.z = transform.position.z;

            targetPos.x = Mathf.Clamp(targetPos.x, boundsMin.x, boundsMax.x);
            targetPos.y = Mathf.Clamp(targetPos.y, boundsMin.y, boundsMax.y);

            float tx = 1f - Mathf.Pow(1f - timeOffset.x, Time.deltaTime * 30);
            float ty = 1f - Mathf.Pow(1f - timeOffset.y, Time.deltaTime * 30);

            Vector2 newPos = new Vector2(Mathf.Lerp(startPos.x, targetPos.x, tx), Mathf.Lerp(startPos.y, targetPos.y, ty));
            transform.position = new Vector3(newPos.x, newPos.y, targetPos.z);
        }
    }

    public void FreezeCamera(bool frozen) => this.frozen = frozen;
}
