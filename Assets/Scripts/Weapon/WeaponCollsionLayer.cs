using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Collision Layer")]
public class WeaponCollsionLayer : ScriptableObject
{
    [SerializeField]
    private LayerMask _enemyMask;
    public LayerMask enemyMask { get { return _enemyMask; } }

    [SerializeField]
    private LayerMask _wallMask;
    public LayerMask wallMask { get { return _wallMask; } }
}
