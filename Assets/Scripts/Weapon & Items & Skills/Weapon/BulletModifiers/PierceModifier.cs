using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceModifier : BulletModifier
{
    [SerializeField]
    private int pierceCount;

    public override void OnHitEnemy(WeaponBullet bullet, Collider2D collision)
    {
        bullet.SetKillBullet(pierceCount <= 0);
        pierceCount--;
    }

    public override void OnHitWall(WeaponBullet bullet, RaycastHit2D raycast)
    {
    }
}
