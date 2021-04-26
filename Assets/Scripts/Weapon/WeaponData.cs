using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponShootType
{
    SemiAuto = 0,
    Auto = 1,
    Charge = 2
}

[CreateAssetMenu(menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    //武器名稱
    [SerializeField]
    private string _weaponName;
    public string weaponName { get { return _weaponName; } private set { } }

    [SerializeField]
    private WeaponBullet _bullet;
    public WeaponBullet bullet { get { return _bullet; } private set { } }

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

    //射速偏差
    [SerializeField, Range(0, 1)]
    private float _speedRand;
    public float speedRand { get { return _speedRand; } private set { } }

    //頻率(或是充能型武器的射擊延遲)
    [SerializeField]
    private float _frequency;
    public float frequency { get { return _frequency; } private set { } }

    //每發射擊的子彈數量
    [SerializeField]
    private int _cps;
    public int cps { get { return _cps; } private set { } }

    //武器的射擊模式
    [SerializeField]
    private WeaponShootType _shootType;
    public WeaponShootType shootType { get { return _shootType; } }

    [SerializeField]
    private float _chargeTime;
    public float chargeTime { get { return _chargeTime; } }

    /* Something that could (should) be added in the future
     * 1. 射擊時的音效
     * 2. 充能時的音效
     * 3. 充能時的特效(某種遊戲物件?)
     */
}
