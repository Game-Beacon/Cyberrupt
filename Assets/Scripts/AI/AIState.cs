using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public abstract class AIState : MonoBehaviour
{
    protected AIStateMachine stateMachine;
    protected Transform main;
    [SerializeField]
    protected bool _overrideTransformUpdate;
    public bool overrideTransformUpdate { get { return _overrideTransformUpdate; } private set { } }

    [SerializeField]
    public UltEvent OnEnter = new UltEvent();
    /*[SerializeField]
    public UltEvent OnUpdate = new UltEvent();*/
    [SerializeField]
    public UltEvent OnExit = new UltEvent();

    public void SetMachine(AIStateMachine machine)
    {
        stateMachine = machine;
        main = stateMachine.gameObject.transform;
    }

    protected void SelfEndState()
    {
        StartCoroutine(EndStateOneFrameLater());
    }

    public void StateEnter()
    {
        OnEnter.Invoke();
        OnStateEnter();
    }

    protected virtual void OnStateEnter() { }

    public void StateUpdate(float delta)
    {
        //OnUpdate.Invoke();
        
        if (_overrideTransformUpdate)
            UpdateTransform(delta);

        OnStateUpdate(delta);
    }

    protected virtual void UpdateTransform(float delta) { }

    protected virtual void OnStateUpdate(float delta) { }

    public void StateExit()
    {
        OnExit.Invoke();
        StopAllCoroutines();
        OnStateExit();
    }

    protected virtual void OnStateExit() { }

    IEnumerator EndStateOneFrameLater()
    {
        yield return new WaitForFixedUpdate();
        stateMachine.InterruptState();
    }
}
