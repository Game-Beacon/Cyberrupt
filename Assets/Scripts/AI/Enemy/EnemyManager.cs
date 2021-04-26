using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : GameBehaviour
{
    private static EnemyManager _instance;
    public static EnemyManager instance { get { return _instance; } }

    private HashSet<Enemy> enemies = new HashSet<Enemy>();

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
