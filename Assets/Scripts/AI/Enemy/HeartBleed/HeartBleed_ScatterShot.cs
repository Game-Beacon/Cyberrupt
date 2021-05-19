using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBleed_ScatterShot : AITimerState
{
    [SerializeField]
    private Transform muzzle = null;
    private Transform target = null;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        if (target == null)
            target = GetComponent<ITarget>().target;
        Vector2 direction = target.position - muzzle.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        muzzle.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void OnStateUpdate(float delta)
    {
        base.OnStateUpdate(delta);
        Vector2 direction = target.position - muzzle.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        muzzle.rotation = Quaternion.Euler(0, 0, angle);
    }
}
