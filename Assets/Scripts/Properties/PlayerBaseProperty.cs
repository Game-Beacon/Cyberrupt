using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerBaseProperty : ScriptableObject
{
    //生命值
    [SerializeField]
    private int _hp;
    public int hp { get { return _hp; } private set { } }

    //攻擊力
    [SerializeField]
    private float _atk;
    public float atk { get { return _atk; } private set { } }

    //離散度
    [SerializeField]
    private float _spread;
    public float spread { get { return _spread; } private set { } }

    //每秒射幾次
    [SerializeField]
    private float _sps;
    public float sps { get { return _sps; } private set { } }

    //每次射擊射幾倍數量的子彈
    [SerializeField]
    private int _cps;
    public int cps { get { return _cps; } private set { } }
}

