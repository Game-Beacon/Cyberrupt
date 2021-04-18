using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITimerState : AIState
{
    [SerializeField]
    private float time;
    private float timer;

    /// <summary>
    /// 在複寫時請保留base
    /// </summary>
    protected override void OnStateEnter()
    {
        timer = 0;
    }

    /// <summary>
    /// 在複寫時請保留base
    /// </summary>
    protected override void OnStateUpdate(float delta)
    {
        timer += delta;
        if (timer >= time)
            SelfEndState();
    }
}
