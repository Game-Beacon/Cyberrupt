using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class Bot_Shoot : AITimerState
{
    [SerializeField]
    protected UltEvent Level1 = new UltEvent();
    [SerializeField]
    protected UltEvent Level2 = new UltEvent();
    [SerializeField]
    protected UltEvent Level3 = new UltEvent();

    private Bot bot = null;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        if (bot == null)
            TryGetComponent(out bot);
        if (bot != null)
        {
            if (bot.level <= 1)
                Level1.Invoke();
            else if (bot.level == 2)
                Level2.Invoke();
            else
                Level3.Invoke();
        }
        else
            Level1.Invoke();
    }
}
