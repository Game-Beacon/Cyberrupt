using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBomb : GameBehaviour, IDanmakuTarget
{
    public Transform target { get { return transform; } }

    [SerializeField]
    private float _hitRadius;
    public float hitRadius { get { return _hitRadius; } }

    public bool isImmune => throw new System.NotImplementedException();

    //====================

    private DanmakuManager danmakuManager;
    private DanmakuObstacle obstacle;

    public override void GameStart()
    {
        danmakuManager = DanmakuManager.instance;
        obstacle = danmakuManager.AddObstacle(this);
    }
}
