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
    private Transform root;
    [SerializeField]
    private LineRenderer lr;
    [SerializeField]
    private float pathLength;
    [SerializeField]
    private float pathSpeed;

    [SerializeField, Range(0, 0.5f)]
    private float lockStrenth;

    [SerializeField]
    private List<Enemy> shields = new List<Enemy>();
    private Player player;

    private Vector2 startPosition;
    private Vector2 currentPosition;
    private Vector2 endPosition;
    private Vector2 pathDirection;

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
        CreatePath();
    }

    private void CreatePath()
    {
        startPosition = transform.position;
        currentPosition = startPosition;
        Vector2 pinpoint = manager.GetRandomPointInScreen(2.5f);
        endPosition = startPosition + (pinpoint - startPosition).normalized * pathLength;
        pathDirection = (endPosition - startPosition).normalized;

        lr.positionCount = 2;
        lr.SetPosition(0, currentPosition);
        lr.SetPosition(1, endPosition);
    }

    private void UpdateTransform()
    {
        currentPosition += pathDirection * Mathf.Min((endPosition - currentPosition).magnitude, pathSpeed * Time.fixedDeltaTime);
        root.position = currentPosition;

        lr.SetPosition(0, currentPosition);
        lr.SetPosition(1, endPosition);

        if ((currentPosition - endPosition).sqrMagnitude < 0.5f)
            Die();

        Vector2 direction = target.position - parent.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion lookAt = Quaternion.Euler(0, 0, angle);
        parent.rotation = Quaternion.Slerp(parent.rotation, lookAt, lockStrenth);
    }

    public override void OnKilled()
    {
        foreach(Enemy shield in shields)
            if(shield != null)
                shield.KillBehaviour(true);
        Destroy(parent.gameObject, 0.03f);
        Destroy(root.gameObject, 0.03f);
    }
}
