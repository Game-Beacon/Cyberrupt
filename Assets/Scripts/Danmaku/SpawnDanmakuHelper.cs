using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDanmakuHelper : GameBehaviour
{
    private Dictionary<int, DanmakuParticleEmitter> particleDict = new Dictionary<int, DanmakuParticleEmitter>();
    private List<DanmakuParticleEmitter> overWriteParticle = new List<DanmakuParticleEmitter>();

    public override void GameAwake()
    {
        AIStateMachine stateMachine;
        if(TryGetComponent(out stateMachine))
        {
            stateMachine.ExitState.AddListener(StopAllOverWrite);
        }
    }

    public void SpawnParticle(DanmakuParticleData data, GameObject parent, float time, bool updatePosition, bool updateRotation, bool updateScale, int key)
    {
        DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(data, parent.transform, time);
        emitter.SetUpdate(updatePosition, updateRotation, updateScale);
        if (particleDict.ContainsKey(key))
        {
            overWriteParticle.Add(particleDict[key]);
            particleDict[key] = emitter;
        } 
        else
            particleDict.Add(key, emitter);
    }

    public void SpawnOneShot(DanmakuParticleData data, GameObject parent, bool updatePosition, bool updateRotation, bool updateScale, int key)
    {
        DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(data, parent.transform, 0);
        emitter.SetUpdate(updatePosition, updateRotation, updateScale);
        if (particleDict.ContainsKey(key))
        {
            overWriteParticle.Add(particleDict[key]);
            particleDict[key] = emitter;
        }
        else
            particleDict.Add(key, emitter);
    }

    public void SetParticleUpdate(int key, bool updatePosition, bool updateRotation, bool updateScale)
    {
        if (particleDict.ContainsKey(key))
            particleDict[key].SetUpdate(updatePosition, updateRotation, updateScale);
    }

    private void StopAllOverWrite()
    {
        foreach (DanmakuParticleEmitter emitter in overWriteParticle)
            emitter.StopSpawnBullets();
        overWriteParticle.Clear();
    }

    public void KillAll()
    {
        foreach (int i in particleDict.Keys)
            particleDict[i].KillAllBullets();
        foreach (DanmakuParticleEmitter emitter in overWriteParticle)
            emitter.KillAllBullets();
        particleDict.Clear();
        overWriteParticle.Clear();
    }
}
