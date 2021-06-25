using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class PickUpManager : GameBehaviour
{
    public static PickUpManager instance { get; private set; }

#if UNITY_EDITOR
    [SerializeField]
    private bool alwaysSpawn = false;
#endif

    [SerializeField]
    private PickUpInstance pickUp;
    private LayerMask playerMask;
    private List<ScriptableObject> pickables = new List<ScriptableObject>();
    private float spawnPickUpChance = 0f;

    public override void GameAwake()
    {
        if(instance == null)
            instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }

        playerMask = CollisionLayer.instance.playerMask;

        Object[] objects = Resources.LoadAll("PickUp", typeof(ScriptableObject));
        foreach(Object obj in objects)
            pickables.Add(obj as ScriptableObject);
    }

    public override void GameStart()
    {
        EnemyManager.instance.OnEnemyDied.AddListener(SpawnPickUp);
    }

    public void Spawn(Transform target)
    {
        PickUpInstance emptyPickUp = Instantiate(pickUp, target.position, Quaternion.identity);
        emptyPickUp.InjectData(pickables[Random.Range(0, pickables.Count)] as IPickable, playerMask, 10);
        emptyPickUp.transform
            .DOMove(emptyPickUp.transform.position + (Vector3)Random.insideUnitCircle, 3)
            .SetEase(Ease.OutCubic);
    }

    void SpawnPickUp(Enemy enemy)
    {
#if UNITY_EDITOR
        if (alwaysSpawn)
        {
            Spawn(enemy.transform);
            return;
        }
#endif

        if (enemy.addSpawnPickUpChance == 0)
            return;

        spawnPickUpChance += enemy.addSpawnPickUpChance;

        while(spawnPickUpChance >= 100)
        {
            this.Spawn(enemy.transform);
            spawnPickUpChance -= 100;
        }

        float rand = Random.Range(0f, 100f);

        if(rand < spawnPickUpChance)
        {
            this.Spawn(enemy.transform);
            spawnPickUpChance = 0;
        }

        //TODO：根據玩家目前的狀態來調整生成掉落物機率的權重
        //比方說當玩家低血量時生成生命回復的機率會增加
    }
}