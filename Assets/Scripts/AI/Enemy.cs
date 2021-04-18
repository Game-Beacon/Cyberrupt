using System;
using UnityEngine;
using UltEvents;

public class Enemy : GameBehaviour
{
    [SerializeField]
    protected float _maxHP;
    public float maxHP { get { return _maxHP; } }

    [SerializeField]
    protected float _hp;
    public float hp { get { return _hp; } }

    [SerializeField]
    protected UltEvent OnSpawn = new UltEvent();
    [SerializeField]
    protected UltEvent OnHit = new UltEvent();
    [SerializeField]
    protected UltEvent OnDeath = new UltEvent();

    public override sealed void GameAwake()
    {
        EnemyAwake();
        OnSpawn.Invoke();
    }

    protected virtual void EnemyAwake() { }

    public override sealed void GameStart()
    {
        EnemyStart();
    }

    protected virtual void EnemyStart() { }

    public override sealed void GameUpdate()
    {
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
}
