using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BossBaseData : ScriptableObject
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
}
