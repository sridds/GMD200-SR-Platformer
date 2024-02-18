using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraTransition : MonoBehaviour
{
    public enum TransitionEntry { Enter, Exit }
    public enum TransitionState { PreDelay, Transition, PostDelay }
    public enum TransitionDirection { Horizontal, Vertical }
    public enum TransitionEventCall
    {
        BothBeforeDelay,
        BothAfterDelay,
        OnEnterBeforeDelay,
        OnEnterAfterDelay,
        OnExitBeforeDelay,
        OnExitAfterDelay
    }

    [SerializeField] private bool onlyMoveCamera;
    [SerializeField] private TransitionEntry entry = TransitionEntry.Enter;
    [SerializeField] private TransitionState state = TransitionState.PreDelay;
    [SerializeField] private TransitionDirection direction = TransitionDirection.Horizontal;
    [SerializeField] private TransitionEventCall eventCallPreDelay = TransitionEventCall.BothBeforeDelay;
    [SerializeField] private TransitionEventCall eventCallPostDelay = TransitionEventCall.BothBeforeDelay;

    [Header("Timers")]
    [SerializeField] private float transitionDelay = 1.0f;
    [SerializeField] private float preTransitionDelay = 1.0f;
    [SerializeField] private float postTransitionDelay = 1.0f;

    [Header("Positions")]
    public Vector2 cameraMinPosition;
    public Vector2 cameraMaxPosition;
    [SerializeField] private Vector2 playerChange;

    [Header("Events")]
    public UnityEvent preTransitionEvent;
    public UnityEvent postTransitionEvent;

    private CameraMovement cam;
    private GameObject player;

    private Vector2 cameraMinPrevious;
    private Vector2 cameraMaxPrevious;
    private Vector2 cameraMoveStart;
    private Vector2 cameraMoveFinish;
    private Vector2 cameraMoveProgress;
    private Vector2 playerMoveStart;
    private Vector2 playerMoveFinish;

    private float progress; // lerp calculation
    private float transitionTimer;

    private bool transition;
    private bool getCamPrevious = true;
    private bool callPreTransitionEvent = true;
    private bool callPostTransitionEvent = true;

    private void Start()
    {
        cam = FindObjectOfType<CameraMovement>();
    }

    private void Update()
    {
        if (transition)
        {
            switch (state)
            {
                case TransitionState.PreDelay:
                    PreDelay();
                    break;
                case TransitionState.Transition:
                    Transition();
                    break;
                case TransitionState.PostDelay:
                    PostDelay();
                    break;
            }
        }
    }

    private void PreDelay()
    {
        CallEventBeforePreDelay();

        transitionTimer -= Time.deltaTime;
        if (transitionTimer <= 0)
        {
            CallEventAfterPreDelay();

            state = TransitionState.Transition;
            transitionTimer = 0;
        }
    }

    private void Transition()
    {
        progress = Mathf.Clamp(transitionTimer, 0, transitionDelay) / transitionDelay;
        transitionTimer += Time.deltaTime;
        cameraMoveProgress = Vector2.Lerp(cameraMoveStart, cameraMoveFinish, progress);
        cam.transform.position = new Vector3(cameraMoveProgress.x, cameraMoveProgress.y, cam.transform.position.z);
        player.transform.position = Vector2.Lerp(playerMoveStart, playerMoveFinish, progress);
        if (progress >= 1)
        {
            cam.boundsMin = (entry == TransitionEntry.Enter) ? cameraMinPosition : cameraMinPrevious;
            cam.boundsMax = (entry == TransitionEntry.Enter) ? cameraMaxPosition : cameraMaxPrevious;

            player.transform.position = playerMoveFinish;
            cam.player = player.transform;
            state = TransitionState.PostDelay;
            transitionTimer = postTransitionDelay;
        }
    }

    private void PostDelay()
    {
        CallEventBeforePostDelay();

        transitionTimer -= Time.deltaTime;
        if (transitionTimer <= 0)
        {
            CallEventAfterPostDelay();

            entry = (entry == TransitionEntry.Enter) ? TransitionEntry.Exit : TransitionEntry.Enter;
            transition = false;

            player.GetComponent<PlayerMovement>().FreezeInput(false);
            player.GetComponent<PlayerMovement>().FreezePlayer(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (onlyMoveCamera)
            {
                cam.boundsMin = cameraMinPosition;
                cam.boundsMax = cameraMaxPosition;
                return;
            }

            if (!transition)
            {
                // Init
                transition = true;
                player = other.gameObject;

                cam.player = null;

                state = TransitionState.PreDelay;
                transitionTimer = preTransitionDelay;

                callPreTransitionEvent = true;
                callPostTransitionEvent = true;

                cameraMoveStart = cam.transform.position;
                playerMoveStart = player.transform.position;

                if (entry == TransitionEntry.Enter)
                {
                    if (getCamPrevious)
                    {
                        getCamPrevious = false;
                        cameraMinPrevious = cam.boundsMin;
                        cameraMaxPrevious = cam.boundsMax;
                    }
                    playerMoveFinish = playerMoveStart + playerChange;
                }
                else
                {
                    playerMoveFinish = playerMoveStart - playerChange;
                }

                // compare direction
                if (direction == TransitionDirection.Horizontal)
                {
                    float cameraMinPosX = cameraMinPosition.x;
                    if (entry == TransitionEntry.Exit)
                    {
                        cameraMinPosX = (playerChange.x > 0) ? cameraMaxPrevious.x : cameraMinPrevious.x;
                    }
                    cameraMoveFinish = new Vector2(cameraMinPosX, cam.transform.position.y);
                }
                else
                {
                    float cameraMinPosY = cameraMinPosition.y;
                    if (entry == TransitionEntry.Exit)
                    {
                        cameraMinPosY = (playerChange.y > 0) ? cameraMaxPrevious.y : cameraMinPrevious.y;
                    }
                    cameraMoveFinish = new Vector2(cam.transform.position.x, cameraMinPosY);
                }

                // pause player HERE
                player.GetComponent<PlayerMovement>().FreezeInput(true);
                player.GetComponent<PlayerMovement>().FreezePlayer(true);
            }
        }
    }

    #region - Events - 

    private void CallPreTransitionEvent()
    {
        if (callPreTransitionEvent)
        {
            callPreTransitionEvent = false;
            preTransitionEvent.Invoke();
        }
    }

    private void CallEventBeforePreDelay()
    {
        switch (eventCallPreDelay)
        {
            case TransitionEventCall.BothBeforeDelay:
                CallPreTransitionEvent();
                break;
            case TransitionEventCall.OnEnterBeforeDelay:
                if (entry == TransitionEntry.Enter)
                {
                    CallPreTransitionEvent();
                }
                break;
            case TransitionEventCall.OnExitBeforeDelay:
                if (entry == TransitionEntry.Exit)
                {
                    CallPreTransitionEvent();
                }
                break;
        }
    }

    private void CallEventAfterPreDelay()
    {
        switch (eventCallPreDelay)
        {
            case TransitionEventCall.BothAfterDelay:
                CallPreTransitionEvent();
                break;
            case TransitionEventCall.OnEnterAfterDelay:
                if (entry == TransitionEntry.Enter)
                {
                    CallPreTransitionEvent();
                }
                break;
            case TransitionEventCall.OnExitAfterDelay:
                if (entry == TransitionEntry.Exit)
                {
                    CallPreTransitionEvent();
                }
                break;
        }
    }

    private void CallPostTransitionEvent()
    {
        if (callPostTransitionEvent)
        {
            callPostTransitionEvent = false;
            postTransitionEvent.Invoke();
        }
    }

    private void CallEventBeforePostDelay()
    {
        switch (eventCallPostDelay)
        {
            case TransitionEventCall.BothBeforeDelay:
                CallPostTransitionEvent();
                break;
            case TransitionEventCall.OnEnterBeforeDelay:
                if (entry == TransitionEntry.Enter)
                {
                    CallPostTransitionEvent();
                }
                break;
            case TransitionEventCall.OnExitBeforeDelay:
                if (entry == TransitionEntry.Exit)
                {
                    CallPostTransitionEvent();
                }
                break;
        }
    }

    private void CallEventAfterPostDelay()
    {
        switch (eventCallPostDelay)
        {
            case TransitionEventCall.BothAfterDelay:
                CallPostTransitionEvent();
                break;
            case TransitionEventCall.OnEnterAfterDelay:
                if (entry == TransitionEntry.Enter)
                {
                    CallPostTransitionEvent();
                }
                break;
            case TransitionEventCall.OnExitAfterDelay:
                if (entry == TransitionEntry.Exit)
                {
                    CallPostTransitionEvent();
                }
                break;
        }
    }
    #endregion
}
