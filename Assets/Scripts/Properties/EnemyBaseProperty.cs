using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyBaseProperty")]
public class EnemyBaseProperty : ScriptableObject
{
    //生命值
    [SerializeField]
    private float _hp;
    public float hp { get { return _hp; } private set { } }

    //擊殺所獲得的分數
    [SerializeField]
    private int _score;
    public int score { get { return _score; } private set { } }

    //擊殺所帶來的掉寶率增加值(Percent)
    [SerializeField,Tooltip("擊殺所帶來的掉寶率增加值(%)")]
    private float _addSpawnPickUpChance;
    public float addSpawnPickUpChance { get { return _addSpawnPickUpChance; } private set { } }
}
