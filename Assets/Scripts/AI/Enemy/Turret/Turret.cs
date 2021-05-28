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
    [Header("Path")]
    [SerializeField]
    private LineRenderer lr;
    [SerializeField]
    private float pathLength;
    [SerializeField]
    private float pathSpeed;
    [SerializeField]
    private float pathDuration;
    [SerializeField]
    [Tooltip("A GameObject that will be placed the end of path during generation")]
    private GameObject pathPeek;
    // Add this for padding
    [Header("")]
    [SerializeField, Range(0, 0.5f)]
    private float lockStrenth;

    //用來判斷砲台是否"離開"螢幕邊界
    //CanAttack的判斷標準是敵人的位置是否在螢幕內
    //如果上一個frame的CanAttack為true，這個frame卻為false，那就代表敵人的座標已經離開邊界
    private bool previousCanAttack;

    private Player player;

    private Vector2 startPosition;
    private Vector2 currentPosition;
    private Vector2 endPosition;
    private Vector2 pathDirection;
    // Denote whether the path finishing generation
    private bool isPathReady;

    protected override void EnemyAwake()
    {
        previousCanAttack = false;

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
        if (previousCanAttack == true && _canAttack == false)
            StartCoroutine(DelayKill());

        previousCanAttack = _canAttack;
    }

    private IEnumerator CreatePath()
    {
        startPosition = transform.position;
        currentPosition = startPosition;
        Vector2 pinpoint = manager.GetRandomPointInScreen(2.5f);
        endPosition = startPosition + (pinpoint - startPosition).normalized * pathLength;
        pathDirection = (endPosition - startPosition).normalized;
        var peek = Instantiate(this.pathPeek, this.root);
        // Setup path start point
        lr.positionCount = 2;
        lr.SetPosition(0, currentPosition);
        // Iterate endpoint overtime
        for(float t = 0; t < this.pathDuration; t += Time.deltaTime)
        {
            var railEnd = Vector3.Lerp(currentPosition, endPosition, t / this.pathDuration);
            lr.SetPosition(1, railEnd);
            peek.transform.position = railEnd;
            yield return null;
        }
        // Mark path as ready
        Destroy(peek);
        this.isPathReady = true;
    }

    private void UpdateTransform()
    {
        // Freeze before path finish
        if(!this.isPathReady)
            return;
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
        Destroy(parent.gameObject, 0.03f);
        Destroy(root.gameObject, 0.03f);
    }

    IEnumerator DelayKill()
    {
        yield return new WaitForSeconds(1);
        Die();
    }
}
