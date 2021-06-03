using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : GameBehaviour
{
    public static EnemyManager instance { get; private set; }

    private List<Enemy> enemies = new List<Enemy>();

    private EnemyWave[] enemyWaves;
    private EnemyWave[] bossWaves;
    private int wave = 0;
    public int enemyCount { get { return enemies.Count; } }

    /*[SerializeField]
    private Bound _screen;

    public float GetWorldScreenMinX { get { return _screen.parent.position.x - (_screen.xSize / 2); } }
    public float GetWorldScreenMaxX { get { return _screen.parent.position.x + (_screen.xSize / 2); } }
    public float GetWorldScreenMinY { get { return _screen.parent.position.y - (_screen.ySize / 2); } }
    public float GetWorldScreenMaxY { get { return _screen.parent.position.y + (_screen.ySize / 2); } }*/

    public GameObjectEvent OnEnemyDied { get; } = new GameObjectEvent();
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

        Object[] obj = Resources.LoadAll("AI/Waves", typeof(EnemyWave));
        enemyWaves = new EnemyWave[obj.Length];

        for (int i = 0; i < obj.Length; i++)
            enemyWaves[i] = (EnemyWave)obj[i];

        obj = Resources.LoadAll("AI/BossWaves", typeof(EnemyWave));
        bossWaves = new EnemyWave[obj.Length];

        for (int i = 0; i < obj.Length; i++)
            bossWaves[i] = (EnemyWave)obj[i];
    }

    public override void GameUpdate()
    {
        //訂閱者要在GameStart才能安全獲取EnemyManager的instance（無法保證GameAwake的執行順序）
        //為了確保所有訂閱者都不會漏接事件，把事件最早發生的情形挪到第一個frame的update中
        //但是這感覺不是個好方法...
        SpawnWave();
        update = false;
    }

    public void AddEnemy(Enemy enemy)
    {
        if(!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
        if (enemyCount == 0)
            SpawnWave();

        OnEnemyDied.Invoke(enemy.gameObject);
    }

    public void SpawnWave()
    {
        wave++;

        OnWaveAdvance.Invoke(wave);

        //Debug.Log("Spawn");
        if(wave % 10 == 0)
        {
            int index = Random.Range(0, bossWaves.Length);

            //TODO: 目前這方法是假設在打BOSS時只有1隻BOSS，如果BOSS的形式是多隻的話會出事
            //或許有更好的解法?
            EnemySpawnData data = bossWaves[index].spawns[0];
            Enemy boss = Instantiate(data.enemy, data.position, Quaternion.identity).GetComponent<Enemy>();
            OnBossSpawn.Invoke(boss);
        }
        else
        {
            int index = Random.Range(0, enemyWaves.Length);

            foreach (EnemySpawnData data in enemyWaves[index].spawns)
                Instantiate(data.enemy, data.position, Quaternion.identity);
        }
    }

    /*public bool InScreen(Vector2 position)
    {
        return !_screen.OverBound(position);
    }

    public Vector2 GetRandomPointInScreen(float edge)
    {
        float maxX = _screen.parent.position.x + (_screen.xSize / 2) - edge;
        float minX = _screen.parent.position.x - (_screen.xSize / 2) + edge;
        float maxY = _screen.parent.position.y + (_screen.ySize / 2) - edge;
        float minY = _screen.parent.position.y - (_screen.ySize / 2) + edge;

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }*/

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_screen.xSize, _screen.ySize));
    }*/
}
