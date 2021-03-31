using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float hitRadius;
    private DanmakuManager danmakuManager;

    public override void GameStart()
    {
        danmakuManager = DanmakuManager.instance;
        danmakuManager.SetDanmakuTarget(transform, hitRadius);
        danmakuManager.bulletHitTarget.AddListener(OnHit);
    }

    private void OnHit()
    {
        Debug.Log("Ouch");
    }

    public override void GameFixedUpdate()
    {
        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            dir += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            dir += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            dir += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            dir += Vector2.right;

        if (dir.magnitude > 0)
            transform.position += (Vector3)dir.normalized * speed * Time.fixedDeltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
