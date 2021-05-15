using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanmakuOneShot : IDanmaku
{
    private DanmakuPattern pattern;
    private static DanmakuManager manager = null;
    private Transform transform;
    public Transform parent { get { return transform; } private set { } }

    private Vector2 worldPosition { get { return transform.position; } }
    private float worldRotation { get { return transform.rotation.eulerAngles.z; } }
    private Vector2 worldScale { get { return transform.lossyScale; } }

    private float _time = 0;
    public float time { get { return _time; } private set { } }

    private int _activeCount;
    public int activeCount { get { return _activeCount; } private set { } }

    public bool spawnBullets { get { return false; } }
    private bool useRageBullet;

    private struct Bullet
    {
        public GameObject gameObject;
        public SpriteRenderer sr;
        public bool isActive;
    }

    private Bullet[] bullets;
    private Queue<SpriteRenderer> returnPoolQueue = new Queue<SpriteRenderer>();

    public DanmakuOneShot(DanmakuPattern data, Transform t, bool rage = false)
    {
        if (manager == null)
            manager = DanmakuManager.instance;
        pattern = data;
        transform = t;
        useRageBullet = rage;
        bullets = new Bullet[pattern.count];
        Init();
    }

    private void Init()
    {
        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        manager.RequestBullets(pattern.count, ref sprites);

        for (int i = 0; i < pattern.count; i++)
        {
            bullets[i].sr = sprites[i];
            //bullets[i].sr.sprite = (useRageBullet) ? pattern.data[i].bullet.rageSprite : pattern.data[i].bullet.sprite;
            bullets[i].gameObject = sprites[i].gameObject;
            bullets[i].gameObject.transform.position = worldPosition;
            bullets[i].isActive = true;
        }

        _activeCount = pattern.count;
    }

    public void Update(float delta)
    {
        Matrix2x2 localMatrix = Matrix2x2.RotationMatrix(worldRotation * Mathf.Deg2Rad) * Matrix2x2.ScaleMatrix(worldScale.x, worldScale.y);

        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i].isActive)
            {
                bullets[i].gameObject.transform.position = worldPosition + localMatrix.Transform(pattern.data[i].localPosition);
                Vector2 pos = bullets[i].gameObject.transform.position;

                manager.TouchTarget(pos, pattern.data[i].bullet.radius);

                if (manager.OverBound(pos))
                {
                    manager.RequestParticlePlay(bullets[i].gameObject.transform.position);
                    bullets[i].isActive = false;
                    bullets[i].gameObject.SetActive(false);
                    _activeCount--;
                    returnPoolQueue.Enqueue(bullets[i].sr);
                }
            }
        }

        _time += delta;

        manager.RecycleBullet(ref returnPoolQueue);
    }

    public void OnDispose()
    {
        Transform t = transform;
        Object.Destroy(t.gameObject);
    }
}
