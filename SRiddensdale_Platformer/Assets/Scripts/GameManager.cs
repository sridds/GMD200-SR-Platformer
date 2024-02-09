using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    private Vector3 savedPosition;
    public Vector3 SavedPosition { get { return savedPosition; } }

    private void Awake() {
        // create an instance of the game manager
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }

        // ensure this is not destroyed
        DontDestroyOnLoad(this);
    }

    public void SetCheckpoint(Vector3 point)
    {
        savedPosition = point;   
    }
}
