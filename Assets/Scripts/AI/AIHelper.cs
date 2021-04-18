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

    public void SpawnParticle(AIEmitterSetting setting, GameObject parent, float targetStrenth, int index)
    {
        DanmakuParticleEmitter emitter = setting.MakeEmitter((parent == null)? null : parent.transform);
        if (currentState != null)
            currentState.AddParticles(emitter);
        particleDict.Add(index, emitter);
    }

    public void SetParticleUpdate(int index, bool updatePosition, bool updateRotation, bool updateScale)
    {
        particleDict[index].SetUpdate(updatePosition, updateRotation, updateScale);
    }
}
