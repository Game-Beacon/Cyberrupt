using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossRand : StateMachineBehaviour
{
    [SerializeField]
    private int patternCount;

    [SerializeField]
    private float pickPatternTime = 0.5f;

    private int nextPattern;
    private float timer = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        nextPattern = Random.Range(0, patternCount);
        timer = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.fixedDeltaTime;

        if(timer >= pickPatternTime)
        {
            animator.SetTrigger("NEXT_STATE");
            animator.SetInteger("PATTERN_INDEX", nextPattern);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
