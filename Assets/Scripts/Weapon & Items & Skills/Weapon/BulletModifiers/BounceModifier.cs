using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceModifier : BulletModifier
{
    [SerializeField]
    private int bounceCount;

    public override void OnHitEnemy(WeaponBullet bullet, Collider2D collision)
    {
    }

    public override void OnHitWall(WeaponBullet bullet, RaycastHit2D raycast)
    {
        /*int limit = 0;
        while(raycast.collider != null && limit < 5) 
        {
            bullet.transform.position = raycast.point + raycast.normal.normalized * 0.05f;
            bullet.rb.velocity = Vector2.Reflect(bullet.rb.velocity, raycast.normal);
            raycast = Physics2D.Raycast(bullet.transform.position, bullet.rb.velocity, bullet.rb.velocity.magnitude * Time.fixedDeltaTime, CollisionLayer.instance.screenMask);
            limit++;
        }*/

        if (Vector2.Dot(bullet.rb.velocity, raycast.normal) >= 0)
            return;

        bullet.transform.position = raycast.point + raycast.normal.normalized * (bullet.scanRaycastRadius * 1.01f);
        Vector2 velocity = Vector2.Reflect(bullet.rb.velocity, raycast.normal);
        bullet.rb.velocity = Vector2.zero;
        StartCoroutine(DelayBounce(bullet, velocity));

        bullet.SetKillBullet(bounceCount <= 0);
        bounceCount--;
    }

    IEnumerator DelayBounce(WeaponBullet bullet, Vector2 velocity)
    {
        yield return null;
        bullet.rb.velocity = velocity;
    }
}
