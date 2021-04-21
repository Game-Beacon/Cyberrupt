using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy, ITarget, IStateMachine, ISpawnDanmaku
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
    private float minRotation;
    [SerializeField]
    private float maxRotation;

    private float rotation;

    [SerializeField]
    private Enemy shield;
    private Player player;

    protected override void EnemyAwake()
    {
        _stateMachine = GetComponent<AIStateMachine>();
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();

        _stateMachine.OnUpdateTransform.AddListener(UpdateTransform);

        rotation = Random.Range(minRotation, maxRotation);
        rotation = (Random.Range(0f, 1f) > 0.5f) ? rotation : -rotation;
    }

    protected override void EnemyStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
    }

    private void UpdateTransform()
    {
        parent.rotation = Quaternion.Euler(0, 0, parent.rotation.eulerAngles.z + rotation * Time.fixedDeltaTime);
    }

    public override void OnKilled()
    {
        if(shield != null)
            shield.KillBehaviour(true);
    }
}
