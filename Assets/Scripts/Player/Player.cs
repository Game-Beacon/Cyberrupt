using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UltEvents;

public class Player : GameBehaviour, IDanmakuTarget
{
    public Transform target { get { return transform; } }
    public float hitRadius { get { return _hitRadius; } }
    public bool isImmune { get { return isDashing.Value | isHurt | isInIFrame | isDead | isInvulnerable; } }
    public ReadOnlyReactiveProperty<bool> IsDashing { get; private set; }
    public Vector2 velocity => this.rb.velocity;

    [SerializeField]
    private Transform muzzle;

    // TODO: Might have to turn these things into a scriptable object
    /*[Space(20), SerializeField]
    private int _maxHp;*/
    [Space(20), SerializeField]
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

    [Space(20), SerializeField]
    private UltEvent OnDash_Self = new UltEvent();
    [SerializeField]
    private UltEvent OnHurt_Self = new UltEvent();
    [SerializeField]
    private UltEvent OnDied_Self = new UltEvent();

    private Rigidbody2D rb;
    private WeaponController _weaponController;
    private SkillController _skillController;
    private DanmakuManager danmakuManager;
    private Camera cam;

    /*public int maxHp { get { return _maxHp; } }*/
    public int hp { get { return _hp; } }
    public WeaponController weaponController { get { return _weaponController; } }
    public SkillController skillController { get { return _skillController; } }

    public GameEvent OnReceiveDamage { get; } = new GameEvent();
    public IntEvent OnHpChange { get; } = new IntEvent();
    public GameEvent OnDied { get; } = new GameEvent();

    private bool isHurt = false;
    private BoolReactiveProperty isDashing = new BoolReactiveProperty();
    private bool isInIFrame = false;
    private bool isDead = false;
    private bool isInvulnerable = false;
    private bool canDash = true;

    public override void GameAwake()
    {
        DependencyContainer.AddDependency(this);
        _weaponController = GetComponent<WeaponController>();
        _skillController = GetComponent<SkillController>();
        this.IsDashing = this.isDashing.ToReadOnlyReactiveProperty();
    }

    public override void GameStart()
    {
        danmakuManager = DanmakuManager.instance;
        danmakuManager.SetDanmakuTarget(this);
        danmakuManager.bulletHitTarget.AddListener(OnHit);
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    public void OnHit()
    {
        if (!isImmune)
        {
            _hp--;
            OnHurt_Self.Invoke();
            OnReceiveDamage.Invoke();
            OnHpChange.Invoke(_hp);
            if(_hp == 0)
            {
                isDead = true;
                update = false;
                OnDied_Self.Invoke();
                OnDied.Invoke();
            }
            StartCoroutine(AfterHurt(hurtTime));
        } 
    }

    public void AddHp(int count)
    {
        //_hp = (_hp + count > _maxHp) ? _maxHp : _hp + count;
        _hp += count;
        OnHpChange.Invoke(_hp);
    }

    public override void GameUpdate()
    {
        LookAtMouse();
        MoveAndDash();
        _weaponController.UpdateController();
        _skillController.UpdateController();
    }

    private void LookAtMouse()
    {
        Vector2 dir = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        muzzle.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void MoveAndDash()
    {
        if (isDashing.Value)
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
            OnDash_Self.Invoke();
            StartCoroutine(Dash(dir));
            return;
        }
        
        rb.velocity = dir.normalized * speed;
    }

    public void SetInvulnerability(bool value)
    {
        isInvulnerable = value;
    }

    IEnumerator Dash(Vector2 dir)
    {
        isDashing.Value = true;
        rb.velocity = dir.normalized * dashSpeed;
        yield return new WaitForSeconds(dashTime);
        isInIFrame = true;
        isDashing.Value = false;
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
        yield return new WaitForSeconds(hurtTime);
        isHurt = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}
