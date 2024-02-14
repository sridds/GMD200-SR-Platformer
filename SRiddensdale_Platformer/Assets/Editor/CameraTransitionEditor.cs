using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(CameraTransition))]
public class CameraTransitionEditor : Editor
{
    private void OnSceneGUI()
    {
        CameraTransition transition = (CameraTransition)target;

        if (Camera.main == null)
        {
            Debug.LogWarning("No camera found!");
            return;
        }

        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));



        // resolution
        float width = 256.0f / 16.0f;
        float height = 240.0f / 16.0f;

        Vector2 pos = new Vector2((transition.transform.position.x - (width / 2)) + (transition.cameraMinPosition.x - transition.transform.position.x), transition.transform.position.y - (height / 2) + (transition.cameraMinPosition.y - transition.transform.position.y));
        Rect rect = new Rect { position = pos, width = (256 / 16) + (transition.cameraMaxPosition.x - transition.cameraMinPosition.x), height = (240 / 16f) + (transition.cameraMaxPosition.y - transition.cameraMinPosition.y) };

        Handles.DrawSolidRectangleWithOutline(rect, new Color(0.5f, 0.5f, 0.5f, 0.1f), Color.yellow);
    }
}
