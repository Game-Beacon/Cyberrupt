using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemySpawnType
{
    FixedPosition = 0,
    RandomPositionInScreen = 1
}

[System.Serializable]
public struct EnemySpawnData
{
    //What to spawn
    public GameObject enemy;
    //Where to spawn
    public Vector2 position;
}

[CreateAssetMenu(menuName = "Enemy/EnemyWave")]
public class EnemyWave : ScriptableObject
{
    [SerializeField]
    [HideInInspector]
    private List<EnemySpawnData> _spawns = new List<EnemySpawnData>();
    public List<EnemySpawnData> spawns { get { return _spawns; } private set { } }

    //The delay to start the new wave after the current wave is finished.
    [SerializeField]
    private int _nextWaveDelay;
    public int nextWaveDelay { get { return _nextWaveDelay; } private set { } }

    //An indicator about how difficult a wave is.
    [SerializeField]
    private int _difficulty;
    public int difficulty { get { return _difficulty; } private set { } }

#if UNITY_EDITOR

    public void AddSpawnData(EnemySpawnData data)
    {
        _spawns.Add(data);
    }

    /// <summary>
    /// 清空波數的資料。（請不要在程式運行中呼叫此程式，這是給編輯器用的。）
    /// </summary>
    public void ResetSpawnData()
    {
        _spawns.Clear();
    }
#endif
}
