using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIStateMachine))]
public class AIHelper : GameBehaviour
{
    private AIStateMachine AI = null;
    private AIState currentState = null;

    private Dictionary<int, DanmakuParticleEmitter> particleDict = new Dictionary<int, DanmakuParticleEmitter>();

    public override void GameAwake()
    {
        if (TryGetComponent(out AI) == false)
            Debug.LogWarning("There's no AIStateMachine on game object " + gameObject.name + ".");
        else
        {
            AI.EnterNewState.AddListener(AIEnterState);
            AI.ExitState.AddListener(AIExitState);
        }
    }

    private void AIEnterState()
    {
        currentState = AI.currentState;
    }

    private void AIExitState()
    {
        currentState = null;
        particleDict.Clear();
    }

    public DanmakuParticleEmitter SpawnParticle(DanmakuParticleData data, GameObject origin, bool followOrigin, Vector2 position, float rotation, float time)
    {
        GameObject go = new GameObject();
        if (origin != null)
            go.transform.SetParent(origin.transform);
        go.transform.localPosition = position;
        go.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        if(!followOrigin)
            go.transform.SetParent(null);
        DanmakuParticleEmitter emitter = new DanmakuParticleEmitter(data, go.transform, time);

        return emitter;
    }

    public void SaveParticle(int key, DanmakuParticleEmitter particle)
    {
        particleDict.Add(key, particle);
    }

    public DanmakuParticleEmitter ExtractParticle(int key)
    {
        DanmakuParticleEmitter particle = particleDict[key];
        particleDict.Remove(key);
        return particle;
    }

    public void SetParticleUpdate(DanmakuParticleEmitter particle, bool updatePosition, bool updateRotation, bool updateScale)
    {
        particle.SetUpdate(updatePosition, updateRotation, updateScale);
    }

    public void Test(Vector2[] vector2s)
    {

    }
}
