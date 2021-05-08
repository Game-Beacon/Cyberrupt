﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameBehaviour, IDanmakuTarget
{
    public Transform target { get { return transform; } }
    public float hitRadius { get { return _hitRadius; } }
    public bool isImmune { get { return isDashing | isHurt | isInIFrame | isDead; } }

    //TODO: Might have to turn these things into a scriptable object
    [SerializeField]
    private int _maxHp;
    [SerializeField]
    private int _hp;

    [Space(20), SerializeField]
    private float speed;
    [SerializeField]
    private float _hitRadius;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float iTimeAfterDash;
    [SerializeField]
    private float dashCoolDown;
    [SerializeField]
    private float hurtTime;

    private Rigidbody2D rb;
    private WeaponController _weaponController;
    private DanmakuManager danmakuManager;
    private Camera cam;

    public int maxHp { get { return _maxHp; } }
    public int hp { get { return _hp; } }
    public WeaponController weaponController { get { return _weaponController; } }
    public IntEvent OnHpChange { get; } = new IntEvent();
    public GameEvent OnDied { get; } = new GameEvent();

    private bool isHurt = false;
    private bool isDashing = false;
    private bool isInIFrame = false;
    private bool isDead = false;
    private bool canDash = true;

    public override void GameAwake()
    {
        DependencyContainer.AddDependency(this);
        _weaponController = GetComponent<WeaponController>();
    }

    public override void GameStart()
    {
        danmakuManager = DanmakuManager.instance;
        danmakuManager.SetDanmakuTarget(this);
        danmakuManager.bulletHitTarget.AddListener(OnHit);
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void OnHit()
    {
        if (!isImmune)
        {
            _hp--;
            OnHpChange.Invoke(_hp);
            if(_hp == 0)
            {
                isDead = true;
                update = false;
                OnDied.Invoke();
            }
            StartCoroutine(AfterHurt(hurtTime));
        } 
    }

    public override void GameUpdate()
    {
        LookAtMouse();
        MoveAndDash();
        _weaponController.UpdateController();
    }

    private void LookAtMouse()
    {
        Vector2 dir = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveAndDash()
    {
        if (isDashing)
            return;

        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            dir += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            dir += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            dir += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            dir += Vector2.right;

        if (canDash && Input.GetKeyDown(KeyCode.Space) && dir.magnitude > 0)
        {
            StartCoroutine(Dash(dir));
            return;
        }

        
        rb.velocity = dir.normalized * speed;
    }

    IEnumerator Dash(Vector2 dir)
    {
        isDashing = true;
        rb.velocity = dir.normalized * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        isInIFrame = true;
        isDashing = false;
        StartCoroutine(DashCoolDown(dashCoolDown));
        yield return new WaitForSeconds(iTimeAfterDash);
        isInIFrame = false;
    }

    IEnumerator DashCoolDown(float coolDown)
    {
        canDash = false;
        yield return new WaitForSeconds(coolDown);
        canDash = true;
    }

    IEnumerator AfterHurt(float hurtTime)
    {
        isHurt = true;
        //Debug.Log("start hurt");
        yield return new WaitForSeconds(hurtTime);
        //Debug.Log("end hurt");
        isHurt = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
