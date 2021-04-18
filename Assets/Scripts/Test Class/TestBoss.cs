using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : Enemy, ITarget, IStateMachine, ISpawnDanmaku
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
    private float speed;

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
        Vector2 direction = _target.position - transform.position;
        transform.position += (Vector3)direction.normalized * speed * Time.fixedDeltaTime;
    }

    /*protected override void MachineAwake()
    {
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();
        _EnterNewState.AddListener(_danmakuHelper.StopAll);
        _ExitState.AddListener(_danmakuHelper.StopAll);
    }

    protected override void MachineStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
    }

    protected override void UpdateTransform()
    {
        Vector2 direction = _target.position - transform.position;
        transform.position += (Vector3)direction.normalized * speed * Time.fixedDeltaTime;
    }

    protected override void MachineFixedUpdate()
    {
        foreach(GameObject obj in lookAts)
        {
            Vector2 dir = _target.position - obj.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public override void GameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            InterruptState();
    }*/
}
