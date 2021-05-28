using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class HeartBleed_Missile : AIState
{
    [Space(20), SerializeField]
    private Transform muzzle;
    [SerializeField]
    private GameObject missile;
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float spawnDelay;
    [SerializeField]
    private float recover;
    [SerializeField]
    private UltEvent OnSpawn = new UltEvent();

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        StartCoroutine(SpawnMissiles());
    }

    IEnumerator SpawnMissiles()
    {
        for(int i = 0; i < spawnCount; i ++)
        {
            Instantiate(missile, muzzle.position, Quaternion.identity);
            OnSpawn.Invoke();
            yield return new WaitForSeconds(spawnDelay);
        }

        yield return new WaitForSeconds(recover);

        SelfEndState();
    }
}
