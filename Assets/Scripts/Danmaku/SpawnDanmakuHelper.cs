using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDanmakuHelper : GameBehaviour
{
    private bool isRage = false;

    private Dictionary<int, DanmakuParticleEmitter> particleDict = new Dictionary<int, DanmakuParticleEmitter>();

    public override void GameAwake()
    {
        AIStateMachine stateMachine;
        if(TryGetComponent(out stateMachine))
        {
            stateMachine.ExitState.AddListener(StopAll);
        }
    }

    public void SetRage(bool rage)
    {
        isRage = rage;
    }

    public void SpawnParticle(DanmakuParticleData data, GameObject parent, float time, bool updatePosition, bool updateRotation, bool updateScale, int key)
    {
        DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(data, parent.transform, time, isRage);
        emitter.SetUpdate(updatePosition, updateRotation, updateScale);
        if (particleDict.ContainsKey(key))
            particleDict[key] = emitter;
        else
            particleDict.Add(key, emitter);
    }

    public void SpawnOneShot(DanmakuParticleData data, GameObject parent, bool updatePosition, bool updateRotation, bool updateScale, int key)
    {
        DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(data, parent.transform, 0, isRage);
        emitter.SetUpdate(updatePosition, updateRotation, updateScale);
        if (particleDict.ContainsKey(key))
            particleDict[key] = emitter;
        else
            particleDict.Add(key, emitter);
    }

    public void SetParticleUpdate(int key, bool updatePosition, bool updateRotation, bool updateScale)
    {
        if (particleDict.ContainsKey(key))
            particleDict[key].SetUpdate(updatePosition, updateRotation, updateScale);
    }

    public void StopAll()
    {
        foreach (int i in particleDict.Keys)
            particleDict[i].StopSpawnBullets();
        particleDict.Clear();
    }

    public void KillAll()
    {
        foreach (int i in particleDict.Keys)
            particleDict[i].KillAllBullets();
        particleDict.Clear();
    }
}
