using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossState_Emojis : AITimerState
{
    private Transform target = null;
    [SerializeField]
    private Transform shooter;
    [SerializeField, Range(0, 0.5f)]
    private float follow;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        ITarget enemyTarget;
        if (target == null && TryGetComponent(out enemyTarget))
            target = enemyTarget.target;

        if(target != null)
        {
            Vector2 direction = target.position - shooter.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shooter.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void OnStateUpdate(float delta)
    {
        base.OnStateUpdate(delta);
        
        if(target != null)
        {
            Vector2 direction = target.position - shooter.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shooter.rotation = Quaternion.Slerp(shooter.rotation, Quaternion.Euler(0, 0, angle), follow);
        }
    }
}
