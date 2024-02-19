using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Paused
    }

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
    public GameState CurrentGameState { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsLevelComplete { get; private set; }

    // delegates
    public delegate void GameStateChanged(GameState state);
    public GameStateChanged OnGameStateChanged;

    public delegate void GameOver();
    public GameOver OnGameOver;

    public delegate void LevelComplete();
    public LevelComplete OnLevelComplete;

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

    private void OnSceneRestart()
    {
        Time.timeScale = 1.0f;
        IsGameOver = false;
        IsLevelComplete = false;
        player = null;

        RespawnAtCheckpoint();
    }
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

    /// <summary>
    /// Called externally to let other scripts perform the necessary logic when the game is over
    /// </summary>
    public void CallGameOver()
    {
        IsGameOver = true;

        OnGameOver?.Invoke();
    }

    /// <summary>
    /// Externally called to let other scripts perform needed logic when the level is finished
    /// </summary>
    public void CallLevelComplete()
    {
        IsLevelComplete = true;

        OnLevelComplete?.Invoke();
    }

    private void Update()
    {
        // for debugging
        if (Input.GetKeyDown(KeyCode.R)) RestartLevel();
    }

    public void RestartLevel() => StartCoroutine(RestartScene());

    private IEnumerator RestartScene()
    {
        FindObjectOfType<ScreenWiper>().WipeIn(0.3f);
        yield return new WaitForSecondsRealtime(1.0f);

        var async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

        // wait until the async operation finishes
        while (!async.isDone) {
            yield return null;
        }

        OnSceneRestart();
    }

    /// <summary>
    /// calls the lerp time scale coroutine to lerp timeScale over a period lerpTime
    /// </summary>
    /// <param name="timeScale"></param>
    /// <param name="lerpTime"></param>
    public void SetTimeScale(float timeScale, float lerpTime)
    {
        StopAllCoroutines();
        StartCoroutine(LerpTimeScale(timeScale, lerpTime));
    }

    /// <summary>
    /// Instantly sets time scale. This may seem redundant but I want the game manager to handle this in case pausing ever gets implemented
    /// </summary>
    /// <param name="timeScale"></param>
    public void SetTimeScaleInstant(float timeScale)
    {
        if (CurrentGameState == GameState.Paused) return;
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// Responsible for lerping the time scale to the passed timeScale variable over a period defined by lerpTime
    /// </summary>
    /// <param name="timeScale"></param>
    /// <param name="lerpTime"></param>
    /// <returns></returns>
    private IEnumerator LerpTimeScale(float timeScale, float lerpTime)
    {
        float elapsedTime = 0.0f;
        float initialTimeScale = Time.timeScale;

        while (elapsedTime < lerpTime)
        {
            if (CurrentGameState == GameState.Paused)
            {
                // ensure the timescale is 0 while paused
                Time.timeScale = 0.0f;
                yield return null;
            }
            else
            {
                // adjust the time scale, ensure that elapsed time uses unscaled time since we are directly modifying the time scale
                Time.timeScale = Mathf.Lerp(initialTimeScale, timeScale, elapsedTime / lerpTime);
                elapsedTime += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        // just in case it overshoots or undershoots
        Time.timeScale = timeScale;
    }
}
