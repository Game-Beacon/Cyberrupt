using System;
using UnityEngine;
using UltEvents;

public class Enemy : GameBehaviour
{
    protected static EnemyManager manager = null;

    [SerializeField]
    protected float _maxHP;
    public float maxHP { get { return _maxHP; } }

    [SerializeField]
    protected float _hp;
    public float hp { get { return _hp; } }

    //這個怪物是其他怪召喚出來的嗎？（比方說砲台的盾，以及生砲灰的生出的砲灰）
    [SerializeField]
    protected bool _isSideProduction = false;
    public bool isSideProduction { get { return _isSideProduction; } }

    [SerializeField]
    public UltEvent OnSpawn = new UltEvent();
    [SerializeField]
    public UltEvent OnHit = new UltEvent();
    [SerializeField]
    public UltEvent OnDeath = new UltEvent();

    protected bool _canAttack = true;
    public bool canAttack { get { return _canAttack; } }

    public override sealed void GameAwake()
    {
        OnDeath += Die;

        EnemyAwake();
        OnSpawn.Invoke();
    }

    protected virtual void EnemyAwake() { }

    public override sealed void GameStart()
    {
        if (manager == null)
            manager = EnemyManager.instance;
        if (manager != null && !_isSideProduction)
            manager.AddEnemy(this);
        EnemyStart();
    }

    protected virtual void EnemyStart() { }

    public override sealed void GameUpdate()
    {
        _canAttack = (manager == null)? true : manager.InScreen(transform.position);
        EnemyUpdate();
        if (_hp <= 0)
            OnDeath.Invoke();
    }

    protected virtual void EnemyUpdate() { }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit.Invoke();
    }

    public void ApplyDamage(float damage)
    {
        _hp -= damage;
    }

    public void SetHP(float max)
    {
        _maxHP = max;
        _hp = _maxHP;
    }

    public void Die()
    {
        if (manager != null && !_isSideProduction)
            manager.RemoveEnemy(this);
        update = false;
        KillBehaviour(true);
    }

    //設定此怪物是否為其他怪物的副產物
    public void SetSideProduction(bool value)
    {
        _isSideProduction = value;
    }
}
