using System;

public class Timer
{
    public float RemainingTime { get; private set; }

    // Constructor to set the remaining time
    public Timer(float duration) => RemainingTime = duration;

    public event Action OnTimerEnd;

    /// <summary>
    /// Called externally to tick the timer down
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Tick(float deltaTime)
    {
        // Stop ticking if the timer ended
        if (RemainingTime == 0f) return;

        RemainingTime -= deltaTime;
        CheckIfTimerEnded();
    }

    private void CheckIfTimerEnded()
    {
        if (RemainingTime > 0f) return;

        // Set remaining time to 0
        RemainingTime = 0f;
        // Invoke only if the timer has ended
        OnTimerEnd?.Invoke();
    }
}
