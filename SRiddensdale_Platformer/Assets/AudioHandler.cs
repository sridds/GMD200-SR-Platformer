using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler instance;

    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundPrefab;

    // Holds a queue of music volume actions
    private Queue<IEnumerator> musicCoroutineQueue = new Queue<IEnumerator>();
    private Coroutine activeMusicCoroutine = null;

    private float prePauseVolume;

    /// <summary>
    /// Sets up the instance
    /// </summary>
    private void Awake() => instance = this;

    private void Start()
    {
        if(GameManager.Instance != null) GameManager.Instance.OnGameStateChanged += UpdateMusicState;
    }

    private void UpdateMusicState(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Paused) PauseMusic();
        else musicSource.volume = prePauseVolume;
    }

    public void PauseMusic() {
        prePauseVolume = musicSource.volume;
        musicSource.volume = 0.0f;
    }

    private void Update()
    {
        UpdateMusicQueue();
    }

    private void UpdateMusicQueue()
    {
        // Continue going through the queue of music actions
        if (musicCoroutineQueue.Count > 0 && activeMusicCoroutine == null)
        {
            activeMusicCoroutine = StartCoroutine(musicCoroutineQueue.Dequeue());
        }
    }

    /// <summary>
    /// Changes the currently active music track.
    /// </summary>
    /// <param name="newTrack"></param>
    /// <param name="playAutomatically"></param>
    public void ChangeTrack(AudioClip newTrack, bool playAutomatically = false)
    {
        // Pauses the current music track and sets the clip to the new track
        musicSource.Pause();
        musicSource.clip = newTrack;

        // Play the music source if specified to play automatically
        if (playAutomatically) musicSource.Play();
    }

    /// <summary>
    /// Fades out current music track and fades in new music track
    /// </summary>
    /// <param name="newTrack"></param>
    /// <param name="fadeInTime"></param>
    /// <param name="fadeOutTime"></param>
    public void FadeChangeTrack(AudioClip newTrack, float fadeInTime, float fadeOutTime) => musicCoroutineQueue.Enqueue(IFadeChangeTrack(newTrack, fadeInTime, fadeOutTime));

    /// <summary>
    /// Fades two music tracks at the same time
    /// </summary>
    /// <param name="newTrack"></param>
    /// <param name="fadeInTime"></param>
    /// <param name="fadeOutTime"></param>
    public void CrossFadeTrack(AudioClip newTrack, float fadeInTime, float fadeOutTime) => musicCoroutineQueue.Enqueue(ICrossFadeTrack(newTrack, fadeInTime, fadeOutTime));

    /// <summary>
    /// Fades out the track that is currently playing
    /// </summary>
    /// <param name="time"></param>
    public void FadeMusic(float time, float volume) => musicCoroutineQueue.Enqueue(IFadeToVolume(musicSource, time, volume, true));

    public void FadeToPitch(float time, float pitch, bool useUnscaled = false) => StartCoroutine(IFadeToPitch(time, pitch, useUnscaled));


    private IEnumerator IFadeToPitch(float time, float pitch, bool useUnscaled)
    {
        float elapsed = 0.0f;
        float initial = musicSource.pitch;

        // lerp pitch
        while(elapsed < time)
        {
            musicSource.pitch = Mathf.Lerp(initial, pitch, elapsed / time);

            if (useUnscaled) elapsed += Time.unscaledDeltaTime;
            else elapsed += Time.deltaTime;

            yield return null;
        }

        musicSource.pitch = pitch;
    }


    /// <summary>
    /// Fades out the current music track and fades into the next one sequentially
    /// </summary>
    /// <param name="newTrack"></param>
    /// <param name="fadeInTime"></param>
    /// <param name="fadeOutTime"></param>
    /// <returns></returns>
    private IEnumerator IFadeChangeTrack(AudioClip newTrack, float fadeInTime, float fadeOutTime)
    {
        // Fade out
        yield return StartCoroutine(IFadeToVolume(musicSource, fadeOutTime, 0.0f));
        ChangeTrack(newTrack, true);
        // Fade back in
        yield return StartCoroutine(IFadeToVolume(musicSource, fadeInTime, 1.0f));

        activeMusicCoroutine = null;
    }

    /// <summary>
    /// Creates a temporary music track to cross fade between the current music track and the new music track
    /// </summary>
    /// <param name="newTrack"></param>
    /// <param name="fadeInTime"></param>
    /// <param name="fadeOutTime"></param>
    /// <returns></returns>
    private IEnumerator ICrossFadeTrack(AudioClip newTrack, float fadeInTime, float fadeOutTime)
    {
        GameObject go = new GameObject("Temp_MusicSource");
        AudioSource temp = go.AddComponent<AudioSource>();

        // Fade out the music source
        StartCoroutine(IFadeToVolume(musicSource, fadeOutTime, 0.0f));

        // Play the temp
        temp.volume = 0.0f;
        temp.clip = newTrack;
        temp.Play();

        // Fade in the temp track
        StartCoroutine(IFadeToVolume(temp, fadeInTime, 1.0f));

        yield return new WaitForSeconds(fadeInTime + fadeOutTime);

        // After the wait, setup the music source to have the same parameters as the temp]
        musicSource.volume = 1.0f;
        musicSource.clip = newTrack;
        musicSource.time = temp.time;

        // Destroy the temporary music source and play music source
        Destroy(go);
        musicSource.Play();

        activeMusicCoroutine = null;
    }

    /// <summary>
    /// Fades audio track to specified volume
    /// </summary>
    /// <param name="source"></param>
    /// <param name="time"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    private IEnumerator IFadeToVolume(AudioSource source, float time, float volume, bool setMusicCoroutineNull = false)
    {
        float elapsedTime = 0.0f;
        float initialVolume = source.volume;

        // Fades to the target volume
        while (elapsedTime < time)
        {
            source.volume = Mathf.Lerp(initialVolume, volume, elapsedTime / time);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        source.volume = volume;

        // This is here to specify whether this coroutine was called within the queue as an independent action or called from another coroutine.
        if (setMusicCoroutineNull) activeMusicCoroutine = null;
    }

    /// <summary>
    /// Processes audio data and plays according to the specified AudioData settings
    /// </summary>
    /// <param name="data"></param>
    public void ProcessAudioData(AudioData data)
    {
        // creates a new audio instance

        //GameObject gameObject = ObjectPoolManager.SpawnObject(soundPrefab.gameObject, data.spawnPosition, Quaternion.identity, ObjectPoolManager.PoolType.AudioSource);
        GameObject go = ObjectPooler.SpawnObject(soundPrefab.gameObject, data.spawnPosition, Quaternion.identity, ObjectPooler.PoolType.AudioSource);
        AudioSource source = go.GetComponent<AudioSource>();

        // Set volume
        source.volume = data.volume;

        // Randomize pitch if data wants to
        source.pitch = data.randomizePitch ? Random.Range(data.minPitch, data.maxPitch) : data.minPitch;
        source.clip = data.clip;

        // Play
        source.Play();
    }
}

[System.Serializable]
public struct AudioData
{
    public AudioClip clip;

    [Header("Properties")]
    [Range(0, 1)] public float volume;

    [Header("Pitch")]
    [Tooltip("If set to false, the minPitch will be the default pitch")]
    public bool randomizePitch;

    [Range(-3, 3)] public float minPitch;

    [ShowIf(nameof(randomizePitch))]
    [AllowNesting]
    [Range(-3, 3)] public float maxPitch;

    [HideInInspector]
    public Vector3 spawnPosition;
}