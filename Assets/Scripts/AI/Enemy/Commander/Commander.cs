using System;
using UnityEngine;
using UnityEngine.Events;

public class Commander : Enemy, ITarget, IStateMachine, ISpawnDanmaku
{
    private Transform _target;
    public Transform target { get { return _target; } }

    private AIStateMachine _stateMachine;
    public AIStateMachine stateMachine { get { return _stateMachine; } }

    private SpawnDanmakuHelper _danmakuHelper;
    public SpawnDanmakuHelper danmakuHelper { get { return _danmakuHelper; } }

    public UnityEvent OnUpdateTransform => this.stateMachine.OnUpdateTransform;    
    //====================

    private Player player;

    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float angularSpeed;

    [SerializeField]
    private GameObject bot;
    [SerializeField]
    private EnemyBaseProperty propertyForBot;

    [SerializeField]
    private int maxSpawnCount;
    private int spawnCount = 0;

    [SerializeField]
    private float baseSpawnTime;
    [SerializeField]
    private float spawnTimeDelta;
    [SerializeField]
    private float spawnRadius;

    private float timer = 0;

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

    protected override void EnemyUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && spawnCount < maxSpawnCount)
        {
            float rand = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

            GameObject go = Instantiate(bot, transform.position + new Vector3(Mathf.Cos(rand), Mathf.Sin(rand)) * spawnRadius, Quaternion.identity);
            Enemy newBot = go.GetComponent<Enemy>();
            newBot.OverrideProperty(propertyForBot);
            newBot.OnDeath.AddPersistentCall((Action)BotKilled);
            spawnCount++;
            timer = baseSpawnTime + spawnTimeDelta * spawnCount;
        }
    }

    private void UpdateTransform()
    {
        // Move to target
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.fixedDeltaTime;
        // Aim target
        direction = target.position - muzzle.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        muzzle.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void BotKilled()
    {
        spawnCount--;
    }
}
