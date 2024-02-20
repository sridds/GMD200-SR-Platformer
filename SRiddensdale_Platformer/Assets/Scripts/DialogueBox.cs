using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField]
    private GameObject _holder;

    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private float _textSpeed;

    [SerializeField]
    private AudioData _typeSound;

    private Queue<string> dialogueQueue = new Queue<string>();
    private Coroutine activeDialogueCoroutine = null;

    public delegate void DialogueStart();
    public DialogueStart OnDialogueStart;

    public delegate void DialogueFinish();
    public DialogueFinish OnDialogueFinish;

    bool entered = false;

    private void Start()
    {
        _holder.SetActive(false);

        // subscribe to the static sign event. any sign can activate dialogue
        Sign.OnSignInteract += QueueDialogue;
    }

    /// <summary>
    /// Queue a single string of dialogue
    /// </summary>
    /// <param name="text"></param>
    public void QueueDialogue(string text) => dialogueQueue.Enqueue(text);

    /// <summary>
    /// Queue an entire array of dialogue lines
    /// </summary>
    /// <param name="text"></param>
    public void QueueDialogue(string[] text)
    {
        foreach(string s in text)
        {
            dialogueQueue.Enqueue(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueQueue.Count > 0 && !entered) Enter();

        if (!entered) return;
        if (CanContinue()) Continue();
        else if (CanExit()) Exit();
    }

    /// <summary>
    /// Returns true if the conditions are met to continue through dialouge
    /// </summary>
    /// <returns></returns>
    private bool CanContinue()
    {
        if (activeDialogueCoroutine == null && dialogueQueue.Count > 0 && Input.GetKeyDown(KeyCode.Z)) return true;
        return false;
    }

    /// <summary>
    /// Returns true if the conditions are met to exit the dialogue box
    /// </summary>
    /// <returns></returns>
    private bool CanExit()
    {
        if (activeDialogueCoroutine == null && dialogueQueue.Count == 0 && Input.GetKeyDown(KeyCode.Z) && entered) return true;
        return false;
    }

    /// <summary>
    /// Enter the dialogue box
    /// </summary>
    private void Enter()
    {
        _holder.SetActive(true);
        OnDialogueStart?.Invoke();

        // immediately get the first line
        Continue();

        entered = true;
    }

    private void Exit()
    {
        _holder.SetActive(false);
        OnDialogueFinish?.Invoke();

        entered = false;
    }

    private void Continue()
    {
        activeDialogueCoroutine = StartCoroutine(HandleDialogue(dialogueQueue.Dequeue()));
    }

    /// <summary>
    /// Handles a single string of dialogue
    /// </summary>
    /// <param name="dialogue"></param>
    /// <returns></returns>
    private IEnumerator HandleDialogue(string dialogue)
    {
        // clear text
        _text.text = "";

        // iterate through each char and add it to the text.
        for(int i = 0; i < dialogue.Length; i++)
        {
            _text.text += dialogue[i];

            yield return new WaitForSecondsRealtime(_textSpeed);
            AudioHandler.instance.ProcessAudioData(_typeSound);
        }

        activeDialogueCoroutine = null;
    }
}
