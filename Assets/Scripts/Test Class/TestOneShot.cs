using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOneShot : GameBehaviour
{
    [SerializeField]
    private DanmakuPattern pattern;
    private DanmakuOneShot shooter;

    public override void GameStart()
    {
        shooter = new DanmakuOneShot(pattern, transform);
    }

    public override void GameFixedUpdate()
    {
        shooter.Update(Time.fixedDeltaTime);
    }
}
