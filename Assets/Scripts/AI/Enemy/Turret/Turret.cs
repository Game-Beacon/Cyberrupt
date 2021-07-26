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
    private List<Enemy> shields;
    [Header("Path")]
    [SerializeField]
    private Rail path;
    [SerializeField]
    private float pathLength;
    [SerializeField]
    private float pathSpeed;
    [SerializeField]
    private float pathDuration;
    // Add this for padding
    [Header("")]
    [SerializeField, Range(0, 0.5f)]
    private float lockStrenth;

    //用來判斷砲台是否"離開"螢幕邊界
    //InScreen的判斷標準是敵人的位置是否在螢幕內
    //如果上一個frame的InScreen為true，這個frame卻為false，那就代表敵人的座標已經離開邊界
    private bool previousInScreen;

    private Player player;

    private Vector2 startPosition;
    private Vector2 currentPosition;
    private Vector2 endPosition;
    private Vector2 pathDirection;

    protected override void EnemyAwake()
    {
        previousInScreen = false;

        _stateMachine = GetComponent<AIStateMachine>();
        _danmakuHelper = GetComponent<SpawnDanmakuHelper>();
        _stateMachine.OnUpdateTransform.AddListener(UpdateTransform);
    }

    protected override void EnemyStart()
    {
        player = DependencyContainer.GetDependency<Player>() as Player;
        _target = player.transform;
        StartCoroutine(CreatePath());
    }

    protected override void EnemyUpdate()
    {
        if (previousInScreen == true && _inScreen == false)
            StartCoroutine(DelayKill());

        previousInScreen = _inScreen;
    }

    private IEnumerator CreatePath()
    {
        startPosition = transform.position;
        currentPosition = startPosition;
        Vector2 pinpoint = screen.GetRandomPointInScreen(2.5f);
        endPosition = startPosition + (pinpoint - startPosition).normalized * pathLength;
        pathDirection = (endPosition - startPosition).normalized;
        this.path.PreparePath(startPosition, endPosition, pathDuration);
        yield return this.path.StartCoroutine(this.path.Create());
    }

    private void UpdateTransform()
    {
        // Move
        currentPosition += pathDirection * Mathf.Min((endPosition - currentPosition).magnitude, pathSpeed * Time.fixedDeltaTime);
        root.position = currentPosition;
        // Update rotation
        Vector2 direction = target.position - parent.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion lookAt = Quaternion.Euler(0, 0, angle);
        parent.rotation = Quaternion.Slerp(parent.rotation, lookAt, lockStrenth);
    }

    public override void OnKilled()
    {
        foreach (Enemy enemy in shields)
            if (enemy != null)
                enemy.Die();
        // Detach path and clear it
        this.path.transform.parent = null;
        // Call `StartCoroutine` by this.path so that
        // the coroutine will not be killed by Turret.
        // This ensure that the coroutine will be
        // correctly finished.
        this.path.StartCoroutine(this.path.Clear());

        if (parent != null)
            DestroySafe(parent.gameObject/*, 0.03f*/);
        if (root != null)
            DestroySafe(root.gameObject/*, 0.03f*/);
    }

    IEnumerator DelayKill()
    {
        yield return new WaitForSeconds(1);
        Die();
    }
}
