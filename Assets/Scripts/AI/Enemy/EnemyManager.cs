using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : GameBehaviour
{
    private static EnemyManager _instance;
    public static EnemyManager instance { get { return _instance; } }

    private List<Enemy> enemies = new List<Enemy>();

    private EnemyWave[] enemyWaves;
    private EnemyWave[] bossWaves;
    private int wave = 0;
    public int enemyCount { get { return enemies.Count; } }

    [SerializeField]
    private Bound _screen;

    public override void GameAwake()
    {
        if (_instance == null)
            _instance = this;
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

        SpawnWave();
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
    }

    public void SpawnWave()
    {
        wave++;

        //Debug.Log("Spawn");
        if(wave % 4 == 0)
        {
            int index = Random.Range(0, bossWaves.Length);

            foreach (EnemySpawnData data in bossWaves[index].spawns)
                Instantiate(data.enemy, data.position, Quaternion.identity);
        }
        else
        {
            int index = Random.Range(0, enemyWaves.Length);

            foreach (EnemySpawnData data in enemyWaves[index].spawns)
                Instantiate(data.enemy, data.position, Quaternion.identity);
        }
    }

    public bool InScreen(Vector2 position)
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_screen.xSize, _screen.ySize));
    }
}
