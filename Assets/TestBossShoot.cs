using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossShoot : StateMachineBehaviour
{
    private TestBoss boss = null;

    [SerializeField]
    private List<int> callPattern = new List<int>();
    [SerializeField]
    private float shootTime;

    private List<DanmakuParticleEmitter> emitters = new List<DanmakuParticleEmitter>();
    private float timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        if(boss == null)
            animator.gameObject.TryGetComponent(out boss);
        
        for(int i = 0; i < callPattern.Count; i ++)
            emitters.Add(boss.AddDanmaku(callPattern[i]));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.fixedDeltaTime;

        if (timer >= shootTime)
            animator.SetTrigger("STATE_FIN");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(DanmakuParticleEmitter emitter in emitters)
            emitter.spawnBullets = false;

        emitters.Clear();
    }
}
