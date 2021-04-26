using System;
using System.Collections.Generic;
using UnityEngine;

public enum EmitType
{
    OneShot = 0,
    Particle = 1
}

public enum ValueType
{
    Constant = 0,
    Random = 1
}

public enum ShapeType
{ 
    Sector = 0,
    Edge = 1
}

public enum DistributeType
{
    Random = 0,
    Repeat = 1,
    RepeatRandom = 2,
    BurstEven = 3
}

public enum PathType
{ 
    Line = 0,
    Sine = 1,
}

[System.Serializable]
public struct EmitModule
{
    [SerializeField]
    private DanmakuPattern[] _patterns;
    public DanmakuPattern[] patterns { get { return _patterns; } private set { } }

    [SerializeField, Range(0.1f, 20f)]
    private float _liveTime;
    public float liveTime { get { return _liveTime; } private set { } }

    [SerializeField, Range(0.1f, 50f)]
    private float _rateOverTime;
    public float rateOverTime { get { return _rateOverTime; } private set { } }
    public float shootDelta { get { return 1 / _rateOverTime; } private set { } }
}

[System.Serializable]
public struct ShapeModule
{
    [SerializeField]
    private ShapeType _shapeType;
    public ShapeType shapeType { get { return _shapeType; } private set { } }
    [SerializeField]
    private DistributeType _distributeType;
    public DistributeType distributeType { get { return _distributeType; } private set { } }

    [SerializeField, Range(0f, 360f)]
    private float _spread;
    public float spread { get { return _spread; } private set { } }

    [SerializeField, Range(0f, 10f)]
    private float _radius;
    public float radius { get { return _radius; } private set { } }

    [SerializeField, Range(0f, 1f)]
    private float _radiusThickness;
    public float radiusThickness { get { return _radiusThickness; } private set { } }

    [SerializeField, Range(0f, 50f)]
    private float _edgeWidth;
    public float edgeWidth { get { return _edgeWidth; } private set { } }

    [SerializeField, Range(0.1f, 5f)]
    private float _cycleTime;
    public float cycleTime { get { return _cycleTime; } private set { } }

    [SerializeField, Range(1, 30)]
    private int _burstCount;
    public int burstCount { get { return _burstCount; } private set { } }
}

[System.Serializable]
public struct PatternModule
{
    [SerializeField]
    private Easing _easeType;
    public Easing easeType { get { return _easeType; } private set { } }
    [SerializeField]
    private ValueType _scaleType;
    public ValueType scaleType { get { return _scaleType; } private set { } }
    [SerializeField]
    private ValueType _rotationType;
    public ValueType rotationType { get { return _rotationType; } private set { } }

    [SerializeField, Range(0.1f, 50f)]
    private float _easeTime;
    public float easeTime { get { return _easeTime; } private set { } }

    [SerializeField, Range(0.1f, 100f)]
    private float _scale;
    public float scale { get { return _scale; } private set { } }

    [SerializeField, Range(0.1f, 100f)]
    private float _minScale;
    public float minScale { get { return _minScale; } private set { } }
    
    [SerializeField, Range(0.1f, 100f)]
    private float _maxScale;
    public float maxScale { get { return _maxScale; } private set { } }

    [SerializeField, Range(-360f, 360f)]
    private float _rotation;
    public float rotation { get { return _rotation; } private set { } }

    [SerializeField, Range(-360f, 360f)]
    private float _minRotation;
    public float minRotation { get { return _minRotation; } private set { } }

    [SerializeField, Range(-360f, 360f)]
    private float _maxRotation;
    public float maxRotation { get { return _maxRotation; } private set { } }
}

[System.Serializable]
public struct PathModule
{
    [SerializeField]
    private PathType _pathType;
    public PathType pathType { get { return _pathType; } private set { } }
    [SerializeField]
    private ValueType _speedType;
    public ValueType speedType { get { return _speedType; } private set { } }

    [SerializeField, Range(-30f, 30f)]
    private float _speed;
    public float speed { get { return _speed; } private set { } }

    [SerializeField, Range(-30f, 30f)]
    private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } private set { } }

    [SerializeField, Range(-30f, 30f)]
    private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } private set { } }

    [SerializeField, Range(0.1f, 10f)]
    private float _waveAmp;
    public float waveAmp { get { return _waveAmp; } private set { } }

    [SerializeField, Range(0.1f, 2f)]
    private float _waveHz;
    public float waveHz { get { return _waveHz; } private set { } }

    [SerializeField, Range(0f, 1f)]
    private float _waveOffest;
    public float waveOffest { get { return _waveOffest; } private set { } }
}

[System.Serializable]
public struct SubEmitModule
{
    [SerializeField]
    private DanmakuPattern _pattern;
    public DanmakuPattern pattern { get { return _pattern; } private set { } }

    [SerializeField, Range(0.02f, 1f)]
    private float _spawnTime;
    public float spawnTime { get { return _spawnTime; } private set { } }

    [SerializeField]
    private Easing _easeType;
    public Easing easeType { get { return _easeType; } private set { } }

    [SerializeField, Range(0.1f, 50f)]
    private float _easeTime;
    public float easeTime { get { return _easeTime; } private set { } }

    [SerializeField, Range(0.1f, 100f)]
    private float _scale;
    public float scale { get { return _scale; } private set { } }

    [SerializeField, Range(-360f, 360f)]
    private float _rotation;
    public float rotation { get { return _rotation; } private set { } }
}

[System.Serializable]
public struct Bound
{
    public Transform parent;
    public float xSize;
    public float ySize;

    public bool OverBound(Vector2 position)
    {
        float maxX = parent.position.x + (xSize / 2);
        float minX = parent.position.x - (xSize / 2);
        float maxY = parent.position.y + (ySize / 2);
        float minY = parent.position.y - (ySize / 2);

        return (position.x > maxX || position.x < minX || position.y > maxY || position.y < minY);
    }
}

[System.Serializable]
public class DanmakuTarget
{
    private IDanmakuTarget danmakuTarget;

    public DanmakuTarget(IDanmakuTarget target)
    {
        danmakuTarget = target;
    }

    public bool TouchTarget(Vector2 position, float r)
    {
        if (danmakuTarget == null || danmakuTarget.target == null)
            return false;
        if (danmakuTarget.isImmune)
            return false;
        return ((Vector2)danmakuTarget.target.transform.position - position).magnitude < danmakuTarget.hitRadius + r;
    }
}

public static class EmitterHelper
{
    //PR stands for Position and Rotation
    private delegate Vector3 PRFunction(ShapeModule shape, float time, int index);
    private static readonly Dictionary<(ShapeType, DistributeType), PRFunction> PRDictionary = new Dictionary<(ShapeType, DistributeType), PRFunction>()
    {
        { (ShapeType.Sector, DistributeType.Random), new PRFunction(SectorRandom) },
        { (ShapeType.Sector, DistributeType.Repeat), new PRFunction(SectorRepeat) },
        { (ShapeType.Sector, DistributeType.RepeatRandom), new PRFunction(SectorRepeatRandom) },
        { (ShapeType.Sector, DistributeType.BurstEven), new PRFunction(SectorBurstEven)},
        { (ShapeType.Edge, DistributeType.Random), new PRFunction(EdgeRandom) },
        { (ShapeType.Edge, DistributeType.Repeat), new PRFunction(EdgeRepeat) },
        { (ShapeType.Edge, DistributeType.RepeatRandom), new PRFunction(EdgeRepeatRandom) },
        { (ShapeType.Edge, DistributeType.BurstEven), new PRFunction(EdgeBurstEven) }
    };

    private static Vector3 SectorRandom(ShapeModule shape, float time, int index)
    {
        float radius = UnityEngine.Random.Range(shape.radius * (1 - shape.radiusThickness), shape.radius);
        float degree = UnityEngine.Random.Range(-shape.spread / 2, shape.spread / 2) * Mathf.Deg2Rad;
        return new Vector3(radius, degree, degree);
    }

    private static Vector3 SectorRepeat(ShapeModule shape, float time, int index)
    {
        float radius = UnityEngine.Random.Range(shape.radius * (1 - shape.radiusThickness), shape.radius);
        float degree = (time % shape.cycleTime) / shape.cycleTime * shape.spread - (shape.spread / 2);
        degree *= Mathf.Deg2Rad;
        return new Vector3(radius, degree, degree);
    }

    private static Vector3 SectorRepeatRandom(ShapeModule shape, float time, int index)
    {
        float radius = UnityEngine.Random.Range(shape.radius * (1 - shape.radiusThickness), shape.radius);
        float degree = (time % shape.cycleTime) / shape.cycleTime * shape.spread - (shape.spread / 2);
        degree += UnityEngine.Random.Range(shape.spread * -0.02f, shape.spread * 0.02f);
        degree *= Mathf.Deg2Rad;
        return new Vector3(radius, degree, degree);
    }

    private static Vector3 SectorBurstEven(ShapeModule shape, float time, int index)
    {
        float radius = UnityEngine.Random.Range(shape.radius * (1 - shape.radiusThickness), shape.radius);
        float gap = shape.spread / shape.burstCount;
        float degree = -(shape.spread / 2) + (gap / 2) + index * gap;
        degree *= Mathf.Deg2Rad;
        return new Vector3(radius, degree, degree);
    }
    //例子：
    //這是板子
    //   ↓
    //   ┼ (0,2) 以極座標表示為(2,90°)
    //   |
    //   |--→ 法向量(1,0)，以極座標表示為(1,0°)
    //   |
    //   |
    private static Vector3 EdgeRandom(ShapeModule shape, float time, int index)
    {
        float radius = UnityEngine.Random.Range(-shape.edgeWidth / 2, shape.edgeWidth / 2);
        return new Vector3(radius, 90 * Mathf.Deg2Rad, 0);
    }

    private static Vector3 EdgeRepeat(ShapeModule shape, float time, int index)
    {
        float radius = (time % shape.cycleTime) / shape.cycleTime * shape.edgeWidth - (shape.edgeWidth / 2);
        return new Vector3(radius, 90 * Mathf.Deg2Rad, 0);
    }

    private static Vector3 EdgeRepeatRandom(ShapeModule shape, float time, int index)
    {
        float radius = (time % shape.cycleTime) / shape.cycleTime * shape.edgeWidth - (shape.edgeWidth / 2);
        radius += UnityEngine.Random.Range(shape.edgeWidth * -0.02f, shape.edgeWidth * 0.02f);
        return new Vector3(radius, 90 * Mathf.Deg2Rad, 0);
    }

    private static Vector3 EdgeBurstEven(ShapeModule shape, float time, int index)
    {
        float gap = shape.edgeWidth / shape.burstCount;
        float radius = -(shape.edgeWidth / 2) + (gap / 2) + index * gap;
        return new Vector3(radius, 90 * Mathf.Deg2Rad, 0);
    }

    /// <summary>
    /// 回傳彈幕在相對座標中的初始位置(x,y)和發射角度(z)。位置以極座標(r,Θ)表示，方便計算。回傳的y跟z都是弧度制。
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static Vector3 GetInitPositionAndRotation(ShapeModule shape ,float time, int index)
    {
        return PRDictionary[(shape.shapeType, shape.distributeType)](shape, time, index);
    }

    /// <summary>
    /// 回傳彈幕的終端大小(x)和角速度(y)。回傳的y是弧度制。
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static Vector2 GetScaleAndAngularSpeed(PatternModule pattern)
    {
        float scale = (pattern.scaleType == ValueType.Constant) ? pattern.scale : UnityEngine.Random.Range(pattern.minScale, pattern.maxScale);
        float angular = (pattern.rotationType == ValueType.Constant) ? pattern.rotation : UnityEngine.Random.Range(pattern.minRotation, pattern.maxRotation);
        angular *= Mathf.Deg2Rad;
        return new Vector2(scale, angular);
    }

    public static float GetInitSpeed(PathModule path)
    {
        return (path.speedType == ValueType.Constant) ? path.speed : UnityEngine.Random.Range(path.minSpeed, path.maxSpeed);
    }

    public static Vector2 GetRawVelocity(PathModule path, float time, float initVelocity)
    {
        if (path.pathType == PathType.Line)
            return new Vector2(initVelocity, 0);
        else
            return new Vector2(initVelocity, path.waveAmp * (float)Math.Cos(Math.PI * 2 * (path.waveHz * time + path.waveOffest)));
    }
}
