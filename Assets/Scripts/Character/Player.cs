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
    private Camera cam;

    public override void GameAwake()
    {
        DependencyContainer.AddDependency(this);
    }

    public override void GameStart()
    {
        danmakuManager = DanmakuManager.instance;
        danmakuManager.SetDanmakuTarget(transform, hitRadius);
        danmakuManager.bulletHitTarget.AddListener(OnHit);
        cam = Camera.main;
    }

    private void OnHit()
    {
        Debug.Log("Ouch");
    }

    public override void GameUpdate()
    {
        Vector2 dir = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
