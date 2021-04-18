using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanmakuParticleEmitter : IDanmaku
{
    private DanmakuParticleData danmakuData;
    private static DanmakuManager manager = null;
    private Transform transform;
    public Transform parent { get { return transform; } private set { } }

    private Vector2 worldPosition { get { return transform.position; } }
    private float worldRotation { get { return transform.rotation.eulerAngles.z; } }

    private float _time = 0;
    public float time { get { return _time; } private set { } }
    private float spawnTime;

    private float shootDelta;

    private int _activeCount;
    public int activeCount { get { return _activeCount; } private set { } }

    private bool _spawnBullets = true;
    public bool spawnBullets { get { return _spawnBullets; } private set { } }
    
    private bool updatePosition = true;
    private bool updateRotation = true;
    private bool updateScale = true;

    private bool useRageBullet;

    private class ActiveBullets
    {
        public Vector2 parent;
        public DanmakuPattern danmaku;

        public float liveTime;
        public bool isMain;

        public float scale;
        public float linearSpeed;
        public float angularSpeed;
        public float shootRotation;

        public GameObject[] children;
        public SpriteRenderer[] srs;

        public int activeCount;
        public float timer;
        public float scaleTimer;
        public float rotationTimer;
        public float positionTimer;

        public bool[] actives;
    }

    private List<ActiveBullets> patterns = new List<ActiveBullets>();
    private Queue<SpriteRenderer> returnPoolQueue = new Queue<SpriteRenderer>();

    public DanmakuParticleEmitter(DanmakuParticleData data, Transform t, float spawn = -1, bool rage = false)
    {
        if (manager == null)
            manager = DanmakuManager.instance;
        if(manager != null)
            manager.AddDanmaku(this);

        danmakuData = data;
        transform = t;
        spawnTime = spawn;
        useRageBullet = rage;
    }

    public void Update(float delta)
    {
        if (_spawnBullets)
            SpawnPattern();

        if (spawnTime >= 0 && _time >= spawnTime)
            _spawnBullets = false;

        int len = patterns.Count - 1;
        for (int i = len; i >= 0; i--)
            UpdatePattern(i, delta);

        for (int i = len; i >= 0; i--)
            if (patterns[i].activeCount <= 0)
                patterns.RemoveAt(i);

        _time += delta;
        shootDelta -= delta;

        manager.RecycleBullet(ref returnPoolQueue);
    }   

    private void SpawnPattern()
    {
        //如果依附的遊戲物件已經不存在，就不要發射新的子彈
        if (transform == null)
        {
            _spawnBullets = false;
            return;
        }  

        if (shootDelta <= 0)
        {
            DanmakuPattern pattern = danmakuData.emitModule.patterns[Random.Range(0, danmakuData.emitModule.patterns.Length)];
            int count = pattern.count;
            int burst = danmakuData.shapeModule.burstCount;

            shootDelta = danmakuData.emitModule.shootDelta;

            for (int i = 0; i < burst; i++)
            {
                List<SpriteRenderer> sprites = new List<SpriteRenderer>();
                manager.RequestBullets(count, ref sprites);

                if (sprites.Count != count)
                    return;

                ActiveBullets newActive = new ActiveBullets();

                Vector3 rawPR = EmitterHelper.GetInitPositionAndRotation(danmakuData.shapeModule, time, i);
                Vector2 rawOffset = rawPR.x * new Vector2(Mathf.Cos(rawPR.y), Mathf.Sin(rawPR.y));
                Vector2 offset = Matrix2x2.RotationMatrix(worldRotation * Mathf.Deg2Rad).Transform(rawOffset);

                newActive.parent = worldPosition + offset;
                newActive.danmaku = pattern;
                newActive.shootRotation = worldRotation * Mathf.Deg2Rad + rawPR.z;

                Vector2 scaleAndAngular = EmitterHelper.GetScaleAndAngularSpeed(danmakuData.patternModule);

                newActive.scale = scaleAndAngular.x;
                newActive.angularSpeed = scaleAndAngular.y;

                newActive.children = new GameObject[count];
                newActive.srs = sprites.ToArray();
                newActive.actives = new bool[count];
                newActive.activeCount = count;

                if(useRageBullet)
                {
                    for (int j = 0; j < count; j++)
                    {
                        sprites[j].sprite = pattern.data[j].bullet.rageSprite;
                        newActive.children[j] = sprites[j].gameObject;
                        newActive.children[j].transform.position = newActive.parent;
                        newActive.actives[j] = true;
                    }
                }
                else
                {
                    for (int j = 0; j < count; j++)
                    {
                        sprites[j].sprite = pattern.data[j].bullet.sprite;
                        newActive.children[j] = sprites[j].gameObject;
                        newActive.children[j].transform.position = newActive.parent;
                        newActive.actives[j] = true;
                    }
                }
                
                newActive.positionTimer = 0;
                newActive.rotationTimer = 0;
                newActive.scaleTimer = 0;
                newActive.liveTime = danmakuData.emitModule.liveTime;
                newActive.isMain = true;

                float speed = EmitterHelper.GetInitSpeed(danmakuData.pathModule);
                newActive.linearSpeed = speed;

                patterns.Add(newActive);
                _activeCount += count;
            }
        }
    }

    private void UpdatePattern(int index, float delta)
    {
        ActiveBullets bullets = patterns[index];

        Matrix2x2 localMatrix = Matrix2x2.identity;

        Vector2 rawVelocity = EmitterHelper.GetRawVelocity(danmakuData.pathModule, bullets.positionTimer, bullets.linearSpeed);
        Vector2 velocity = Matrix2x2.RotationMatrix(bullets.shootRotation).Transform(rawVelocity);

        if(updatePosition)
            bullets.parent += velocity * delta;
        localMatrix = bullets.scale * EaseLibrary.CallEaseFunction(danmakuData.patternModule.easeType, bullets.scaleTimer / danmakuData.patternModule.easeTime) * Matrix2x2.RotationMatrix(bullets.angularSpeed * bullets.rotationTimer + bullets.shootRotation);

        //如果彈幕存在超過存活時間，強制銷毀彈幕
        //否則更新彈幕
        if (bullets.timer >= bullets.liveTime)
        {
            for (int i = 0; i < bullets.children.Length; i++)
            {
                if (bullets.actives[i])
                {
                    manager.RequestParticlePlay(bullets.children[i].transform.position);
                    bullets.actives[i] = false;
                    bullets.activeCount--;
                    bullets.children[i].SetActive(false);
                    _activeCount--;
                    returnPoolQueue.Enqueue(bullets.srs[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < bullets.children.Length; i++)
            {
                if (bullets.actives[i])
                {
                    bullets.children[i].transform.position = bullets.parent + localMatrix.Transform(bullets.danmaku.data[i].localPosition);
                    Vector2 pos = bullets.children[i].transform.position;

                    if (manager.TouchTarget(pos, bullets.danmaku.data[i].bullet.radius) || manager.OverBound(pos))
                    {
                        manager.RequestParticlePlay(bullets.children[i].transform.position);
                        bullets.actives[i] = false;
                        bullets.activeCount--;
                        bullets.children[i].SetActive(false);
                        _activeCount--;
                        returnPoolQueue.Enqueue(bullets.srs[i]);
                    }
                }
            }
        }

        if (updatePosition)
            bullets.positionTimer += delta;
        if (updateRotation)
            bullets.rotationTimer += delta;
        if (updateScale)
            bullets.scaleTimer += delta;

        bullets.timer += delta;
    }

    public void SetUpdate(bool position, bool rotation, bool scale)
    {
        updatePosition = position;
        updateRotation = rotation;
        updateScale = scale;
    }

    public void StopSpawnBullets()
    {
        _spawnBullets = false;
    }

    public void KillAllBullets()
    {
        _spawnBullets = false;
        int len = patterns.Count - 1;
        for (int i = len; i >= 0; i--)
            patterns[i].timer = patterns[i].liveTime + 1;
    }

    public void OnDispose()
    {
        //Transform t = transform;
        //Object.Destroy(t.gameObject);
    }
}
