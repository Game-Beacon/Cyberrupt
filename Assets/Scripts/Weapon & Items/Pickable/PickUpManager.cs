using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : GameBehaviour
{
    public static PickUpManager instance { get; private set; }
    [SerializeField]
    private PickUpInstance pickUp;
    [SerializeField]
    private LayerMask playerMask;
    private List<ScriptableObject> pickables = new List<ScriptableObject>();
    private int killCount = 0;


    public override void GameAwake()
    {
        if (instance == null)
            instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }

        Object[] objects = Resources.LoadAll("PickUp", typeof(ScriptableObject));
        foreach (Object obj in objects)
            pickables.Add(obj as ScriptableObject);
    }

    public override void GameStart()
    {
        EnemyManager.instance.OnEnemyDied.AddListener(SpawnPickUp);
    }

    void SpawnPickUp(GameObject spawner)
    {
        killCount++;

        float rand = Random.Range(0f, 1f);
        if(rand < killCount / 100f)
        {
            PickUpInstance emptyPickUp = Instantiate(pickUp, spawner.transform.position, Quaternion.identity);
            emptyPickUp.InjectData(pickables[Random.Range(0, pickables.Count)] as IPickable, playerMask, 10);
            killCount = 0;
        }
    }
}
