using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class TestBossState_CannonCrown : AIState
{
    [Space(20)]

    [SerializeField]
    private float speed;
    private Vector2 velocity;

    [SerializeField]
    private UltEvent OnSuccessCollide = new UltEvent();
    [SerializeField]
    private int totalShootTimes = 5;
    private int shootTimes;
    
    [SerializeField]
    private float shootCoolDown;
    private float coolDownTimer;

    protected override void OnStateEnter()
    {
        coolDownTimer = 0;
        shootTimes = 0;
        int rand = Random.Range(0, 4);
        float angle = (45 + 90 * rand) * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
    }

    protected override void UpdateTransform(float delta)
    {
        transform.position += (Vector3)velocity * delta;
    }

    protected override void OnStateUpdate(float delta)
    {
        coolDownTimer -= delta;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(stateMachine != null && stateMachine.currentState == this && coolDownTimer <= 0)
        {
            coolDownTimer = shootCoolDown;
            OnSuccessCollide.Invoke();
            shootTimes++;
            if (shootTimes >= totalShootTimes)
                SelfEndState();
        }
        
        velocity = Vector2.Reflect(velocity, collision.contacts[0].normal);
    }
}
