using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;

public abstract class AIState : MonoBehaviour
{
    protected AIStateMachine stateMachine;
    protected Transform main;
    [SerializeField]
    protected bool _overrideTransformUpdate;
    public bool overrideTransformUpdate { get { return _overrideTransformUpdate; } private set { } }
    
    protected List<(DanmakuParticleEmitter, float)> followParticles = new List<(DanmakuParticleEmitter, float)>();
    protected List<IEnumerator> asyncs = new List<IEnumerator>();

    [SerializeField]
    protected UltEvent OnEnter = new UltEvent();
    [SerializeField]
    protected UltEvent OnUpdate = new UltEvent();
    [SerializeField]
    protected UltEvent OnExit = new UltEvent();

    public void SetMachine(AIStateMachine machine)
    {
        stateMachine = machine;
        main = stateMachine.gameObject.transform;
    }

    protected void SelfEndState()
    {
        stateMachine.InterruptState();
    }

    public void StateEnter()
    {
        OnEnter.Invoke();
        OnStateEnter();
    }

    protected virtual void OnStateEnter() { }

    public void StateUpdate(float delta)
    {
        OnUpdate.Invoke();
        
        if (_overrideTransformUpdate)
            UpdateTransform(delta);

        if(stateMachine.target != null)
        {
            foreach ((DanmakuParticleEmitter, float) data in followParticles)
            {
                if (data.Item2 == 0)
                    continue;

                Transform t = data.Item1.parent;
                Vector2 lookAt = stateMachine.target.position - t.position;
                float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg;
                Quaternion quaternion = Quaternion.Euler(0, 0, angle);

                t.rotation = Quaternion.Slerp(t.rotation, quaternion, data.Item2);
            }
        }

        OnStateUpdate(delta);
    }

    protected virtual void UpdateTransform(float delta) { }

    protected virtual void OnStateUpdate(float delta) { }

    public void StateExit()
    {
        followParticles.Clear();
        OnExit.Invoke();
        StopAllCoroutines();
        asyncs.Clear();
        OnStateExit();
    }

    protected virtual void OnStateExit() { }

    public void AddFollowParticles(DanmakuParticleEmitter particle, float followStr)
    {
        followParticles.Add((particle, followStr));
    }
}
