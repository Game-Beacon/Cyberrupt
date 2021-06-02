using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : Enemy, ITarget, IStateMachine, ISpawnDanmaku
{
    private Transform _target;
    public Transform target { get { return _target; } }

    private AIStateMachine _stateMachine;
    public AIStateMachine stateMachine { get { return _stateMachine; } }

    private SpawnDanmakuHelper _danmakuHelper;
    public SpawnDanmakuHelper danmakuHelper { get { return _danmakuHelper; } }

    //====================

    [SerializeField]
    private Transform parent;
    [SerializeField]
    private float speed;
    private Player player;

    protected override void EnemyAwake()
    {
        _stateMachine = GetComponent<AIStateMachine>();
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();

        _stateMachine.OnUpdateTransform.AddListener(UpdateTransform);
    }

    protected override void EnemyStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
    }

    private void UpdateTransform()
    {
        Vector2 direction = (target.position - parent.position).normalized;
        parent.position += (Vector3)direction * speed * Time.fixedDeltaTime;
    }

    public override void OnKilled()
    {
        if (parent != null)
            DestroySafe(parent.gameObject);
    }
}
