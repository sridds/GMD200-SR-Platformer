using UnityEngine;
using System;

public abstract class BaseState<EState> where EState : Enum
{
    // constructor to initialize key
    public BaseState(EState key) => StateKey = key;

    // accessor for the key
    public EState StateKey { get; private set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public abstract void OnTriggerEnter(Collider2D collision);
    public abstract void OnTriggerStay(Collider2D collision);
    public abstract void OnTriggerExit(Collider2D collision);
}
