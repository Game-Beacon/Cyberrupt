using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meltdown_Strafe : AITimerState
{
    [SerializeField]
    private float lookAtStrenth;
    private Transform target = null;
    private Meltdown meltdown = null;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        if (target == null)
            target = GetComponent<ITarget>().target;
        if (meltdown == null)
            meltdown = GetComponent<Meltdown>();
    }


    protected override void UpdateTransform(float delta)
    {
        Vector2 direction = target.position - transform.position;

        //Deal with rotation.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, lookAtStrenth);
    }

    protected override void OnStateExit()
    {
        base.OnStateExit();
        meltdown.ReadjustDirection();
    }
}
