using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class TimeManager
{
    private static bool _paused = false;
    public static bool paused { get { return _paused; } }

    public static UnityEvent OnGamePause { get; } = new UnityEvent();
    public static UnityEvent OnGameUnpause { get; } = new UnityEvent();

    public static void PauseGame()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            _paused = true;
            OnGamePause.Invoke();
        }
    }

    public static void UnpauseGame()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
            _paused = false;
            OnGameUnpause.Invoke();
        }
    }
}
