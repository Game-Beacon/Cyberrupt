using System;
using UnityEngine;
using UltEvents;

public class Enemy : GameBehaviour
{
    protected static EnemyManager manager = null;
    protected static ScreenBound screen = null;

    [SerializeField]
    protected EnemyBaseProperty _property;
    public EnemyBaseProperty property { get { return _property; } }

    [SerializeField]
    protected string _name;
    public string Name { get { return _name; } }

    [SerializeField]
    protected float _maxHP;
    public float maxHP { get { return _maxHP; } }

    [SerializeField]
    protected float _hp;
    public float hp { get { return _hp; } }

    //這個怪物是其他怪召喚出來的嗎？（比方說砲台的盾，以及生砲灰的生出的砲灰）
    /*[SerializeField]
    protected bool _isSideProduction = false;
    public bool isSideProduction { get { return _isSideProduction; } }*/

    [SerializeField]
    public UltEvent OnSpawn = new UltEvent();
    [SerializeField]
    public UltEvent OnHit = new UltEvent();
    [SerializeField]
    public UltEvent OnDeath = new UltEvent();

    public AsyncEvent Death { get; } = new AsyncEvent();

    [HideInInspector]
    public FloatEvent OnReceiveDamage { get; } = new FloatEvent();

    private bool dead = false;
    private bool dieInvoke = false;
    protected bool _canAttack = false;
    protected bool _inScreen = false;
    public bool canAttack { get { return _canAttack; } }

    public override sealed void GameAwake()
    {
        OnDeath += Die;
        Death.OnEventComplete.AddListener(() => OnDeath.Invoke());

        _maxHP = _property.hp;
        _hp = _property.hp;

        EnemyAwake();
    }

    protected virtual void EnemyAwake() { }

    public override sealed void GameStart()
    {
        if (manager == null)
            manager = EnemyManager.instance;
        if (manager != null /*&& !_isSideProduction*/)
            manager.AddEnemy(this);
        if (screen == null)
            screen = ScreenBound.instance;
        OnSpawn.Invoke();
        EnemyStart();
    }

    protected virtual void EnemyStart() { }

    public override sealed void GameUpdate()
    {
        _inScreen = (manager == null) ? true : screen.InScreen(transform.position);
        if (_canAttack == false)
            _canAttack = _inScreen;
        EnemyUpdate();
        if (_hp <= 0 && !dead)
        {
            dead = true;
            Death.Invoke(this);
        }   
    }

    protected virtual void EnemyUpdate() { }

    public void ApplyDamage(float damage)
    {
        float before = _hp;
        _hp = Mathf.Max(_hp - damage, 0);
        OnReceiveDamage.Invoke(before - _hp);
        OnHit.Invoke();
    }

    public void SetHP(float max)
    {
        _maxHP = max;
        _hp = _maxHP;
    }

    public void OverrideProperty(EnemyBaseProperty newProperty)
    {
        _property = newProperty;
    }

    public void Die()
    {
        //確保此函式不會執行超過兩次以上
        if (dieInvoke)
            return;
        dieInvoke = true;

        if (manager != null /*&& !_isSideProduction*/)
            manager.RemoveEnemy(this);
        update = false;
        KillBehaviour(true);
    }
}
