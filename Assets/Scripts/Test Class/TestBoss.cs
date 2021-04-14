using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : GameBehaviour
{
    private List<DanmakuParticleEmitter> danmakuEmitters = new List<DanmakuParticleEmitter>();

    [SerializeField]
    private List<AIEmitterSetting> emitterSettings = new List<AIEmitterSetting>();

    public int emitterCount;
    public bool rage;

    public override void GameFixedUpdate()
    {
        foreach (DanmakuParticleEmitter emitter in danmakuEmitters)
            emitter.Update(Time.deltaTime);

        for (int i = danmakuEmitters.Count - 1; i >= 0; i--)
            if (danmakuEmitters[i].activeCount <= 0)
                danmakuEmitters.RemoveAt(i);

        emitterCount = danmakuEmitters.Count;
    }

    public DanmakuParticleEmitter AddDanmaku(int callIndex)
    {
        //DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(emitterSettings[callIndex].particle, emitterSettings[callIndex].location, rage);
        //danmakuEmitters.Add(emitter);
        return null;
    }
}
