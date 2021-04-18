using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    //武器名稱
    [SerializeField]
    private string _weaponName;
    public string weaponName { get { return _weaponName; } private set { } }

    [SerializeField]
    private GameObject _bullet;
    public GameObject bullet { get { return _bullet; } private set { } }

    //傷害
    [SerializeField]
    private float _damage;
    public float damage { get { return _damage; } private set { } }

    //散射度
    [SerializeField]
    private float _spreadAngle;
    public float spreadAngle { get { return _spreadAngle; } private set { } }

    //射速
    [SerializeField]
    private float _speed;
    public float speed { get { return _speed; } private set { } }

    //頻率
    [SerializeField]
    private float _frequency;
    public float frequency { get { return _frequency; } private set { } }

    //每發射擊的子彈數量
    [SerializeField]
    private int _cps;
    public int cps { get { return _cps; } private set { } }
}
