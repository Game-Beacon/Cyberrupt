using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : GameBehaviour
{
    public static EnemyManager instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    [SerializeField]
    private EnemyWaveGroup waveGroup;
    [SerializeField]
    private bool spawnSpecificWave = false;
    [SerializeField]
    private EnemyWave specificWave;

    private int waveIndex = 0;
    public int enemyCount { get { return enemies.Count; } }
    private EnemyWave[] waves;

    public ObjectEvent<Enemy> OnEnemySpawned { get; } = new ObjectEvent<Enemy>();
    public ObjectEvent<Enemy> OnEnemyDied { get; } = new ObjectEvent<Enemy>();
    public IntEvent OnWaveAdvance { get; } = new IntEvent();
    public ObjectEvent<Enemy> OnBossSpawn { get; } = new ObjectEvent<Enemy>();

    public override void GameAwake()
    {
        if (instance == null)
            instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }

        if (waveGroup != null)
            waves = waveGroup.GetWaves();
    }

    public override void GameUpdate()
    {
        //訂閱者要在GameStart才能安全獲取EnemyManager的instance（無法保證GameAwake的執行順序）
        //為了確保所有訂閱者都不會漏接事件，把事件最早發生的情形挪到第一個frame的update中
        //但是這感覺不是個好方法...
        MoveToNextWave();
        update = false;
    }

    public void AddEnemy(Enemy enemy)
    {
        if(!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            OnEnemySpawned.Invoke(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
        if (enemyCount == 0)
            MoveToNextWave();

        OnEnemyDied.Invoke(enemy);
    }

    public void MoveToNextWave()
    {
        waveIndex++;

        OnWaveAdvance.Invoke(waveIndex);

#if UNITY_EDITOR
        if(spawnSpecificWave)
        {
            SpawnWave(specificWave);
            return;
        }
#endif
        if (waveIndex <= waves.Length)
            SpawnWave(waves[waveIndex - 1]);
    }

    private void SpawnWave(EnemyWave wave)
    {
        if(wave.isBossWave)
        {
            //TODO: 目前這方法是假設在打BOSS時只有1隻BOSS，如果BOSS的形式是多隻的話會出事
            //或許有更好的解法?
            EnemySpawnData data = wave.spawns[0];
            Enemy boss = Instantiate(data.enemy, data.position, Quaternion.identity).GetComponent<Enemy>();
            OnBossSpawn.Invoke(boss);
        }
        else
        {
            foreach (EnemySpawnData data in wave.spawns)
                Instantiate(data.enemy, data.position, Quaternion.identity);
        }
    }
}
