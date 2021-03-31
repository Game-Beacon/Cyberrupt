using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanmakuParticleEmitter
{
    private DanmakuParticleData danmakuData;
    private static DanmakuManager manager = null;
    private Transform transform;

    private Vector2 worldPosition { get { return transform.position; } }
    private float worldRotation { get { return transform.rotation.eulerAngles.z; } }

    private float _time = 0;
    public float time { get { return _time; } private set { } }

    private float shootDelta;

    private int _activeCount;
    public int activeCount { get { return _activeCount; } private set { } }

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

        public bool[] actives;
    }

    private List<ActiveBullets> patterns = new List<ActiveBullets>();
    private List<ActiveBullets> subPatterns = new List<ActiveBullets>();
    private Queue<SpriteRenderer> returnPoolQueue = new Queue<SpriteRenderer>();

    public DanmakuParticleEmitter(DanmakuParticleData data, Transform t)
    {
        if (manager == null)
            manager = DanmakuManager.instance;
        danmakuData = data;
        transform = t;
    }

    public void Update(float delta)
    {
        SpawnPattern();

        int len = patterns.Count - 1;
        for (int i = len; i >= 0; i--)
            UpdatePattern(i, delta);

        for (int i = len; i >= 0; i--)
            if (patterns[i].activeCount <= 0)
                patterns.RemoveAt(i);

        len = subPatterns.Count - 1;

        if (len >= 0)
        {
            for (int i = len; i >= 0; i--)
                UpdateSubPattern(i, delta);

            for (int i = len; i >= 0; i--)
                if (subPatterns[i].activeCount <= 0)
                    subPatterns.RemoveAt(i);
        }

        _time += delta;
        shootDelta -= delta;

        manager.RecycleBullet(ref returnPoolQueue);
    }

    private void SpawnPattern()
    {
        if (shootDelta <= 0)
        {
            int count = danmakuData.emitModule.pattern.count;
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
                newActive.danmaku = danmakuData.emitModule.pattern;
                newActive.shootRotation = worldRotation * Mathf.Deg2Rad + rawPR.z;

                Vector2 scaleAndAngular = EmitterHelper.GetScaleAndAngularSpeed(danmakuData.patternModule);

                newActive.scale = scaleAndAngular.x;
                newActive.angularSpeed = scaleAndAngular.y;

                newActive.children = new GameObject[count];
                newActive.srs = sprites.ToArray();
                newActive.actives = new bool[count];
                newActive.activeCount = count;

                for (int j = 0; j < count; j++)
                {
                    sprites[j].sprite = danmakuData.emitModule.pattern.data[j].bullet.sprite;
                    newActive.children[j] = sprites[j].gameObject;
                    newActive.actives[j] = true;
                }

                newActive.timer = 0;
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

        Vector2 rawVelocity = EmitterHelper.GetRawVelocity(danmakuData.pathModule, bullets.timer, bullets.linearSpeed);
        Vector2 velocity = Matrix2x2.RotationMatrix(bullets.shootRotation).Transform(rawVelocity);

        bullets.parent += velocity * delta;
        Matrix2x2 localMatrix = bullets.scale * EaseLibrary.CallEaseFunction(danmakuData.patternModule.easeType, bullets.timer / danmakuData.patternModule.easeTime) * Matrix2x2.RotationMatrix(bullets.angularSpeed * bullets.timer + bullets.shootRotation);

        //如果彈幕存在超過存活時間，強制銷毀彈幕
        //否則更新彈幕
        if (bullets.timer >= bullets.liveTime)
        {
            for (int i = 0; i < bullets.children.Length; i++)
            {
                if (bullets.actives[i])
                {
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

                    manager.TouchTarget(pos, bullets.danmaku.data[i].bullet.radius);

                    if (manager.OverBound(pos))
                    {
                        bullets.actives[i] = false;
                        bullets.activeCount--;
                        bullets.children[i].SetActive(false);
                        _activeCount--;
                        returnPoolQueue.Enqueue(bullets.srs[i]);
                    }
                }
            }

            if (danmakuData.useSubEmitter && (int)(bullets.timer / danmakuData.subEmitModule.spawnTime) != (int)((bullets.timer + delta) / danmakuData.subEmitModule.spawnTime))
                SpawnSubPattern(bullets.parent, bullets.shootRotation);
        }

        bullets.timer += delta;
    }

    private void UpdateSubPattern(int index, float delta)
    {
        ActiveBullets bullets = subPatterns[index];

        Matrix2x2 localMatrix = bullets.scale * EaseLibrary.CallEaseFunction(danmakuData.subEmitModule.easeType, bullets.timer / danmakuData.subEmitModule.easeTime) * Matrix2x2.RotationMatrix(bullets.angularSpeed * bullets.timer + bullets.shootRotation);

        //如果彈幕存在超過存活時間，強制銷毀彈幕
        //否則更新彈幕
        if (bullets.timer >= bullets.liveTime)
        {
            for (int i = 0; i < bullets.children.Length; i++)
            {
                if (bullets.actives[i])
                {
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

                    manager.TouchTarget(pos, bullets.danmaku.data[i].bullet.radius);

                    if (manager.OverBound(pos))
                    {
                        bullets.actives[i] = false;
                        bullets.activeCount--;
                        bullets.children[i].SetActive(false);
                        _activeCount--;
                        returnPoolQueue.Enqueue(bullets.srs[i]);
                    }
                }
            }
        }

        bullets.timer += delta;
    }

    private void SpawnSubPattern(Vector2 position, float rotation)
    {
        int count = danmakuData.subEmitModule.pattern.count;

        List<SpriteRenderer> sprites = new List<SpriteRenderer>();
        manager.RequestBullets(count, ref sprites);

        if (sprites.Count != count)
            return;

        ActiveBullets newActive = new ActiveBullets();

        newActive.parent = position;
        newActive.danmaku = danmakuData.subEmitModule.pattern;
        newActive.shootRotation = rotation;
        newActive.scale = danmakuData.subEmitModule.scale;
        newActive.angularSpeed = danmakuData.subEmitModule.rotation * Mathf.Deg2Rad;

        newActive.children = new GameObject[count];
        newActive.srs = sprites.ToArray();
        newActive.actives = new bool[count];
        newActive.activeCount = count;

        for (int j = 0; j < count; j++)
        {
            sprites[j].sprite = danmakuData.subEmitModule.pattern.data[j].bullet.sprite;
            newActive.children[j] = sprites[j].gameObject;
            newActive.actives[j] = true;
        }

        newActive.timer = 0;
        newActive.liveTime = danmakuData.subEmitModule.easeTime;
        newActive.isMain = false;

        newActive.linearSpeed = 0;

        subPatterns.Add(newActive);
        _activeCount += count;
    }
}
