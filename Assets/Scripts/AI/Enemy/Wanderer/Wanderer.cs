using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : Enemy, ITarget, ISpawnDanmaku
{
    private Transform _target;
    public Transform target { get { return _target; } }

    private SpawnDanmakuHelper _danmakuHelper;
    public SpawnDanmakuHelper danmakuHelper { get { return _danmakuHelper; } }

    //====================

    private Player player;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float minRotationSpeed;
    [SerializeField]
    private float maxRotationSpeed;

    private float rotationSpeed;
    private Vector2 direction;

    protected override void EnemyAwake()
    {
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();
    }

    protected override void EnemyStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;

        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * ((Random.Range(0f, 1f) > 0.5f)? 1 : -1);

        float directionAngle = Random.Range(20f, 70f);
        int quadrant = Random.Range(0, 4);
        directionAngle = (directionAngle + 90 * quadrant) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(directionAngle), Mathf.Sin(directionAngle));
    }

    protected override void EnemyUpdate()
    {
        Vector2 outBound = screen.SnapInScreen(transform.position);
        if (outBound.x != 0)
            direction = new Vector2((outBound.x > 0)? Mathf.Abs(direction.x) : -Mathf.Abs(direction.x), direction.y);
        if (outBound.y != 0)
            direction = new Vector2(direction.x, (outBound.y > 0) ? Mathf.Abs(direction.y) : -Mathf.Abs(direction.y));

        //Handling transform.
        transform.position += (Vector3)direction * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotationSpeed * Time.deltaTime);
    }
}
