using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyBaseProperty : ScriptableObject
{
    [SerializeField]
    private float _hp;
    public float hp { get { return _hp; } private set { } }
}
