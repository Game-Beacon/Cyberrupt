using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class DanmakuManager : GameBehaviour
{
    private static DanmakuManager _instance = null;
    public static DanmakuManager instance { get { return _instance; } private set { } }

    [SerializeField]
    private GameObject poolPrefab;
    private Queue<SpriteRenderer> pool = new Queue<SpriteRenderer>();
    [SerializeField]
    private int poolSize = 2000;

    [SerializeField]
    private int poolCount;

    [SerializeField]
    private DanmakuBound _bound;

    [SerializeField]
    private DanmakuTarget _target = null;
    public DanmakuTarget target { get { return _target; } private set { } }

    private UnityEvent _bulletHitTarget = new UnityEvent();
    public UnityEvent bulletHitTarget { get { return _bulletHitTarget; } private set { } }

    public override void GameAwake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }

        _bound.parent = transform;

        for(int i = 0; i < poolSize; i ++)
        {
            GameObject newObject = Instantiate(poolPrefab);
            SpriteRenderer sr = null;
            Assert.IsTrue(newObject.TryGetComponent(out sr), "There's no sprite renderer on the game object.");
            newObject.SetActive(false);
            pool.Enqueue(sr);
        }

        update = false;
    }

    public override void GameFixedUpdate()
    {
        poolCount = pool.Count;
    }

    public void RequestBullets(int count, ref List<SpriteRenderer> sprites)
    {
        if (pool.Count < count)
            return;

        for (int i = 0; i < count; i++)
        {
            SpriteRenderer sr = pool.Dequeue();
            sr.gameObject.SetActive(true);
            sprites.Add(sr);
        }
    }

    public void RecycleBullet(ref Queue<SpriteRenderer> sprites)
    {
        while (sprites.Count > 0)
            pool.Enqueue(sprites.Dequeue());
    }

    public void SetDanmakuTarget(Transform transform, float r)
    {
        _target = new DanmakuTarget()
        {
            target = transform,
            radius = r
        };
    }

    public bool OverBound(Vector2 position)
    {
        return _bound.OverBound(position);
    }

    public bool TouchTarget(Vector2 position, float radius)
    {
        bool result = _target.TouchTarget(position, radius);
        if(result)
            bulletHitTarget.Invoke();
        return result;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(_bound.xSize, _bound.ySize));
    }
}
