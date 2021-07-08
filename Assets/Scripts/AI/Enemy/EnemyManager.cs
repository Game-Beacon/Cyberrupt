using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : GameBehaviour
{
    public static EnemyManager instance { get; private set; }

    private ScreenBound bound = null;
    private WarningUI warning = null;

    private bool startUp = false;
    private bool isFirstWave = true;
    private int waveIndex = 0;
    private EnemyWave[] waves;
    private List<Enemy> enemies = new List<Enemy>();
    public int enemyCount { get { return enemies.Count; } }

    [SerializeField]
    private GameObject enemySpawnZone;
    [SerializeField]
    private EnemyWaveGroup waveGroup;
    [SerializeField]
    private int skipWaveCount;
    [SerializeField]
    private float defaultWaveDelay;
    [SerializeField]
    private bool spawnSpecificWave = false;
    [SerializeField]
    private EnemyWave specificWave;

    private float _delayTimer = 0;
    public float delayTimer { get { return _delayTimer; } }

    public ObjectEvent<Enemy> OnEnemySpawned { get; } = new ObjectEvent<Enemy>();
    public ObjectEvent<Enemy> OnEnemyDied { get; } = new ObjectEvent<Enemy>();

    public IntEvent OnWaveAdvance { get; } = new IntEvent();

    public GameEvent OnEnterBossWave { get; } = new GameEvent();
    public ObjectEvent<Enemy> OnBossSpawn { get; } = new ObjectEvent<Enemy>();
    public GameEvent OnExitBossWave { get; } = new GameEvent();
    public GameEvent OnGameClear { get; } = new GameEvent();

    public override void GameAwake()
    {
        if (instance == null)
            instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }

        waveIndex = skipWaveCount;

        if (waveGroup != null)
            waves = waveGroup.GetWaves();
    }

    public override void GameStart()
    {
        bound = ScreenBound.instance;
        warning = DependencyContainer.GetDependency<WarningUI>() as WarningUI;
    }

    public override void GameUpdate()
    {
        //訂閱者要在GameStart才能安全獲取EnemyManager的instance（無法保證GameAwake的執行順序）
        //為了確保所有訂閱者都不會漏接事件，把事件最早發生的情形挪到第一個frame的update中
        //但是這感覺不是個好方法...
        if(!startUp)
        {
            MoveToNextWave();
            startUp = true;
        }

        if (_delayTimer > 0 && Input.GetKeyDown(KeyCode.F))
            _delayTimer = 0;
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

        OnEnemyDied.Invoke(enemy);

        if (enemyCount == 0)
            MoveToNextWave();
    }

    public void MoveToNextWave()
    {
        waveIndex++;

        if (waveIndex - 2 >= 0 && waves[waveIndex - 2].isBossWave)
            OnExitBossWave.Invoke();

        if (waveIndex > waves.Length)
        {
            OnGameClear.Invoke();
            return;
        }

        StartCoroutine(WaitAndSpawn());
    }

    IEnumerator WaitAndSpawn()
    {
        //生第一波怪的時候不應該出現延遲
        if(!isFirstWave)
        {
            _delayTimer = (waves[waveIndex - 2].haveWaveDelay) ? waves[waveIndex - 2].nextWaveDelay : defaultWaveDelay;

            while (_delayTimer > 0)
            {
                _delayTimer -= Time.deltaTime;
                yield return null;
            }
            _delayTimer = 0;
        }

        OnWaveAdvance.Invoke(waveIndex);
        isFirstWave = false;

#if UNITY_EDITOR
        if (spawnSpecificWave)
        {
            SpawnWave(specificWave);
            yield break;
        }
#endif
        if (waves[waveIndex - 1].isBossWave)
            OnEnterBossWave.Invoke();
        SpawnWave(waves[waveIndex - 1]);
    }

    private void SpawnWave(EnemyWave wave)
    {
        if(wave.isBossWave)
        {
            warning.OnWarningDone += () => SpawnBoss(wave);
            warning.StartWarning();
        }
        else
        {
            foreach (EnemySpawnData data in wave.spawns)
            {
                if (bound.InScreen(data.position))
                    Instantiate(enemySpawnZone, data.position, Quaternion.identity).GetComponent<EnemySpawnZone>().InjectData(data);
                else
                    Instantiate(data.enemy, data.position, Quaternion.identity);
            }
        }
    }

    private void SpawnBoss(EnemyWave wave)
    {
        //TODO: 目前這方法是假設在打BOSS時只有1隻BOSS，如果BOSS的形式是多隻的話會出事
        //或許有更好的解法?
        EnemySpawnData data = wave.spawns[0];

        if (bound.InScreen(data.position))
        {
            EnemySpawnZone zone = Instantiate(enemySpawnZone, data.position, Quaternion.identity).GetComponent<EnemySpawnZone>();
            zone.InjectData(data);
            zone.OnEnemySpawn += x => OnBossSpawn.Invoke(x);
        }
        else
        {
            Enemy boss = Instantiate(data.enemy, data.position, Quaternion.identity).GetComponent<Enemy>();
            OnBossSpawn.Invoke(boss);
        }
    }
}
