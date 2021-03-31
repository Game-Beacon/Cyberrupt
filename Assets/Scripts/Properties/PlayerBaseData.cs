using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerBaseData : ScriptableObject
{
    [SerializeField]
    private float _hp;
    public float hp { get { return _hp; } private set { } }

    [SerializeField]
    private float _atk;
    public float atk { get { return _atk; } private set { } }

    [SerializeField]
    private float _def;
    public float def { get { return _def; } private set { } }

    [SerializeField]
    private float _sht;
    public float sht { get { return _sht; } private set { } }

    [SerializeField]
    private float _sps;
    public float sps { get { return _sps; } private set { } }

    [SerializeField]
    private float _eng;
    public float eng { get { return _eng; } private set { } }
}

