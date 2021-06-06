using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petya : Enemy, ITarget, IStateMachine, ISpawnDanmaku
{
    private Transform _target;
    public Transform target { get { return _target; } }

    private AIStateMachine _stateMachine;
    public AIStateMachine stateMachine { get { return _stateMachine; } }

    private SpawnDanmakuHelper _danmakuHelper;
    public SpawnDanmakuHelper danmakuHelper { get { return _danmakuHelper; } }

    //====================

    private Player player;

    [SerializeField]
    private GameObject up;
    [SerializeField]
    private GameObject left;
    [SerializeField]
    private GameObject right;
    [SerializeField]
    private GameObject center;


    [SerializeField]
    private float speed;
    [SerializeField]
    private float slowDownRadius;
    [SerializeField]
    private float stopRadius;

    [SerializeField, Range(0, 1)]
    private float lookAtStrenth;

    protected override void EnemyAwake()
    {
        center.transform.SetParent(null);

        _stateMachine = GetComponent<AIStateMachine>();
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();

        _stateMachine.OnUpdateTransform.AddListener(UpdateTransform);
    }

    protected override void EnemyStart()
    {
        up.transform.SetParent(null);
        up.transform.position = new Vector2(0, (screen == null)? 6 : screen.GetWorldScreenMaxY + 0.5f);
        up.transform.rotation = Quaternion.Euler(0, 0, -90);

        left.transform.SetParent(null);
        left.transform.position = new Vector2((screen == null) ? -10 : screen.GetWorldScreenMinX - 0.5f, 0);

        right.transform.SetParent(null);
        right.transform.position = new Vector2((screen == null) ? 10 : screen.GetWorldScreenMaxX + 0.5f, 0);

        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
    }

    protected override void EnemyUpdate()
    {
        center.transform.position = transform.position;
    }

    private void UpdateTransform()
    {
        Vector2 direction = _target.position - transform.position;

        //Deal with position.
        float lerp = Mathf.Clamp01((direction.magnitude - stopRadius) / (slowDownRadius - stopRadius));

        transform.position += (Vector3)direction.normalized * speed * lerp * Time.fixedDeltaTime;

        //Deal with rotation.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, lookAtStrenth);
    }

    public override void OnKilled()
    {
        Destroy(up);
        Destroy(left);
        Destroy(right);
        Destroy(center);
    }
}
