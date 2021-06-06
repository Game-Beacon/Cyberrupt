using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIStateMachine : GameBehaviour
{
    protected Enemy enemy;
    protected Transform _target;
    public Transform target { get { return _target; } private set { } }

    [SerializeField]
    protected List<AIState> AIStates = new List<AIState>();    
    private int index = 0;
    private int lastStateIndex = -1;
    private AIState _currentState = null;
    public AIState currentState { get { return _currentState; } private set { } }
    protected int[] stateIndexes;

    [SerializeField]
    protected float pickStateTime;
    protected float pickStateTimer = 0;

    protected bool interruptState = false;

    protected UnityEvent _OnStart = new UnityEvent();
    public UnityEvent OnStart { get { return _OnStart; } private set { } }
    protected UnityEvent _OnUpdateTransform = new UnityEvent();
    public UnityEvent OnUpdateTransform { get { return _OnUpdateTransform; } private set { } }
    protected UnityEvent _EnterNewState = new UnityEvent();
    public UnityEvent EnterNewState { get { return _EnterNewState; } private set { } }
    protected UnityEvent _ExitState = new UnityEvent();
    public UnityEvent ExitState { get { return _ExitState; } private set { } }

    public sealed override void GameAwake()
    {
        enemy = GetComponent<Enemy>();
        pickStateTimer = pickStateTime;
        foreach (AIState state in AIStates)
            state.SetMachine(this);
    }

    public sealed override void GameStart()
    {
        stateIndexes = new int[AIStates.Count];
        for (int i = 0; i < AIStates.Count; i++)
            stateIndexes[i] = i;
        RefreshStateIndexes();

        _OnStart.Invoke();
    }

    public sealed override void GameFixedUpdate()
    {
        if (_currentState == null || !_currentState.overrideTransformUpdate)
            _OnUpdateTransform.Invoke();

        if(enemy.canAttack)
        {
            pickStateTimer -= Time.fixedDeltaTime;
            pickStateTimer = Mathf.Max(pickStateTimer, 0);

            if (AIStates.Count > 0 && pickStateTimer <= 0)
                UpdateStateMachine();
        }
    }

    private void UpdateStateMachine()
    {
        if (_currentState == null && AIStates.Count > 0)
        {
            _currentState = AIStates[stateIndexes[index]];
            _EnterNewState.Invoke();
            _currentState.StateEnter();
            index++;
            if(index == AIStates.Count)
            {
                RefreshStateIndexes();
                index = 0;
            }
        }
        if (_currentState != null)
            _currentState.StateUpdate(Time.fixedDeltaTime);
        if (interruptState)
        {
            _currentState.StateExit();
            _currentState = null;
            _ExitState.Invoke();
            pickStateTimer = pickStateTime;
            interruptState = false;
        }
    }

    private void RefreshStateIndexes()
    {
        if (AIStates.Count == 0)
            return;

        for (int i = 0; i < AIStates.Count; i++)
        {
            int rand = Random.Range(0, AIStates.Count);
            int temp = stateIndexes[i];
            stateIndexes[i] = stateIndexes[rand];
            stateIndexes[rand] = temp;
        }

        if (stateIndexes[0] == lastStateIndex)
            System.Array.Reverse(stateIndexes);

        lastStateIndex = stateIndexes[AIStates.Count - 1];
    }

    public void InterruptState()
    {
        interruptState = true;
    }

    public void SetPickStateTimeRand(float randomness)
    {
        pickStateTime *= Random.Range(1 - 0.5f * randomness, 1 + 0.5f * randomness);
    }
}
