using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// IHeartGameDev made a wonderful video explaining how to implement abstract state machines in Unity using generics:
/// https://www.youtube.com/watch?v=qsIiFsddGV4&list=TLPQMTMwMjIwMjSVSHk_cY6Wiw&index=1&ab_channel=iHeartGameDev
/// </summary>
/// <typeparam name="EState"></typeparam>
public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState> CurrentState;

    private bool isTransitioningState = false;

    private void Start()
    {
        CurrentState.EnterState();
    }

    private void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        CurrentState.UpdateState();
        if (nextStateKey.Equals(CurrentState.StateKey)) {
            CurrentState.UpdateState();
        }
        else if(!isTransitioningState) {
            TransitionToState(nextStateKey);
        }
    }

    public void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();

        isTransitioningState = false;
    }


    private void OnTriggerEnter2D(Collider2D collision) => CurrentState.OnTriggerEnter(collision);
   
    private void OnTriggerStay2D(Collider2D collision) => CurrentState.OnTriggerStay(collision);

    private void OnTriggerExit2D(Collider2D collision) => CurrentState.OnTriggerExit(collision);
}
