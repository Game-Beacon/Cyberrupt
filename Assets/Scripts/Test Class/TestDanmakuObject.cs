using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class TestDanmakuObject : GameBehaviour
{
    [SerializeField]
    private float speed;
    private Vector2 direction;
    [SerializeField]
    private float killTime;

    [SerializeField]
    private UltEvent OnSpawn = new UltEvent();
    [SerializeField]
    private UltEvent OnKill = new UltEvent(); 

    public override void GameStart()
    {
        OnSpawn.Invoke();
        StartCoroutine(KillSelf(killTime));
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public override void GameUpdate()
    {
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    public override void OnKilled()
    {
        OnKill.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CollisionLayer.CollideWithMask(collision.gameObject, CollisionLayer.instance.bombMask))
        {
            StartCoroutine(KillAfterFrame());
        }    
    }

    IEnumerator KillAfterFrame()
    {
        yield return null;
        KillBehaviour(true);
    }

    IEnumerator KillSelf(float time)
    {
        yield return new WaitForSeconds(killTime);
        KillBehaviour(true);
    }
}
