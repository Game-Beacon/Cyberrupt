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
    private UltEvent OnSpawn = new UltEvent();

    private List<GameObject> missiles = new List<GameObject>();
    private int missileCounter = 0;

    protected override void OnStateEnter()
    {
        base.OnStateEnter();

        missileCounter = 0;
        StartCoroutine(SpawnMissiles());
    }

    protected override void OnStateUpdate(float delta)
    {
        base.OnStateUpdate(delta);

        for(int i = missiles.Count - 1; i >= 0; i--)
        {
            if (missiles[i] == null)
                missiles.RemoveAt(i);
        }

        if (missiles.Count == 0)
            SelfEndState();
    }

    IEnumerator SpawnMissiles()
    {
        missileCounter = spawnCount;

        for (int i = 0; i < spawnCount; i ++)
        {
            GameObject go = Instantiate(missile, muzzle.position, Quaternion.identity);
            missiles.Add(go);
            OnSpawn.Invoke();
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
