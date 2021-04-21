using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossState_Pistol : AIState
{
    [Space(20)]
    private Transform target = null;
    [SerializeField]
    private GameObject Bullet;
    [SerializeField]
    private Transform rotor;
    [SerializeField]
    private Transform shooter;

    [Space(20)]
    [SerializeField]
    private float shootSpread;
    [SerializeField]
    private int totalShootTimes = 4;
    [SerializeField]
    private float shootGap = 2;
    private float timer = 0;

    private int shootTimes = 0;

    private bool warmUp;
    [SerializeField]
    private float startDelay = 1;
    [SerializeField]
    private float endDelay = 1;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        timer = 0;
        shootTimes = 0;
        warmUp = false;

        ITarget enemyTarget;
        if (target == null && TryGetComponent(out enemyTarget))
            target = enemyTarget.target;

        IEnumerator enumerator = StartDelay(startDelay);
        asyncs.Add(enumerator);
        StartCoroutine(enumerator);
    }

    protected override void OnStateUpdate(float delta)
    {
        if (!warmUp)
            return;

        if (target != null && shootTimes < totalShootTimes)
        {
            Vector2 direction = target.position - rotor.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rotor.rotation = Quaternion.Euler(0, 0, angle - 13);
        }

        if (shootTimes < totalShootTimes && timer >= shootTimes * shootGap)
        {
            float spread = Random.Range(-shootSpread / 2f, shootSpread / 2f);
            Quaternion rotation = Quaternion.Euler(0, 0, shooter.rotation.eulerAngles.z + spread);
            Instantiate(Bullet, shooter.position, rotation);
            shootTimes += 1;
            if(shootTimes == totalShootTimes)
            {
                IEnumerator enumerator = EndDelay(endDelay);
                asyncs.Add(enumerator);
                StartCoroutine(enumerator);
            }
        }

        timer += delta;
    }

    protected override void OnStateExit()
    {
        timer = 0;
        shootTimes = 0;
        warmUp = false;
        rotor.rotation = Quaternion.identity;
    }

    IEnumerator StartDelay(float delay)
    {
        float timer = 0;
        while(timer < delay)
        {
            timer += Time.fixedDeltaTime;
            if(target != null)
            {
                Vector2 direction = target.position - rotor.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                rotor.rotation = Quaternion.Slerp(rotor.rotation, Quaternion.Euler(0, 0, angle - 13), 0.1f);
            }
            yield return new WaitForFixedUpdate();
        }
        warmUp = true;
    }

    IEnumerator EndDelay(float delay)
    {
        float timer = 0;

        Quaternion start = rotor.rotation;
        Quaternion end = Quaternion.identity;

        while (timer < delay)
        {
            timer += Time.fixedDeltaTime;
            rotor.rotation = Quaternion.Slerp(start, end, Mathf.Clamp01(timer / delay));
            yield return new WaitForFixedUpdate();
        }
        SelfEndState();
    }
}
