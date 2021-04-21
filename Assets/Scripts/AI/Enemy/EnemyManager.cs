using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : GameBehaviour
{
    private static EnemyManager _instance;
    public static EnemyManager instance { get { return _instance; } }

    private List<Enemy> enemies = new List<Enemy>();

    public override void GameAwake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }
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
    }
}
