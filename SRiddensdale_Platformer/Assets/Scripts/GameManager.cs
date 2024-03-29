using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Playing,
        Paused
    }

    // constants
    private const int RESULTS_SCENE_INDEX = 5;

    // Instance reference
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    // Private variables
    private Vector3 savedPosition;
    private Player player;

    // Accessors
    public int Coins { get; private set; }
    public bool CheckpointSaved { get; private set; }
    public Vector3 SavedPosition { get { return savedPosition; } }
    public Player LocalPlayer { get { if (player == null) player = FindObjectOfType<Player>(); return player; } }
    public GameState CurrentGameState { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsLevelComplete { get; private set; }
    public bool IsSignInteracting { get; private set; }
    public float TimePlaying { get; private set; }

    private float timeScaleBeforePause;

    // delegates
    public delegate void GameStateChanged(GameState state);
    public GameStateChanged OnGameStateChanged;

    public delegate void GameOver();
    public GameOver OnGameOver;

    public delegate void LevelComplete();
    public LevelComplete OnLevelComplete;

    public delegate void CoinIncrease();
    public CoinIncrease OnCoinIncrease;

    private void Awake() {
        // create an instance of the game manager
        if(instance == null) {
            instance = this;
        }
        else if(instance != this){
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SubscribeToEvents();

        // ensure timescale is reset
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Subscribes to any necessary events to function
    /// </summary>
    private void SubscribeToEvents()
    {
        // subscribe to static sign event
        Sign.OnSignHover += SignHover;
        Sign.OnSignExit += SignExitHover;

        // get reference to dialogue box
        DialogueBox box = FindObjectOfType<DialogueBox>();

        // subscribe to box events
        if(box != null)
        {
            FindObjectOfType<DialogueBox>().OnDialogueStart += SignInteract;
            FindObjectOfType<DialogueBox>().OnDialogueFinish += SignEndInteraction;
        }
    }

    private void OnSceneLoad()
    {
        SubscribeToEvents();

        // reset all values
        Time.timeScale = 1.0f;
        IsGameOver = false;
        IsLevelComplete = false;
        player = null;
        TimePlaying = 0.0f;

        RespawnAtCheckpoint();
    }

    /// <summary>
    /// Adds a coin
    /// </summary>
    public void AddCoin()
    {
        Coins++;

        OnCoinIncrease?.Invoke();
    }

    /// <summary>
    /// Set interacting flag
    /// </summary>
    private void SignHover() => IsSignInteracting = true;

    /// <summary>
    /// Pauses the game and ensures nothing else can interfere with the player interacting with the sign
    /// </summary>
    private void SignInteract()
    {
        CurrentGameState = GameState.Paused;

        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Conclude the sign interaction, reseting the state back to what it was before
    /// </summary>
    private void SignEndInteraction()
    {
        CurrentGameState = GameState.Playing;
        Time.timeScale = timeScaleBeforePause;
    }


    private void SignExitHover() => IsSignInteracting = false;

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

        TimePlaying += Time.deltaTime;
    }

    /// <summary>
    /// Go to results screen
    /// </summary>
    public void ResultsScreen() {
        StartCoroutine(LoadScene(RESULTS_SCENE_INDEX));
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void RestartLevel() {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    /// Handles the asynchrounous loading of the provided scene index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator LoadScene(int index)
    {
        UnsubscribeEvents();

        if (IsLevelComplete) {
            // add new level data with corresponding coins and build index
            PersistentData.AddNewData(Coins, SceneManager.GetActiveScene().buildIndex, TimePlaying);
        }

        FindObjectOfType<ScreenWiper>().WipeIn(0.3f);
        yield return new WaitForSecondsRealtime(1.0f);

        var async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);

        // wait until the async operation finishes
        while (!async.isDone) {
            yield return null;
        }

        OnSceneLoad();
    }

    /// <summary>
    /// unsubscribe from all events
    /// </summary>
    private void UnsubscribeEvents()
    {
        Sign.OnSignHover -= SignHover;
        Sign.OnSignExit -= SignExitHover;

        FindObjectOfType<DialogueBox>().OnDialogueStart -= SignInteract;
        FindObjectOfType<DialogueBox>().OnDialogueFinish -= SignEndInteraction;
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
