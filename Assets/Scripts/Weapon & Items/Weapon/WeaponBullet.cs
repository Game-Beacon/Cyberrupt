using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class WeaponBullet : GameBehaviour
{
    [SerializeField]
    protected WeaponCollsionLayer collisionLayer = null;

    protected Rigidbody2D rb;

    protected float damage;
    protected float speed;
    protected Vector2 direction;

    public override void GameAwake()
    {
        if (collisionLayer == null)
            collisionLayer = Resources.FindObjectsOfTypeAll<WeaponCollsionLayer>().FirstOrDefault();

        if (!TryGetComponent(out rb))
            Debug.LogError("There's no rigidbody2D attached to " + gameObject.name);
    }

    public void SetBulletProperty(float Damage, float Speed, Vector2 Direction)
    {
        damage = Damage;
        speed = Speed;
        direction = Direction;
    }

    protected virtual void OnHitEnemy(GameObject hitObject)
    {
        Enemy enemy = hitObject.GetComponent<Enemy>();
        enemy.ApplyDamage(damage);
        KillBehaviour(true);
    }

    protected virtual void OnHitWall(GameObject hitObject)
    {
        KillBehaviour(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.collider.gameObject.layer) & collisionLayer.enemyMask) != 0)
            OnHitEnemy(collision.collider.gameObject);
        if (((1 << collision.collider.gameObject.layer) & collisionLayer.wallMask) != 0)
            OnHitWall(collision.collider.gameObject);
    }

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
