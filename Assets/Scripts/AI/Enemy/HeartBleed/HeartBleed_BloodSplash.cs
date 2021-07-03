using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class HeartBleed_BloodSplash : AIState
{
    [Space(20), SerializeField]
    private int count;
    [SerializeField]
    private float delay;
    [SerializeField]
    public UltEvent OnAttack = new UltEvent();

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        for(int i = 0; i < count; i ++)
        {
            OnAttack.Invoke();
            if(i != count - 1)
                yield return new WaitForSeconds(delay);
        }
        SelfEndState();
    }
}
