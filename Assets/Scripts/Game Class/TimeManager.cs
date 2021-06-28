using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class TimeManager
{
    public static bool paused = false;

    public static UnityEvent OnGamePause { get; } = new UnityEvent();
    public static UnityEvent OnGameUnpause { get; } = new UnityEvent();

    public static void PauseGame()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            paused = true;
            OnGamePause.Invoke();
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
            OnGameUnpause.Invoke();
        }
    }
}
