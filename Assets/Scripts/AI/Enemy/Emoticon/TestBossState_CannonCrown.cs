using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class TestBossState_CannonCrown : AIState
{
    /*[Space(20)]

    [SerializeField]
    private LayerMask hitLayer;*/
    [SerializeField]
    private ParticleSystem moveParticle;

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

        moveParticle.Play();
    }

    protected override void UpdateTransform(float delta)
    {
        //transform.position += (Vector3)velocity * delta;
        LayerMask mask = CollisionLayer.instance.screenMask;
        RaycastHit2D raycast = Physics2D.CircleCast(transform.position, 1, velocity, velocity.magnitude * delta, mask);
        if(raycast.collider != null)
        {
            velocity = Vector2.Reflect(velocity, raycast.normal);
            transform.position = raycast.point + raycast.normal.normalized;
            if (coolDownTimer <= 0 && shootTimes < totalShootTimes)
            {
                coolDownTimer = shootCoolDown;
                OnSuccessCollide.Invoke();
                shootTimes++;
                if (shootTimes >= totalShootTimes)
                {
                    StartCoroutine(EndDelay());
                    velocity = Vector2.zero;
                }
            }
        }
        else
            transform.position += (Vector3)velocity * delta;
    }

    protected override void OnStateUpdate(float delta)
    {
        coolDownTimer -= delta;
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if(stateMachine != null && stateMachine.currentState == this && ((1 << collision.gameObject.layer) & hitLayer) != 0 && coolDownTimer <= 0 && shootTimes < totalShootTimes)
        {
            coolDownTimer = shootCoolDown;
            OnSuccessCollide.Invoke();
            shootTimes++;
            if (shootTimes >= totalShootTimes)
            {
                StartCoroutine(EndDelay());
                velocity = Vector2.zero;
            }
           
        }

        if (((1 << collision.gameObject.layer) & hitLayer) != 0)
            velocity = Vector2.Reflect(velocity, collision.contacts[0].normal);
        else
            velocity = velocity.normalized * speed;
    }*/

    IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(0.1f);
        moveParticle.Stop();
        SelfEndState();
    }
}
