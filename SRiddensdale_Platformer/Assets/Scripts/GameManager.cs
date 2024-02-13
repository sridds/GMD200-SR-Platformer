using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Instance reference
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    // Private variables
    private Vector3 savedPosition;
    private Player player;

    // Accessors
    public bool CheckpointSaved { get; private set; }
    public Vector3 SavedPosition { get { return savedPosition; } }
    public Player LocalPlayer { get { if (player == null) player = FindObjectOfType<Player>(); return player; } }

    private void Awake() {
        // create an instance of the game manager
        if(instance == null) {
            instance = this;
        }
        else if(instance != this){
            Destroy(gameObject);
        }

        // ensure this is not destroyed
        DontDestroyOnLoad(this);
    }

    private void OnSceneRestart() => RespawnAtCheckpoint();

    /// <summary>
    /// Respawns the player at the current active checkpoint
    /// </summary>
    private void RespawnAtCheckpoint()
    {
        // if theres no checkpoint to spawn at, dont bother
        if (!CheckpointSaved) return;
        LocalPlayer.transform.position = savedPosition;
    }

    public void SetCheckpoint(Vector3 point)
    {
        savedPosition = point;
        CheckpointSaved = true;
    }

    private void Update()
    {
        // for debugging
        if (Input.GetKeyDown(KeyCode.R)) RestartLevel();
    }

    public void RestartLevel() => StartCoroutine(RestartScene());

    private IEnumerator RestartScene()
    {
        var async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

        // wait until the async operation finishes
        while (!async.isDone) {
            yield return null;
        }

        OnSceneRestart();
    }
}
