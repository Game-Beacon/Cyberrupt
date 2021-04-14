using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIStateMachine : GameBehaviour
{
    protected float _maxHp;
    public float maxHp { get { return _maxHp; } private set { } }

    protected float _hp;
    public float hp { get { return _hp; } private set { } }

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

    protected UnityEvent _EnterNewState = new UnityEvent();
    public UnityEvent EnterNewState { get { return _EnterNewState; } private set { } }
    protected UnityEvent _ExitState = new UnityEvent();
    public UnityEvent ExitState { get { return _ExitState; } private set { } }

    public sealed override void GameAwake()
    {
        foreach (AIState state in AIStates)
            state.SetMachine(this);
        MachineAwake();
    }

    protected virtual void MachineAwake() { }

    public sealed override void GameStart()
    {
        stateIndexes = new int[AIStates.Count];
        for (int i = 0; i < AIStates.Count; i++)
            stateIndexes[i] = i;
        RefreshStateIndexes();

        MachineStart();
    }

    protected virtual void MachineStart() { }

    public sealed override void GameFixedUpdate()
    {
        if (_currentState == null || !_currentState.overrideTransformUpdate)
            UpdateTransform();

        pickStateTimer -= Time.fixedDeltaTime;
        pickStateTimer = Mathf.Max(pickStateTimer, 0);

        if (pickStateTimer <= 0)
            UpdateStateMachine();
    }

    protected virtual void UpdateTransform() { }

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
}
