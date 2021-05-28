using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public class HeartBleed_HeartBeat : AIState
{
    private EnemyGroup enemyGroup = null;

    [Space(20)]
    private Transform target = null;
    [SerializeField]
    private GameObject heartBeat;
    [SerializeField]
    private int beatCount;
    [SerializeField]
    private float beatDelay;
    [SerializeField]
    private UltEvent OnHeartBeat = new UltEvent();

    [Space(20), SerializeField]
    private float fadeTime;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        if (enemyGroup == null)
            enemyGroup = GetComponent<EnemyGroup>();

        ITarget enemyTarget;
        if (target == null && TryGetComponent(out enemyTarget))
            target = enemyTarget.target;

        StartCoroutine(StartHeartBeat());
    }

    IEnumerator StartHeartBeat()
    {
        enemyGroup.enableColliders = false;
        DOTween.To(() => enemyGroup.alpha, x => enemyGroup.alpha = x, 0, fadeTime);
        yield return new WaitForSeconds(fadeTime);

        Vector2 position = Vector2.zero;

        for(int i = 0; i < beatCount; i++)
        {
            position = target.position;
            Instantiate(heartBeat, position, Quaternion.identity);
            yield return new WaitForSeconds(beatDelay);
        }

        transform.position = position;

        DOTween.To(() => enemyGroup.alpha, x => enemyGroup.alpha = x, 1, fadeTime);
        enemyGroup.enableColliders = true;
        yield return new WaitForSeconds(fadeTime);
        SelfEndState();
    }
}
