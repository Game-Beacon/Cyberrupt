using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meltdown : Enemy, ITarget, IStateMachine, ISpawnDanmaku
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
    [SerializeField]
    private float slowDownRadius;
    [SerializeField]
    private float stopRadius;

    private Vector2 velocityDirection;    
    [SerializeField, Range(0f, 1f)]
    private float lookAtStrenth;
    [SerializeField, Range(0f, 1f)]
    private float steerStrenth;

    protected override void EnemyAwake()
    {
        _stateMachine = GetComponent<AIStateMachine>();
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();

        _stateMachine.OnUpdateTransform.AddListener(UpdateTransform);
        _stateMachine.SetPickStateTimeRand(0.2f);
    }

    protected override void EnemyStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;

        velocityDirection = (_target.position - transform.position).normalized;
    }

    private void UpdateTransform()
    {
        Vector2 direction = _target.position - transform.position;

        //Deal with rotation.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetQuaternion = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, lookAtStrenth);

        //Deal with direction.
        float directionAngle = Mathf.Atan2(velocityDirection.y, velocityDirection.x) * Mathf.Rad2Deg;
        Quaternion directionQuaternion = Quaternion.Euler(0, 0, directionAngle);

        float lerp = Mathf.Clamp((direction.magnitude - stopRadius) / (slowDownRadius - stopRadius), 0, 1.5f);
        lerp = (lerp > 1) ? lerp : EaseLibrary.CallEaseFunction(Easing.OutCubic, lerp);

        float steerStr = (lerp > 1) ? steerStrenth * lerp : steerStrenth;

        Quaternion result = Quaternion.Slerp(directionQuaternion, targetQuaternion, steerStr);
        float resultAngle = result.eulerAngles.z * Mathf.Deg2Rad;
        velocityDirection = new Vector2(Mathf.Cos(resultAngle), Mathf.Sin(resultAngle));

        transform.position += (Vector3)velocityDirection * speed * lerp * Time.fixedDeltaTime;
    }

    public void ReadjustDirection()
    {
        velocityDirection = (_target.position - transform.position).normalized;
    }
}
