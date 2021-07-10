using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class WeaponBullet : GameBehaviour
{
    [SerializeField]
    protected CollisionLayer collisionLayer = null;
    [SerializeField]
    protected ParticleSystem deathParticle = null;
    [SerializeField]
    protected float _scanRaycastRadius;

    public float scanRaycastRadius { get { return _scanRaycastRadius; } }

    protected Rigidbody2D _rb;
    public Rigidbody2D rb { get { return _rb; } protected set { } }

    protected float damage;
    protected float speed;
    protected Vector2 direction;
    protected bool canKillBullet = false;

    protected BulletModifier[] modifiers;

    public override void GameAwake()
    {
        if (collisionLayer == null)
            collisionLayer = CollisionLayer.instance;

        if (!TryGetComponent(out _rb))
            Debug.LogError("There's no rigidbody2D attached to " + gameObject.name);

        modifiers = GetComponents<BulletModifier>();
        if (modifiers == null)
            modifiers = new BulletModifier[0];
    }

    public void SetBulletProperty(float Damage, float Speed, Vector2 Direction)
    {
        damage = Damage;
        speed = Speed;
        direction = Direction;
    }

    public override void GameFixedUpdate()
    {
        RaycastHit2D raycast;
        
        raycast = (_scanRaycastRadius > 0)? 
        Physics2D.CircleCast(transform.position, _scanRaycastRadius, rb.velocity, rb.velocity.magnitude * Time.fixedDeltaTime, collisionLayer.screenMask):
        Physics2D.Raycast(transform.position, rb.velocity, rb.velocity.magnitude * Time.fixedDeltaTime, collisionLayer.screenMask);
        if (raycast.collider != null)
            OnHitWall(raycast);

        raycast = (_scanRaycastRadius > 0) ?
        Physics2D.CircleCast(transform.position, _scanRaycastRadius, rb.velocity, rb.velocity.magnitude * Time.fixedDeltaTime, collisionLayer.enemyMask) :
        Physics2D.Raycast(transform.position, rb.velocity, rb.velocity.magnitude * Time.fixedDeltaTime, collisionLayer.enemyMask);
        if (raycast.collider != null)
            OnHitEnemy(raycast.collider);
    }

    protected virtual void OnHitEnemy(Collider2D collision)
    {
        GameObject hitObject = collision.gameObject;

        Enemy enemy = hitObject.GetComponent<Enemy>();
        if (enemy == null)
            enemy = hitObject.GetComponentInParent<Enemy>();
        enemy.ApplyDamage(damage);

        foreach (BulletModifier modifier in modifiers)
            modifier.OnHitEnemy(this, collision);

        if (canKillBullet || modifiers.Length == 0)
            KillBehaviour(true);
    }

    protected virtual void OnHitWall(RaycastHit2D raycast)
    {
        transform.position = raycast.point;

        foreach (BulletModifier modifier in modifiers)
            modifier.OnHitWall(this, raycast);

        if (canKillBullet || modifiers.Length == 0)
            KillBehaviour(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CollisionLayer.CollideWithMask(collision.gameObject, collisionLayer.enemyMask))
            OnHitEnemy(collision);
        /*if (CollisionLayer.CollideWithMask(collision.gameObject, collisionLayer.screenMask))
            OnHitWall(collision);*/
    }

    public void SetKillBullet(bool value)
    {
        canKillBullet |= value;
    }

    public override void OnKilled()
    {
        RaycastHit2D raycast = Physics2D.Raycast(transform.position, rb.velocity, rb.velocity.magnitude * Time.fixedDeltaTime, collisionLayer.screenMask);
        if (raycast.collider != null)
            transform.position = raycast.point;

        if (deathParticle != null)
            Instantiate(deathParticle, transform.position, Quaternion.identity);
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionLayer.CollideWithMask(collision.collider.gameObject, collisionLayer.enemyMask))
            OnHitEnemy(collision);
        if (CollisionLayer.CollideWithMask(collision.collider.gameObject, collisionLayer.screenMask))
            OnHitWall(collision);
    }*/

    //TODO (for this class)：
    //Build something that acts like a constructor.
    //Inject initial value into this class. (speed, direction, damage... etc)
    //Add a OnHit event (when the bullet hit enemy or something else.)
    //As for the updating logic (Ex: how the bullet move), it's those who inherit this class have to deal with.

    //TODO (for weapon system):
    //Create another class that handle firing bullets (and dependency injection) and switch between weapons.

    //TODO (not sure):
    //Maybe we have to add a system that can modify the "updating" logic 
    //(instead of just value injection on initialization) in the bullets? (Something like nova drift's homing system.)
}
