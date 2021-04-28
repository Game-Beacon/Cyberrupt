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

    [SerializeField]
    protected UltEvent OnSpawn = new UltEvent();
    [SerializeField]
    protected UltEvent OnHit = new UltEvent();
    [SerializeField]
    protected UltEvent OnDeath = new UltEvent();

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
        if (manager != null)
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
        if (manager != null)
            manager.RemoveEnemy(this);
        update = false;
        KillBehaviour(true);
    }
}
