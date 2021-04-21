using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class Turret_Shoot : AITimerState
{
    [Space(20)]
    [SerializeField]
    private Transform parent;
    private Transform target = null;
    [SerializeField, Range(0,0.5f)]
    private float lockOnStrenth;
    [SerializeField]
    private float lockOnTime;

    [SerializeField]
    private UltEvent OnLockOnTarget = new UltEvent();

    protected override void OnStateEnter()
    {
        base.OnStateEnter();
        if (target == null)
            target = GetComponent<ITarget>().target;

        float angle = parent.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 facing = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 direction = (target.position - parent.position).normalized;

        if (Vector2.Dot(facing, direction) >= 0.975f)
            StartCoroutine(LockTarget(lockOnTime));
        else
            SelfEndState();
    }

    IEnumerator LockTarget(float lockOn)
    {
        float timer = 0;

        while(timer < lockOn)
        {
            timer += Time.fixedDeltaTime;
            Vector2 direction = target.position - parent.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion lookAt = Quaternion.Euler(0, 0, angle);
            parent.rotation = Quaternion.Slerp(parent.rotation, lookAt, lockOnStrenth);
            yield return new WaitForFixedUpdate();
        }
        OnLockOnTarget.Invoke();
    }
}
