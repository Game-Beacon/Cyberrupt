using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Enemy, ITarget, IStateMachine, ISpawnDanmaku
{
    private Transform _target;
    public Transform target { get { return _target; } }

    private AIStateMachine _stateMachine;
    public AIStateMachine stateMachine { get { return _stateMachine; } }

    private SpawnDanmakuHelper _danmakuHelper;
    public SpawnDanmakuHelper danmakuHelper { get { return _danmakuHelper; } }

    //====================

    private static List<Bot> bots = new List<Bot>();

    private Player player;

    [SerializeField]
    private float speed;
    [SerializeField]
    private float radius;
    private float radiusSquare;
    [SerializeField, Range(0, 5)]
    private float keepDistance;
    [SerializeField, Range(0, 1)]
    private float targetStrenth;
    [SerializeField, Range(0, 1)]
    private float avoidStrenth;
    [SerializeField, Range(0, 1)]
    private float cohesionStrenth;
    [SerializeField, Range(0, 0.5f)]
    private float steerStrenth;

    private Vector2 direction;

    private int _level = 1;
    public int level { get { return _level; } private set { } }

    [SerializeField]
    private Gradient gradient;
    private SpriteRenderer sr;
    private float levelSmoothed = 0;
    [SerializeField, Range(0,0.5f)]
    private float levelSmoothLerp;

    protected override void EnemyAwake()
    {
        _stateMachine = GetComponent<AIStateMachine>();
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();

        _stateMachine.OnUpdateTransform.AddListener(UpdateTransform);
        _stateMachine.SetPickStateTimeRand(0.25f);
        
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        radiusSquare = radius * radius;

        sr = GetComponentInChildren<SpriteRenderer>();
        bots.Add(this);
    }

    protected override void EnemyStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
    }

    private void UpdateTransform()
    {
        //Make the "Boids" like movement
        Vector2 targetVector = (_target.position - transform.position).normalized;
        Vector2 avoidVector = Vector2.zero;
        Vector2 cohesionVector = Vector2.zero;

        Vector2 massCenter = transform.position;
        int mass = 1;

        int surroundings = 0;

        float distance = (_target.position - transform.position).magnitude;

        foreach (Bot bot in bots)
        {
            if (bot == this)
                continue;

            Vector2 vect = transform.position - bot.transform.position;
            float sqrMagnitude = vect.sqrMagnitude;

            if (sqrMagnitude > radiusSquare)
                continue;
            else
            {
                mass++;
                surroundings++;
                massCenter += (Vector2)bot.transform.position;
                avoidVector += vect.normalized * (radiusSquare - sqrMagnitude) / radiusSquare;
            }
        }

        //Finalize avoidance and cohesion
        massCenter /= mass;
        cohesionVector = (massCenter - (Vector2)transform.position).normalized;
        if(avoidVector.sqrMagnitude > 1)
            avoidVector = avoidVector.normalized;

        //Deal with position
        Vector2 finalVector = (avoidVector * avoidStrenth + targetVector * targetStrenth + cohesionVector * cohesionStrenth).normalized;
        direction = Vector2.Lerp(direction, finalVector, 0.2f).normalized;
        transform.position += (Vector3)direction * speed * Mathf.Clamp01((2 * distance / keepDistance) - 1f) * Time.fixedDeltaTime;

        //Deal with rotation
        float angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;
        Quaternion quaternion = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, steerStrenth);

        //Set the level of this bot based on it's surroundings.
        if (surroundings < 2)
            _level = 1;
        else if (surroundings < 4)
            _level = 2;
        else
            _level = 3;

        levelSmoothed = Mathf.Lerp(levelSmoothed, _level, levelSmoothLerp);
        sr.color = gradient.Evaluate(levelSmoothed / 3f);
    }

    public override void OnKilled()
    {
        bots.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
