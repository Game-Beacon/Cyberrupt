using UnityEngine;

public class Bomb : GameBehaviour, IDanmakuTarget
{
    public Transform target { get { return transform; } }

    [SerializeField]
    private float _hitRadius;
    private float currentHitRadius = 0;
    public float hitRadius { get { return currentHitRadius; } }

    public bool isImmune =>
        throw new System.NotImplementedException();

    //====================

    [SerializeField]
    private float bombTime;
    [SerializeField]
    private float expandTime;
    [SerializeField]
    private Easing ease;

    private DanmakuManager danmakuManager;
    private DanmakuObstacle obstacle;
    private CircleCollider2D circleCollider;
    private float timer = 0;

    public override void GameStart()
    {
        danmakuManager = DanmakuManager.instance;
        obstacle = danmakuManager.AddObstacle(this);
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public override void GameUpdate()
    {
        // Update hit radius
        if(timer >= expandTime)
            currentHitRadius = _hitRadius;
        else
            currentHitRadius = EaseLibrary.CallEaseFunction(ease, timer / expandTime) * _hitRadius;
        circleCollider.radius = currentHitRadius;
        // Kill at timer end
        if(timer >= bombTime)
        {
            KillBehaviour(true);
            update = false;
        }
        timer += Time.deltaTime;
    }

    public override void OnKilled()
    {
        danmakuManager.RemoveObstacle(obstacle);
        obstacle = null;
    }
}