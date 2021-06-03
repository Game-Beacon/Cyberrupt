using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;


public class TestBossState_DelayShot : AIState
{
    [Space(20)]
    private Transform target = null;
    [SerializeField]
    private Transform shooter;

    [SerializeField]
    private int totalShootTimes = 4;
    [SerializeField]
    private float shootGap = 2; 
    private float timer = 0;

    private int shootTimes = 0;

    [SerializeField]
    private UltEvent OnSpawn = new UltEvent();
    [SerializeField]
    private float shootDelay = 1;
    [SerializeField]
    private UltEvent ShootAfterSpawn = new UltEvent();
    [SerializeField]
    private float endDelay = 1;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        ITarget enemyTarget;
        if (target == null && TryGetComponent(out enemyTarget))
            target = enemyTarget.target;
    }

    protected override void OnStateUpdate(float delta)
    {
        if(shootTimes < totalShootTimes && timer >= shootTimes * shootGap)
        {
            if (target != null)
            {
                Vector2 direction = target.position - shooter.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                shooter.rotation = Quaternion.Euler(0, 0, angle);
            }

            OnSpawn.Invoke();
            shootTimes += 1;
            IEnumerator enumerator = DelayShoot(shootDelay);
            //asyncs.Add(enumerator);
            StartCoroutine(enumerator);
        }

        timer += delta;
    }

    protected override void OnStateExit()
    {
        timer = 0;
        shootTimes = 0;
    }

    IEnumerator DelayShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShootAfterSpawn.Invoke();
        if (shootTimes == totalShootTimes)
        {
            IEnumerator enumerator = EndDelay(endDelay);
            //asyncs.Add(enumerator);
            StartCoroutine(enumerator);
        }  
    }

    IEnumerator EndDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SelfEndState();
    }
}
