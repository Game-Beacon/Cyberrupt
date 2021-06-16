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
    private float spawnPickUpChance = 0f;

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

    void SpawnPickUp(Enemy enemy)
    {
        spawnPickUpChance += enemy.addSpawnPickUpChance;

        while(spawnPickUpChance >= 100)
        {
            PickUpInstance emptyPickUp = Instantiate(pickUp, enemy.transform.position, Quaternion.identity);
            emptyPickUp.InjectData(pickables[Random.Range(0, pickables.Count)] as IPickable, playerMask, 10);
            spawnPickUpChance -= 100;
        }

        float rand = Random.Range(0f, 100f);

        if(rand < spawnPickUpChance)
        {
            PickUpInstance emptyPickUp = Instantiate(pickUp, enemy.transform.position, Quaternion.identity);
            emptyPickUp.InjectData(pickables[Random.Range(0, pickables.Count)] as IPickable, playerMask, 10);
            spawnPickUpChance = 0;
        }

        //TODO：根據玩家目前的狀態來調整生成掉落物機率的權重
        //比方說當玩家低血量時生成生命回復的機率會增加
    }
}
